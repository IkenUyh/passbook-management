using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.GiaoDichModel
{
    public class RutTienRequest
    {
        [JsonPropertyName("maSoTietKiem")]
        public string MaSoTietKiem { get; set; }

        // Dùng decimal để tránh sai số tiền tệ khi xử lý tất toán/rút gốc
        [JsonPropertyName("soTienRut")]
        public decimal SoTienRut { get; set; }

        // LocalDate ở Java tương ứng với DateTime trong C#
        [JsonPropertyName("ngayRut")]
        public DateTime NgayRut { get; set; }
    }
}
