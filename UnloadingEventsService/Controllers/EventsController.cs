using ConfigurationService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnloadingEventsService.Models;

namespace UnloadingEventsService.Controllers
{

    public static class EventsController
    {

        public static long ConvertToIdentifier(int series, int number)
        {
            // Серия занимает 8 бит, номер — 16 бит.
            // Сдвигаем серию влево на 16 бит и добавляем номер.
            long fullId = ((long)series << 16) | (uint)number;
            return fullId;
        }


        public static async Task<string> GetEvents(HttpClient client, string token, int NumberOfDaysEvents)
        {
            string strEvents = string.Empty;


            // Зоны вход в которые хотим получить
            var columns = new List<Columns>
            {
                new Columns {column = "in", value = "1"}, // Некотнтролируемая территория
                new Columns {column = "in", value = "13739"}, // КПП
                new Columns {column = "in", value = "28439844"} // КПП2
            };

            var filter = new Filter
            {
                type = "or",
                rows = columns,
                
            };

            var filtersJson = JsonSerializer.Serialize(filter);
            var beginDatetime = DateTime.Now.AddDays(NumberOfDaysEvents * -1).ToString("yyyy-MM-dd HH:mm");
            var endDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            string urlGetEvents = $"eventsystem?beginDatetime={beginDatetime}&endDatetime={endDatetime}&filters={filtersJson}&token={token}";
            

            try
            {
                // Получаем общее количество страниц событий
                var response = await client.GetAsync(urlGetEvents);
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                var totalPages =  JsonSerializer.Deserialize<ApiResponse>(responseBody)?.Total;

                // Циклом получаем все страницы
                for (int i = 1; i <= totalPages; i++) // перебор страниц
                {
                    string urlPageEvents = $"eventsystem?beginDatetime={beginDatetime}&endDatetime={endDatetime}&filters={filtersJson}&page={i}&token={token}";
                    response = await client.GetAsync(urlPageEvents);
                    responseBody = await response.Content.ReadAsStringAsync();
                    var Page =  JsonSerializer.Deserialize<ApiResponse>(responseBody);

                    if (Page?.Rows != null )
                    {

                        foreach (var eventRow in Page.Rows) // перебор рядов
                        {
                            // Конвертация серии и номера карты в идентификатор
                            if (eventRow.Identifier != null)
                            {
                                if (eventRow.Identifier.Contains('/'))
                                {
                                    try
                                    {
                                        int series = int.Parse(eventRow.Identifier.Split('/')[0]);
                                        int number = int.Parse(eventRow.Identifier.Split('/')[1]);
                                        eventRow.Identifier = ConvertToIdentifier(series, number).ToString();
                                    }
                                    catch (Exception ex)
                                    {

                                        Logger.Log($"Ошибка конвертации идентификатора {eventRow.Fio} идентификатор: {eventRow.Identifier} \n {ex.Message}");
                                    }
                                    
                                }
                            }


                            if ((eventRow.EventNameId == 17) && (eventRow.TabelNumber != "") && (eventRow.TabelNumber != null))
                            {
                                

                                strEvents += string.Concat(eventRow.TabelNumber, ";",
                                ((eventRow.ZoneEnterId > 1) ? 0 : 1),
                                $";{eventRow.TimeLabel?.Substring(11)};{eventRow.TimeLabel?.Substring(0, eventRow.TimeLabel.Length - 9)};",
                                eventRow.Identifier, Environment.NewLine);
                            }
                            
                        }
                    }
                    // Конец страницы
                }


                return strEvents;
            }
            catch (Exception ex)
            {
                Logger.Log($"Данные событий не получены {ex.Message}");
                return strEvents;
            }
        }

        public static string PreparationForUnloadingEvents(ApiResponse apiResponse)
        {
            string strEvents = string.Empty;


            if (apiResponse?.Rows != null)
            {
                foreach (var row in apiResponse.Rows)
                {
                    if (row.Identifier != null)
                    {
                        if (row.Identifier.Contains('/'))
                        {
                            row.Identifier = row.Identifier.Replace("/", "");
                        }
                    }
                }
                foreach (var eventRow in apiResponse.Rows)
                {

                    strEvents += string.Concat(eventRow.TabelNumber, ";",
                        ((eventRow.ZoneEnterId > 1) ? 0 : 1),
                        $";{eventRow.TimeLabel?.Substring(11)};{eventRow.TimeLabel?.Substring(0, eventRow.TimeLabel.Length - 9)};",
                        eventRow.Identifier, Environment.NewLine);
                }
            }
            return strEvents;
        }

    }
}
