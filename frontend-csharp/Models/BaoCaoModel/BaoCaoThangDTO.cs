using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.BaoCaoModel
{
    public class BaoCaoThangDTO
    {
        [JsonPropertyName("maLoaiTk")]
        public string MaLoaiTk { get; set; }

        [JsonPropertyName("loaiTietKiem")]
        public string LoaiTietKiem { get; set; }

        [JsonPropertyName("soSoMo")]
        public int SoSoMo { get; set; }

        [JsonPropertyName("soSoDong")]
        public int SoSoDong { get; set; }

        [JsonPropertyName("chenhLech")]
        public int ChenhLech { get; set; }
    }
}
