using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace UnloadingEventsService.Models
{
    public class EventRow
    {
        [JsonPropertyName("time_label")]
        public string? TimeLabel { get; set; }

        [JsonPropertyName("tabel_number")]
        public string? TabelNumber { get; set; }

        [JsonPropertyName("fio")]
        public string? Fio { get; set; }

        [JsonPropertyName("division_name")]
        public string? DivisionName { get; set; }

        [JsonPropertyName("division")]
        public int Division { get; set; }

        [JsonPropertyName("event_name")]
        public string? EventName { get; set; }

        [JsonPropertyName("event_name_id")]
        public int EventNameId { get; set; }

        [JsonPropertyName("device_name")]
        public string? DeviceName { get; set; }

        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        [JsonPropertyName("device_zone_id")]
        public int DeviceZoneId { get; set; }

        [JsonPropertyName("ip_address")]
        public string? IpAddress { get; set; }

        [JsonPropertyName("res_name")]
        public string? ResName { get; set; }

        [JsonPropertyName("res_id")]
        public int ResId { get; set; }

        [JsonPropertyName("rst_name")]
        public string? RstName { get; set; }

        [JsonPropertyName("typ_resource_table")]
        public string? TypResourceTable { get; set; }

        [JsonPropertyName("typ_resource_field")]
        public string? TypResourceField { get; set; }

        [JsonPropertyName("resource_number")]
        public int ResourceNumber { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("ident_type")]
        public int? IdentType { get; set; }

        [JsonPropertyName("resource_type")]
        public int ResourceType { get; set; }

        [JsonPropertyName("device_type_id")]
        public int DeviceTypeId { get; set; }

        [JsonPropertyName("zone_exit")]
        public string? ZoneExit { get; set; }

        [JsonPropertyName("zone_exit_id")]
        public int ZoneExitId { get; set; }

        [JsonPropertyName("zone_enter")]
        public string? ZoneEnter { get; set; }

        [JsonPropertyName("zone_enter_id")]
        public int ZoneEnterId { get; set; }

        [JsonPropertyName("user_name")]
        public string? UserName { get; set; }

        [JsonPropertyName("operator_id")]
        public int? OperatorId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }

        [JsonPropertyName("subcategory")]
        public string? Subcategory { get; set; }

        [JsonPropertyName("subcategory_id")]
        public int SubcategoryId { get; set; }

        [JsonPropertyName("position_name")]
        public string? PositionName { get; set; }

        [JsonPropertyName("position")]
        public int? Position { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("db_time_label")]
        public string? DbTimeLabel { get; set; }

        [JsonPropertyName("time_label_utc")]
        public string? TimeLabelUtc { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("segment_id")]
        public int? SegmentId { get; set; }

        [JsonPropertyName("segment_name")]
        public string? SegmentName { get; set; }

        [JsonPropertyName("cars")]
        public string? Cars { get; set; }
    }
}
