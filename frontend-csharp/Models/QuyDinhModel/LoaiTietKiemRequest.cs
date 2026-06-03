using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.QuyDinhModel
{
    public class LoaiTietKiemRequest
    {
        [JsonPropertyName("maLoaiTk")]
        public string MaLoaiTk { get; set; }

        [JsonPropertyName("tenLoaiTk")]
        public string TenLoaiTk { get; set; }

        [JsonPropertyName("kyHan")]
        public int? KyHan { get; set; } 

        [JsonPropertyName("laiSuat")]
        public decimal LaiSuat { get; set; }
    }
}
