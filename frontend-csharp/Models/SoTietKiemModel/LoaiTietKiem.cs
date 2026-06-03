using System.Text.Json.Serialization;

namespace frontend_csharp.Models.SoTietKiem.SoTietKiemModel
{
    public class LoaiTietKiem
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