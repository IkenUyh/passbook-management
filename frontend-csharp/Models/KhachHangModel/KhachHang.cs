using System.Text.Json.Serialization;

namespace frontend_csharp.Models.KhachHangModel
{
    public class KhachHang
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("ten")]
        public string Ten { get; set; }

        [JsonPropertyName("cmnd")]
        public string Cmnd { get; set; }

        [JsonPropertyName("diaChi")]
        public string DiaChi { get; set; }

        [JsonPropertyName("sdt")]
        public string Sdt { get; set; }
    }
}