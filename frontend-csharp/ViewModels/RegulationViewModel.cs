using frontend_csharp.Models;
using System.Collections.ObjectModel;

namespace frontend_csharp.ViewModels
{
    public class RegulationViewModel
    {
        public QuyDinh RegulationInfo { get; set; }

        public ObservableCollection<KyHanModel> DanhSachKyHan { get; set; }

        public RegulationViewModel()
        {
            RegulationInfo = new QuyDinh
            {
                Id = 1,
                TienGuiToiThieu = 1000000,
                TienGuiThemToiThieu = 100000,
                ThoiGianGuiToiThieuNgay = 15,
                LaiSuatKkh = 4.5m,
                LaiSuat3t = 5.0m,
                LaiSuat6t = 5.5m
            };

            DanhSachKyHan = new ObservableCollection<KyHanModel>
            {
                new KyHanModel { STT = 1, TenKyHan = "Không kỳ hạn", LaiSuat = (double)RegulationInfo.LaiSuatKkh, GhiChu = "Gói mặc định" },
                new KyHanModel { STT = 2, TenKyHan = "3 tháng", LaiSuat = (double)RegulationInfo.LaiSuat3t, GhiChu = "Gói mặc định" },
                new KyHanModel { STT = 3, TenKyHan = "6 tháng", LaiSuat = (double)RegulationInfo.LaiSuat6t, GhiChu = "Gói mặc định" }
            };
        }
    }
    public class KyHanModel
    {
        public int STT { get; set; }
        public string TenKyHan { get; set; }
        public double LaiSuat { get; set; }
        public string GhiChu { get; set; }
    }
}