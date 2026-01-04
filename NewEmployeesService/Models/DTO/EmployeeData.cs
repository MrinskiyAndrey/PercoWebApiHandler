using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NewEmployeesService.Models.DTO
{
    public class EmployeeData
    {
        //[JsonPropertyName("id")]
        [JsonIgnore]
        public int? Id { get; set; }

        [JsonPropertyName("last_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? LastName { get; set; }

        [JsonPropertyName("first_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? FirstName { get; set; }

        [JsonPropertyName("middle_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? MiddleName { get; set; }

        [JsonPropertyName("tabel_number")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? TabelNumber { get; set; }

        [JsonPropertyName("hiring_date")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? HiringDate { get; set; }

        [JsonPropertyName("division")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Division { get; set; }

        [JsonPropertyName("position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Position { get; set; }

        [JsonPropertyName("work_schedule")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? WorkSchedule { get; set; }

        [JsonPropertyName("access_template")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<int>? AccessTemplate { get; set; }

        [JsonPropertyName("begin_datetime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? BeginDatetime { get; set; }

        [JsonPropertyName("end_datetime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? EndDatetime { get; set; }

        [JsonPropertyName("birth_date")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? BirthDate { get; set; }

        [JsonPropertyName("identifier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<IdentifierData>? Identifier { get; set; }

        [JsonPropertyName("barcode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<BarcodeData>? Barcode { get; set; }

        [JsonPropertyName("photo")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Photo { get; set; }

        [JsonPropertyName("additional_fields")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public AdditionalFields? AdditionalFields { get; set; }
    }

    // Класс для объектов внутри массива "identifier"
    public class IdentifierData
    {
        [JsonPropertyName("identifier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Identifier { get; set; }
    }

    // Класс для объектов внутри массива "barcode"
    public class BarcodeData
    {
        [JsonPropertyName("number")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Number { get; set; }

        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Type { get; set; }
    }

    // Класс для объекта "additional_fields"
    public class AdditionalFields
    {
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<TextField>? Text { get; set; }

        [JsonPropertyName("image")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<ImageField>? Image { get; set; }
    }

    // Класс для объектов внутри массива "additional_fields.text"
    public class TextField
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Id { get; set; }

        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Text { get; set; }
    }

    // Класс для объектов внутри массива "additional_fields.image"
    public class ImageField
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Id { get; set; }

        [JsonPropertyName("image")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ImageUrl { get; set; }
    }
}
