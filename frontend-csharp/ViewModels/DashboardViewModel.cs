using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using frontend_csharp.Services;

namespace frontend_csharp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        // --- CÁC BIẾN CHO BIỂU ĐỒ ---
        public ISeries[] LineSeries { get; set; }
        public Axis[] LineXAxes { get; set; }
        public Axis[] LineYAxes { get; set; }

        public ISeries[] BarSeries { get; set; }
        public Axis[] BarXAxes { get; set; }
        public Axis[] BarYAxes { get; set; }

        private double[] _rawLineData = new double[7];
        private string[] _lineLabels = new string[7];
        private double[] _rawMoSoData = new double[12];
        private double[] _rawDongSoData = new double[12];

        // --- BIẾN TRẠNG THÁI KIỂM TRA DỮ LIỆU TRỐNG (CHO BIỂU ĐỒ) ---
        private bool _isLineDataEmpty;
        public bool IsLineDataEmpty
        {
            get => _isLineDataEmpty;
            set { _isLineDataEmpty = value; OnPropertyChanged(); }
        }

        private bool _isBarDataEmpty;
        public bool IsBarDataEmpty
        {
            get => _isBarDataEmpty;
            set { _isBarDataEmpty = value; OnPropertyChanged(); }
        }

        // --- CÁC BIẾN CHO THẺ THỐNG KÊ (KPI) ---
        private string _tongThuString = "0 VNĐ";
        public string TongThuString
        {
            get => _tongThuString;
            set { _tongThuString = value; OnPropertyChanged(); }
        }

        private string _tongChiString = "0 VNĐ";
        public string TongChiString
        {
            get => _tongChiString;
            set { _tongChiString = value; OnPropertyChanged(); }
        }

        private string _soLuongSoString = "0";
        public string SoLuongSoString
        {
            get => _soLuongSoString;
            set { _soLuongSoString = value; OnPropertyChanged(); }
        }

        // --- CÁC BIẾN CHO DANH SÁCH SỔ TIẾT KIỆM ---
        private List<SoSapDenHanUI> _soSapDenHanList;
        public List<SoSapDenHanUI> SoSapDenHanList
        {
            get => _soSapDenHanList;
            set { _soSapDenHanList = value; OnPropertyChanged(); }
        }

        private List<SoMoiNhatUI> _soMoiNhatList;
        public List<SoMoiNhatUI> SoMoiNhatList
        {
            get => _soMoiNhatList;
            set { _soMoiNhatList = value; OnPropertyChanged(); }
        }

        private readonly ApiService _apiService;

        public DashboardViewModel()
        {
            _apiService = new ApiService();
            LoadDataAndRenderAsync();
        }

        private async void LoadDataAndRenderAsync()
        {
            // Tải dữ liệu tổng hợp (KPI & 2 danh sách hiển thị)
            await LoadPassbookDataAsync();

            // Tải dữ liệu cho biểu đồ
            await LoadBarChartDataAsync();
            await LoadLineChartDataAsync();

            // Thực hiện dựng và vẽ hoạt ảnh biểu đồ
            PlayAnimation();
        }

        private async Task LoadPassbookDataAsync()
        {
            try
            {
                // 1. Lấy báo cáo ngày hôm nay để tính tổng thu / tổng chi cho các thẻ KPI
                var baoCaoHnay = await _apiService.GetBaoCaoNgayAsync(DateTime.Now);

                decimal tongThu = baoCaoHnay.Sum(x => x.TongThu);
                decimal tongChi = baoCaoHnay.Sum(x => x.TongChi);

                TongThuString = $"{tongThu:N0} VNĐ".Replace(",", ".");
                TongChiString = $"{tongChi:N0} VNĐ".Replace(",", ".");

                // 2. Lấy toàn bộ danh sách sổ một lần duy nhất để tái sử dụng cho các tác vụ bên dưới
                var danhSachSo = await _apiService.GetDanhSachSoTietKiemAsync();

                // Đếm tổng số lượng sổ hiện đang hoạt động
                int soLuong = danhSachSo.Count(s => s.TrangThai == "Đang mở");
                SoLuongSoString = soLuong.ToString();

                // 3. Xử lý danh sách "Sổ sắp đến hạn" (Lọc sổ đang mở, có ngày đáo hạn tính từ hôm nay và xếp gần nhất)
                SoSapDenHanList = danhSachSo
                    .Where(s => s.TrangThai == "Đang mở" && s.NgayDaoHan.HasValue && s.NgayDaoHan.Value >= DateTime.Today)
                    .OrderBy(s => s.NgayDaoHan.Value)
                    .Take(5)
                    .Select(s => new SoSapDenHanUI
                    {
                        Id = s.Id,
                        TenKhachHang = s.KhachHang?.Ten ?? "Ẩn danh",
                        NgayDaoHanText = $"Đáo hạn: {s.NgayDaoHan.Value:dd/MM/yyyy}"
                    })
                    .ToList();

                // 4. Xử lý danh sách "Sổ tiết kiệm mới nhất" (Sắp xếp theo ngày mở sổ giảm dần)
                SoMoiNhatList = danhSachSo
                    .OrderByDescending(s => s.NgayMo)
                    .Take(7)
                    .Select(s => new SoMoiNhatUI
                    {
                        Id = s.Id,
                        TenKhachHang = s.KhachHang?.Ten ?? "Ẩn danh",
                        SoDuText = $"{s.SoDu:N0}đ".Replace(",", ".")
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải dữ liệu danh sách sổ: {ex.Message}");
            }
        }

        private async Task LoadBarChartDataAsync()
        {
            int currentYear = DateTime.Now.Year;

            for (int month = 1; month <= 12; month++)
            {
                var dataThang = await _apiService.GetBaoCaoThangAsync(month, currentYear);

                _rawMoSoData[month - 1] = dataThang.Sum(x => x.SoSoMo);
                _rawDongSoData[month - 1] = dataThang.Sum(x => x.SoSoDong);
            }
        }

        private async Task LoadLineChartDataAsync()
        {
            DateTime currentDate = DateTime.Now;

            for (int i = 6; i >= 0; i--)
            {
                DateTime targetDate = currentDate.AddDays(-i);
                var dataNgay = await _apiService.GetBaoCaoNgayAsync(targetDate);

                _rawLineData[6 - i] = (double)dataNgay.Sum(x => x.TongThu);
                _lineLabels[6 - i] = targetDate.ToString("dd/MM");
            }
        }

        public void PlayAnimation()
        {
            PrepareLineChart();
            PrepareBarChart();
        }

        private void PrepareLineChart()
        {
            double rawMin = _rawLineData.Min();
            double rawMax = _rawLineData.Max();

            // Kiểm tra xem dữ liệu có trống không (mọi giá trị đều = 0)
            IsLineDataEmpty = (rawMax == 0 && rawMin == 0);

            if (rawMax == 0 && rawMin == 0) rawMax = 10;

            double targetMin = System.Math.Floor((rawMin - 0.1) / 5.0) * 5;
            double targetMax = System.Math.Ceiling((rawMax + 0.1) / 5.0) * 5;

            List<double> separators = new List<double>();
            for (double i = targetMin; i <= targetMax; i += 5) separators.Add(i);

            LineSeries = new ISeries[] {
                new LineSeries<double> {
                    Values = _rawLineData,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Black, 2.5f),
                    GeometryFill = new SolidColorPaint(new SKColor(0x2D, 0xB8, 0x7F)),
                    GeometryStroke = new SolidColorPaint(SKColors.White, 2),
                    GeometrySize = 10,
                    LineSmoothness = 0
                }
            };

            LineXAxes = new Axis[] {
                new Axis {
                    Labels = _lineLabels,
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 10
                }
            };

            LineYAxes = new Axis[] {
                new Axis {
                    MinLimit = targetMin - 2,
                    MaxLimit = targetMax + 2,
                    CustomSeparators = separators.ToArray(),
                    Labeler = v => $"{v}tr",
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 9
                }
            };

            OnPropertyChanged(nameof(LineSeries));
            OnPropertyChanged(nameof(LineXAxes));
            OnPropertyChanged(nameof(LineYAxes));
        }

        private void PrepareBarChart()
        {
            double maxVal = _rawMoSoData.Concat(_rawDongSoData).Max();

            // Kiểm tra xem dữ liệu có trống không (mọi giá trị đều = 0)
            IsBarDataEmpty = (maxVal == 0);

            if (maxVal == 0) maxVal = 10;

            double magnitude = System.Math.Floor(System.Math.Log10(maxVal));
            double roundingStep = System.Math.Pow(10, magnitude);
            if (maxVal / roundingStep < 2.5) roundingStep /= 10;
            double targetMax = System.Math.Ceiling((maxVal + 1) / roundingStep) * roundingStep;

            List<double> ySeparators = new List<double>();
            for (double i = 0; i <= targetMax; i += roundingStep) ySeparators.Add(i);

            var moSoColor = new SKColor(0x2D, 0xB8, 0x7F);
            var dongSoColor = new SKColor(0x2D, 0x34, 0x36);

            BarSeries = new ISeries[] {
                new ColumnSeries<double> { Values = _rawMoSoData, Name = "Mở sổ", Fill = new SolidColorPaint(moSoColor), MaxBarWidth = 18 },
                new ColumnSeries<double> { Values = _rawDongSoData, Name = "Đóng sổ", Fill = new SolidColorPaint(dongSoColor), MaxBarWidth = 18 }
            };

            BarXAxes = new Axis[] {
                new Axis {
                    Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                    CustomSeparators = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 },
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 8
                }
            };

            BarYAxes = new Axis[] {
                new Axis {
                    MinLimit = 0,
                    MaxLimit = targetMax,
                    CustomSeparators = ySeparators.ToArray(),
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 10
                }
            };

            OnPropertyChanged(nameof(BarSeries));
            OnPropertyChanged(nameof(BarXAxes));
            OnPropertyChanged(nameof(BarYAxes));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // --- CÁC CLASS ĐÓNG GÓI DỮ LIỆU ĐỂ HIỂN THỊ TRÊN CÁC THÀNH PHẦN DANH SÁCH ---
    public class SoSapDenHanUI
    {
        public string Id { get; set; }
        public string TenKhachHang { get; set; }
        public string NgayDaoHanText { get; set; }
    }

    public class SoMoiNhatUI
    {
        public string Id { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDuText { get; set; }
    }
}