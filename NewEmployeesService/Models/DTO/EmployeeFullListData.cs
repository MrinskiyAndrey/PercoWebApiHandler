using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NewEmployeesService.Models.DTO
{
    public class EmployeeFullListData
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        [JsonPropertyName("last_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? LastName { get; set; }

        [JsonPropertyName("first_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? FirstName { get; set; }

        [JsonPropertyName("middle_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? MiddleName { get; set; }

        [JsonPropertyName("division_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int DivisionId { get; set; }

        [JsonPropertyName("position_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? PositionId { get; set; }

        [JsonPropertyName("is_active")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int IsActive { get; set; }

        [JsonPropertyName("hiring_date")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? HiringDate { get; set; }

        [JsonPropertyName("tabel_number")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? TabelNumber { get; set; }

        [JsonPropertyName("work_schedule_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? WorkScheduleId { get; set; }

        [JsonPropertyName("dismissed_date")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? DismissedDate { get; set; }

        [JsonPropertyName("birth_date")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? BirthDate { get; set; }

        [JsonPropertyName("division_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? DivisionName { get; set; }

        [JsonPropertyName("position_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PositionName { get; set; }

        [JsonPropertyName("work_schedule_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? WorkScheduleName { get; set; }
    }
}
