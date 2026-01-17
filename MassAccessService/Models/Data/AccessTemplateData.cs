using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MassAccessService.Models.Data
{
    public class AccessTemplateData
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Name { get; set; }
        [JsonPropertyName("comment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Comment { get; set; }
        [JsonPropertyName("is_removed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? IsRemoved { get; set; }

    }
}
