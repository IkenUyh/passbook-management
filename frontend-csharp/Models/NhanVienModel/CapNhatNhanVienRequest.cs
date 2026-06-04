using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.NhanVienModel
{
    public class CapNhatNhanVienRequest
    {
        [JsonPropertyName("hoTen")]
        public string HoTen { get; set; }

        [JsonPropertyName("soDienThoai")]
        public string SoDienThoai { get; set; }

        [JsonPropertyName("cccd")]
        public string Cccd { get; set; }
    }
}
