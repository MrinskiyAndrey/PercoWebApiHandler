using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NewEmployeesService.Models.DTO
{
    public class IdData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
