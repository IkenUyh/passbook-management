using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.NhanVienModel
{
    public class DoiMatKhauRequest
    {
        [JsonPropertyName("matKhauCu")]
        public string MatKhauCu { get; set; }

        [JsonPropertyName("matKhauMoi")]
        public string MatKhauMoi { get; set; }
    }
}
