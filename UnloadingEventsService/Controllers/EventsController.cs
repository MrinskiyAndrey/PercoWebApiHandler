using ConfigurationService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnloadingEventsService.Models;

namespace UnloadingEventsService.Controllers
{
    public static class EventsController
    {
        private const int ZoneUncontrolled = 1;
        private const int ZoneKpp1 = 13739;
        private const int ZoneKpp2 = 28439844;

        private const int EventEnter = 17;
        private const int EventExit = 529;

        public static long ConvertToIdentifier(int series, int number)
        {
            long fullId = ((long)series << 16) | (uint)number;
            return fullId;
        }

        public static async Task<string> GetEvents(
            HttpClient client,
            string token,
            int numberOfDaysEvents,
            int maxConcurrency = 3)
        {
            if (numberOfDaysEvents <= 0) numberOfDaysEvents = 1;

            var filtersJson = BuildFiltersJson();
            var filtersEncoded = Uri.EscapeDataString(filtersJson);

            // Разбивка по календарным суткам: [00:00 дня, 00:00 следующего дня)
            var today = DateTime.Now.Date;
            var ranges = new List<(DateTime Begin, DateTime End)>(numberOfDaysEvents);
            for (int d = numberOfDaysEvents; d >= 1; d--)
            {
                var b = today.AddDays(-d);
                var e = today.AddDays(-d + 1);
                ranges.Add((b, e));
            }
            // Последний (сегодняшний) — от полуночи до текущего момента
            ranges.Add((today, DateTime.Now));

            using var semaphore = new SemaphoreSlim(maxConcurrency);

            var tasks = new Task<string>[ranges.Count];
            for (int i = 0; i < ranges.Count; i++)
            {
                int idx = i;
                var (b, e) = ranges[idx];

                tasks[idx] = Task.Run(async () =>
                {
                    await semaphore.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        return await GetEventsForRange(client, token, b, e, filtersEncoded)
                                     .ConfigureAwait(false);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }

            var parts = await Task.WhenAll(tasks).ConfigureAwait(false);

            // Собираем все строки в один список
            var items = new List<(string Key, string Line)>();
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                foreach (var line in part.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    // Формат строки: TabelNumber;direction;HH:mm:ss;yyyy-MM-dd;Identifier
                    // Значит date = [3], time = [2].
                    var p = line.Split(';');
                    if (p.Length < 4) continue;

                    items.Add((p[3] + " " + p[2], line.TrimEnd('\r')));
                }
            }

            // Сортировка: старые сверху (по возрастанию даты и времени)
            items.Sort((x, y) => string.CompareOrdinal(x.Key, y.Key));

            var sb = new StringBuilder(items.Count * 48);
            foreach (var it in items) sb.AppendLine(it.Line);
            return sb.ToString();
        }

        private static string BuildFiltersJson()
        {
            var columns = new List<Columns>
            {
                new Columns { column = "in", value = ZoneUncontrolled.ToString() },
                new Columns { column = "in", value = ZoneKpp1.ToString() },
                new Columns { column = "in", value = ZoneKpp2.ToString() }
            };

            var filter = new Filter
            {
                type = "or",
                rows = columns
            };

            return JsonSerializer.Serialize(filter);
        }

        private static async Task<string> GetEventsForRange(
            HttpClient client,
            string token,
            DateTime begin,
            DateTime end,
            string filtersEncoded)
        {
            var sb = new StringBuilder();

            var beginStr = Uri.EscapeDataString(begin.ToString("yyyy-MM-dd HH:mm"));
            var endStr = Uri.EscapeDataString(end.ToString("yyyy-MM-dd HH:mm"));

            try
            {
                string firstUrl =
                    $"eventsystem?beginDatetime={beginStr}" +
                    $"&endDatetime={endStr}" +
                    $"&filters={filtersEncoded}&page=1&rows=10000&token={token}";

                var firstBody = await client.GetStringAsync(firstUrl).ConfigureAwait(false);
                var firstPage = JsonSerializer.Deserialize<ApiResponse>(firstBody);
                var totalPages = firstPage?.Total ?? 0;

                if (firstPage?.Rows != null)
                    AppendRows(sb, firstPage.Rows);

                for (int i = 2; i <= totalPages; i++)
                {
                    string urlPage =
                        $"eventsystem?beginDatetime={beginStr}" +
                        $"&endDatetime={endStr}" +
                        $"&filters={filtersEncoded}&page={i}&rows=10000&token={token}";

                    var body = await client.GetStringAsync(urlPage).ConfigureAwait(false);
                    var page = JsonSerializer.Deserialize<ApiResponse>(body);

                    if (page?.Rows != null)
                        AppendRows(sb, page.Rows);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Данные событий не получены за {begin:yyyy-MM-dd}..{end:yyyy-MM-dd}: {ex.Message}");
            }

            return sb.ToString();
        }

        private static void AppendRows(StringBuilder sb, IEnumerable<EventRow> rows)
        {
            foreach (var eventRow in rows)
            {
                var identifier = eventRow.Identifier;
                if (!string.IsNullOrEmpty(identifier) && identifier.Contains('/'))
                {
                    var parts = identifier.Split('/');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out var series) &&
                        int.TryParse(parts[1], out var number))
                    {
                        eventRow.Identifier = ConvertToIdentifier(series, number).ToString();
                    }
                    else
                    {
                        Logger.Log($"Ошибка конвертации идентификатора {eventRow.Fio} идентификатор: {identifier}");
                    }
                }

                if (string.IsNullOrEmpty(eventRow.TabelNumber)) continue;
                if (eventRow.EventNameId != EventEnter && eventRow.EventNameId != EventExit) continue;

                int? direction = null;
                if (eventRow.ZoneExitId == ZoneUncontrolled &&
                    (eventRow.ZoneEnterId == ZoneKpp1 || eventRow.ZoneEnterId == ZoneKpp2))
                    direction = 0;
                else if ((eventRow.ZoneExitId == ZoneKpp1 || eventRow.ZoneExitId == ZoneKpp2) &&
                         eventRow.ZoneEnterId == ZoneUncontrolled)
                    direction = 1;

                if (!direction.HasValue) continue;

                var timeLabel = eventRow.TimeLabel;
                if (string.IsNullOrEmpty(timeLabel) || timeLabel.Length < 16) continue;

                var time = timeLabel.Substring(11);
                var date = timeLabel.Substring(0, timeLabel.Length - 9);

                sb.AppendLine($"{eventRow.TabelNumber};{direction};{time};{date};{eventRow.Identifier}");
            }
        }
    }
}