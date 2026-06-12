using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.QuyDinhModel
{
    public class QuyDinh
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("tienGuiToiThieu")]
        public decimal TienGuiToiThieu { get; set; }

        [JsonPropertyName("tienGuiThemToiThieu")]
        public decimal TienGuiThemToiThieu { get; set; }

        [JsonPropertyName("thoiGianGuiToiThieuNgay")]
        public int ThoiGianGuiToiThieuNgay { get; set; }

        [JsonPropertyName("laiSuatKkh")]
        public decimal LaiSuatKkh { get; set; }

        [JsonPropertyName("laiSuat3t")]
        public decimal LaiSuat3t { get; set; }

        [JsonPropertyName("laiSuat6t")]
        public decimal LaiSuat6t { get; set; }
    }
}