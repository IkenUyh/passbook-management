using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.KhachHangModel
{
    public class KhachHangRequest
    {
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
