using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.QuyDinhModel
{
    public class CapNhatThamSoRequest
    {
        [JsonPropertyName("tienGuiBanDauToiThieu")]
        public decimal TienGuiBanDauToiThieu { get; set; }

        [JsonPropertyName("tienGuiThemToiThieu")]
        public decimal TienGuiThemToiThieu { get; set; }

        [JsonPropertyName("soNgayGuiToiThieu")]
        public int SoNgayGuiToiThieu { get; set; }
    }
}
