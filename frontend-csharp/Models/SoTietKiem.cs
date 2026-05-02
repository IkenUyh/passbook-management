using System;
using System.Text.Json.Serialization;

namespace frontend_csharp.Models
{
    public class SoTietKiem
    {
        // Bên Java gán bằng String nên C# cũng là string
        [JsonPropertyName("id")]
        public string Id { get; set; }

        // Mapping thẳng object KhachHang vào đây
        [JsonPropertyName("khachHang")]
        public KhachHang KhachHang { get; set; }

        [JsonPropertyName("loaiTietKiem")]
        public string LoaiTietKiem { get; set; }

        [JsonPropertyName("soDu")]
        public decimal SoDu { get; set; }

        [JsonPropertyName("ngayMo")]
        public DateTime NgayMo { get; set; }

        [JsonPropertyName("ngayDaoHan")]
        public DateTime? NgayDaoHan { get; set; }

        [JsonPropertyName("trangThai")]
        public string TrangThai { get; set; }
    }
}