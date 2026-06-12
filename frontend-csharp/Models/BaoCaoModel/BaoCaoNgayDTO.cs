using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models.BaoCaoModel
{
    public class BaoCaoNgayDTO
    {
        [JsonPropertyName("maLoaiTk")]
        public string MaLoaiTk { get; set; }

        [JsonPropertyName("loaiTietKiem")]
        public string LoaiTietKiem { get; set; }

        [JsonPropertyName("tongThu")]
        public decimal TongThu { get; set; }

        [JsonPropertyName("tongChi")]
        public decimal TongChi { get; set; }

        [JsonPropertyName("chenhLech")]
        public decimal ChenhLech { get; set; }
    }
}
