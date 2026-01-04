using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NewEmployeesService.Models.DTO
{
    public class DivisionData
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        [JsonRequired]
        [JsonPropertyName("name")]
        public string Name { get; set; } = "(не определено)";

        [JsonPropertyName("parent_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? ParentId { get; set; }

        [JsonPropertyName("is_removed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? IsRemoved { get; set; }

        [JsonPropertyName("tel")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Tel { get; set; }

        [JsonPropertyName("comment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Comment { get; set; }

        [JsonPropertyName("staff_access_templates")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? StaffAccessTemplates { get; set; }

        [JsonPropertyName("accompanying_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? AccompanyingName { get; set; }

        [JsonPropertyName("staff_access_template_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? StaffAccessTemplateName { get; set; }

        [JsonPropertyName("visitor_access_template_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? VisitorAccessTemplateName { get; set; }

        [JsonPropertyName("work_schedule_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? WorkScheduleName { get; set; }

        [JsonPropertyName("node_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? NodeType { get; set; }

        [JsonPropertyName("root_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? RootId { get; set; }

        [JsonPropertyName("staff_access_templates_names")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? StaffAccessTemplatesNames { get; set; }

        [JsonPropertyName("readonly")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? IsReadonly { get; set; }
    }
}
