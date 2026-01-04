using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NewEmployeesService.Models.DTO
{
    public class PositionData
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        [JsonPropertyName ("name")]
        public string Name { get; set; } = "(не определена)";

        [JsonPropertyName("comment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Comment { get; set; }

        [JsonPropertyName("is_removed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? IsRemoved { get; set; }
    }
}
