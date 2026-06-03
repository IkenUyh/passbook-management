using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.NhanVienModel
{
    public class NhanVienRequest
    {
        [JsonPropertyName("hoTen")]
        public string HoTen { get; set; }

        [JsonPropertyName("soDienThoai")]
        public string SoDienThoai { get; set; }

        [JsonPropertyName("cccd")]
        public string Cccd { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; } // "ADMIN" hoặc "NHAN_VIEN"
    }
}
