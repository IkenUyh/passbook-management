using System;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.GiaoDichModel
{
    public class GuiTienRequest
    {
        [JsonPropertyName("maSoTietKiem")]
        public string MaSoTietKiem { get; set; }

        [JsonPropertyName("soTienGui")]
        public decimal SoTienGui { get; set; }

        [JsonPropertyName("ngayGui")]
        public String NgayGui { get; set; }
    }
}