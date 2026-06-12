using System.Text.Json.Serialization;

namespace frontend_csharp.Models.SoTietKiem.SoTietKiemModel
{
    public class MoSoRequest
    {
        [JsonPropertyName("tenKhachHang")]
        public string TenKhachHang { get; set; }

        [JsonPropertyName("cmnd")]
        public string Cmnd { get; set; }

        [JsonPropertyName("diaChi")]
        public string DiaChi { get; set; }

        [JsonPropertyName("loaiTietKiem")]
        public string LoaiTietKiem { get; set; }

        [JsonPropertyName("soTienGuiBanDau")]
        public decimal SoTienGuiBanDau { get; set; }
    }
}