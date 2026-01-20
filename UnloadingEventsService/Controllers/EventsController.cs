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

            StringBuilder strEvents = new StringBuilder();

            // Зоны вход в которые хотим получить
            var columns = new List<Columns>
            {
                new Columns {column = "in", value = "1"}, // Неконтролируемая территория
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

            string urlGetEvents = $"eventsystem?beginDatetime={beginDatetime}&endDatetime={endDatetime}&filters={filtersJson}&rows=10000&token={token}";
            

            try
            {
                // Получаем общее количество страниц событий
                var response = await client.GetAsync(urlGetEvents);
                var responseBody = await response.Content.ReadAsStringAsync();
                var totalPages =  JsonSerializer.Deserialize<ApiResponse>(responseBody)?.Total;

                // Циклом получаем все страницы
                for (int i = 1; i <= totalPages; i++) // перебор страниц
                {
                    string urlPageEvents = $"eventsystem?beginDatetime={beginDatetime}&endDatetime={endDatetime}&filters={filtersJson}&page={i}&rows=10000&token={token}";
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

                            if (!string.IsNullOrEmpty(eventRow.TabelNumber) && (eventRow.EventNameId == 17 || eventRow.EventNameId == 529))
                            {
                                int? direction = null;

                                if(eventRow.ZoneExitId == 1 && (eventRow.ZoneEnterId == 13739 || eventRow.ZoneEnterId == 28439844)) direction = 0; // Вход
                                else if ((eventRow.ZoneExitId == 13739 || eventRow.ZoneExitId == 28439844) && eventRow.ZoneEnterId == 1) direction = 1; // Выход


                                if (direction.HasValue)
                                {
                                    strEvents.AppendLine($"{eventRow.TabelNumber};{direction};{eventRow.TimeLabel?.Substring(11)};" +
                                        $"{eventRow.TimeLabel?.Substring(0, eventRow.TimeLabel.Length - 9)};{eventRow.Identifier}");
                                }
                            }

                        }
                    }
                    // Конец страницы
                    Console.WriteLine($"Отработанна страница {i} из {totalPages}");
                }

                return strEvents.ToString();
            }
            catch (Exception ex)
            {
                Logger.Log($"Данные событий не получены {ex.Message}");
                return strEvents.ToString();
            }
        }


    }
}
