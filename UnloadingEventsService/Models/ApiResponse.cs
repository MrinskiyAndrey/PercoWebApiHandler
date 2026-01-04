using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace UnloadingEventsService.Models
{
    public class ApiResponse
    {
        [JsonPropertyName("rows")]
        public List<EventRow>? Rows { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("records")]
        public int Records { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
