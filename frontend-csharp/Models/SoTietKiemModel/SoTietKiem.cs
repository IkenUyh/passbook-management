using System;
using System.Text.Json.Serialization;
using frontend_csharp.Models.KhachHangModel;
using frontend_csharp.Models.SoTietKiemModel;

namespace frontend_csharp.Models.SoTietKiemModel
{
    public class SoTietKiem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("khachHang")]
        public KhachHang KhachHang { get; set; }

        // Sửa từ string thành Object để khớp với @ManyToOne ở backend
        [JsonPropertyName("loaiTietKiem")]
        public Models.SoTietKiem.SoTietKiemModel.LoaiTietKiem LoaiTietKiem { get; set; }

        [JsonPropertyName("soDu")]
        public decimal SoDu { get; set; }

        [JsonPropertyName("ngayMo")]
        public DateTime NgayMo { get; set; }

        [JsonPropertyName("ngayDaoHan")]
        public DateTime? NgayDaoHan { get; set; }

        [JsonPropertyName("trangThai")]
        public string TrangThai { get; set; }

        [JsonPropertyName("version")]
        public long? Version { get; set; }
    }
}