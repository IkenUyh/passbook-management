using System;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.AuditLogModel
{
    public class AuditLog
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("hanhDong")]
        public string HanhDong { get; set; }

        [JsonPropertyName("chiTiet")]
        public string ChiTiet { get; set; }

        [JsonPropertyName("nguoiThucHien")]
        public string NguoiThucHien { get; set; }

        [JsonPropertyName("thoiGian")]
        public DateTime ThoiGian { get; set; }
    }
}