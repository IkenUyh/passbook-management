using frontend_csharp.Models.QuyDinhModel;
using frontend_csharp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace frontend_csharp.ViewModels
{
    public class RegulationViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        public QuyDinh RegulationInfo { get; set; }
        public ObservableCollection<KyHanModel> DanhSachKyHan { get; set; }

        private KyHanModel _kyHanDangSua;
        public KyHanModel KyHanDangSua
        {
            get => _kyHanDangSua;
            set
            {
                _kyHanDangSua = value;
                OnPropertyChanged();
            }
        }

        // --- CÁC BIẾN DÙNG CHO POPUP THÊM KỲ HẠN MỚI ---
        private string _kyHanMoi;
        public string KyHanMoi
        {
            get => _kyHanMoi;
            set { _kyHanMoi = value; OnPropertyChanged(); }
        }

        private string _laiSuatMoi;
        public string LaiSuatMoi
        {
            get => _laiSuatMoi;
            set { _laiSuatMoi = value; OnPropertyChanged(); }
        }
        // ----------------------------------------------

        public RegulationViewModel()
        {
            _apiService = new ApiService();

            RegulationInfo = new QuyDinh();
            DanhSachKyHan = new ObservableCollection<KyHanModel>();
        }

        public async Task LoadDataAsync()
        {
            // 1. Load tham số chung
            var thamSo = await _apiService.GetThamSoChungAsync();
            if (thamSo != null)
            {
                RegulationInfo.TienGuiToiThieu = thamSo.TienGuiBanDauToiThieu;
                RegulationInfo.TienGuiThemToiThieu = thamSo.TienGuiThemToiThieu;
                RegulationInfo.ThoiGianGuiToiThieuNgay = thamSo.SoNgayGuiToiThieu;

                OnPropertyChanged(nameof(RegulationInfo));
            }

            // 2. Load danh sách loại tiết kiệm
            var dsLoaiTietKiem = await _apiService.GetDanhSachLoaiTietKiemAsync();
            if (dsLoaiTietKiem != null && dsLoaiTietKiem.Any())
            {
                DanhSachKyHan.Clear();
                int stt = 1;
                foreach (var item in dsLoaiTietKiem)
                {
                    int soThang = item.KyHan ?? 0;
                    DanhSachKyHan.Add(new KyHanModel
                    {
                        STT = stt++,
                        MaLoaiTk = item.MaLoaiTk,
                        TenKyHan = soThang == 0 ? "Không kỳ hạn" : soThang.ToString(),
                        KyHan = soThang,
                        LaiSuat = (double)item.LaiSuat
                    });
                }
            }
        }

        public async Task<bool> SaveThamSoAsync()
        {
            var request = new CapNhatThamSoRequest
            {
                TienGuiBanDauToiThieu = RegulationInfo.TienGuiToiThieu,
                TienGuiThemToiThieu = RegulationInfo.TienGuiThemToiThieu,
                SoNgayGuiToiThieu = RegulationInfo.ThoiGianGuiToiThieuNgay
            };

            return await _apiService.UpdateThamSoChungAsync(request);
        }

        public async Task<bool> SaveKyHanAsync()
        {
            if (KyHanDangSua == null) return false;

            var request = new LoaiTietKiemRequest
            {
                MaLoaiTk = KyHanDangSua.MaLoaiTk,
                TenLoaiTk = KyHanDangSua.KyHan == 0 ? "Không kỳ hạn" : KyHanDangSua.KyHan + " Tháng",
                KyHan = KyHanDangSua.KyHan,
                LaiSuat = (decimal)KyHanDangSua.LaiSuat
            };

            return await _apiService.SaveLoaiTietKiemAsync(request);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class KyHanModel
    {
        public int STT { get; set; }
        public string MaLoaiTk { get; set; }
        public string TenKyHan { get; set; }
        public int KyHan { get; set; }
        public double LaiSuat { get; set; }
    }
}