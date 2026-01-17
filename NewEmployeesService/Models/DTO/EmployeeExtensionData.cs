using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NewEmployeesService.Models.DTO
{
    public class EmployeeExtensionData
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("middle_name")]
        public string? MiddleName { get; set; }

        [JsonPropertyName("tabel_number")]
        public string? TabelNumber { get; set; }

        [JsonPropertyName("hiring_date")]
        public string? HiringDate { get; set; }

        [JsonPropertyName("is_active")]
        public int IsActive { get; set; }

        [JsonPropertyName("is_block")]
        public int IsBlock { get; set; }

        [JsonPropertyName("dismissed_date")]
        public string? DismissedDate { get; set; }

        [JsonPropertyName("begin_datetime")]
        public string? BeginDatetime { get; set; }

        [JsonPropertyName("end_datetime")]
        public string? EndDatetime { get; set; }

        [JsonPropertyName("birth_date")]
        public string? BirthDate { get; set; }

        [JsonPropertyName("photo")]
        public string? Photo { get; set; }

        // Используем Dictionary, так как ключи "3746905" динамические
        [JsonPropertyName("division")]
        public Dictionary<string, string>? Division { get; set; }

        [JsonPropertyName("position")]
        public Dictionary<string, string>? Position { get; set; }

        [JsonPropertyName("access_template")]
        public List<Dictionary<string, string>>? AccessTemplate { get; set; }

        [JsonPropertyName("identifier")]
        public List<Identifier>? Identifiers { get; set; }

        [JsonPropertyName("barcode")]
        public List<object>? Barcode { get; set; }

        [JsonPropertyName("additional_fields")]
        public AdditionalFields? AdditionalFields { get; set; }
    }

    public class Identifier
    {
        [JsonPropertyName("identifier")]
        public string? Value { get; set; }

        [JsonPropertyName("is_universal")]
        public bool IsUniversal { get; set; }
    }


    public class AdditionalTextField
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type_id")]
        public int TypeId { get; set; }

        [JsonPropertyName("default_value")]
        public object? DefaultValue { get; set; }

        [JsonPropertyName("items")]
        public object? Items { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("is_const")]
        public int IsConst { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

}
