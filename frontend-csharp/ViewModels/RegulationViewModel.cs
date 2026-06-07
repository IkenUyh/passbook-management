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

        private string _editKyHan;
        public string EditKyHan
        {
            get => _editKyHan;
            set { _editKyHan = value; OnPropertyChanged(); }
        }

        private string _editLaiSuat;
        public string EditLaiSuat
        {
            get => _editLaiSuat;
            set { _editLaiSuat = value; OnPropertyChanged(); }
        }

        private string _popupMessage;
        public string PopupMessage
        {
            get => _popupMessage;
            set { _popupMessage = value; OnPropertyChanged(); }
        }

        private string _tienGuiToiThieuText;
        public string TienGuiToiThieuText
        {
            get => _tienGuiToiThieuText;
            set { _tienGuiToiThieuText = value; OnPropertyChanged(); }
        }

        private string _tienGuiThemToiThieuText;
        public string TienGuiThemToiThieuText
        {
            get => _tienGuiThemToiThieuText;
            set { _tienGuiThemToiThieuText = value; OnPropertyChanged(); }
        }

        private string _thoiGianGuiToiThieuNgayText;
        public string ThoiGianGuiToiThieuNgayText
        {
            get => _thoiGianGuiToiThieuNgayText;
            set { _thoiGianGuiToiThieuNgayText = value; OnPropertyChanged(); }
        }

        // --- ERROR MESSAGES ---
        private string _generalErrorMessage;
        public string GeneralErrorMessage
        {
            get => _generalErrorMessage;
            set { _generalErrorMessage = value; OnPropertyChanged(); }
        }

        private string _tienGuiToiThieuError;
        public string TienGuiToiThieuError
        {
            get => _tienGuiToiThieuError;
            set { _tienGuiToiThieuError = value; OnPropertyChanged(); }
        }

        private string _tienGuiThemToiThieuError;
        public string TienGuiThemToiThieuError
        {
            get => _tienGuiThemToiThieuError;
            set { _tienGuiThemToiThieuError = value; OnPropertyChanged(); }
        }

        private string _thoiGianGuiToiThieuNgayError;
        public string ThoiGianGuiToiThieuNgayError
        {
            get => _thoiGianGuiToiThieuNgayError;
            set { _thoiGianGuiToiThieuNgayError = value; OnPropertyChanged(); }
        }

        private string _kyHanMoiError;
        public string KyHanMoiError
        {
            get => _kyHanMoiError;
            set { _kyHanMoiError = value; OnPropertyChanged(); }
        }

        private string _laiSuatMoiError;
        public string LaiSuatMoiError
        {
            get => _laiSuatMoiError;
            set { _laiSuatMoiError = value; OnPropertyChanged(); }
        }

        private string _kyHanDangSuaError;
        public string KyHanDangSuaError
        {
            get => _kyHanDangSuaError;
            set { _kyHanDangSuaError = value; OnPropertyChanged(); }
        }

        private string _laiSuatDangSuaError;
        public string LaiSuatDangSuaError
        {
            get => _laiSuatDangSuaError;
            set { _laiSuatDangSuaError = value; OnPropertyChanged(); }
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

                TienGuiToiThieuText = thamSo.TienGuiBanDauToiThieu.ToString("N0");
                TienGuiThemToiThieuText = thamSo.TienGuiThemToiThieu.ToString("N0");
                ThoiGianGuiToiThieuNgayText = thamSo.SoNgayGuiToiThieu.ToString();

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