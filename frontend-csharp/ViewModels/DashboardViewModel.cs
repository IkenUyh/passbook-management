using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using frontend_csharp.Models.SoTietKiemModel;

namespace frontend_csharp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public int CurrentYear => DateTime.Now.Year;

        private decimal _tongThuTrongNgay;
        public decimal TongThuTrongNgay { get => _tongThuTrongNgay; set { _tongThuTrongNgay = value; OnPropertyChanged(); } }

        private decimal _tongChiTrongNgay;
        public decimal TongChiTrongNgay { get => _tongChiTrongNgay; set { _tongChiTrongNgay = value; OnPropertyChanged(); } }

        private int _soLuongSoDangMo;
        public int SoLuongSoDangMo { get => _soLuongSoDangMo; set { _soLuongSoDangMo = value; OnPropertyChanged(); } }

        public ObservableCollection<SoTietKiem> SoSapDenHanList { get; set; } = new ObservableCollection<SoTietKiem>();
        public ObservableCollection<SoTietKiem> SoMoiNhatList { get; set; } = new ObservableCollection<SoTietKiem>();
        public ISeries[] LineSeries { get; set; }
        public Axis[] LineXAxes { get; set; }
        public Axis[] LineYAxes { get; set; }

        public ISeries[] BarSeries { get; set; }
        public Axis[] BarXAxes { get; set; }
        public Axis[] BarYAxes { get; set; }

        private double[] _rawLineData = new double[7];
        private double[] _rawMoSoData = new double[12];
        private double[] _rawDongSoData = new double[12];
        private string[] _lineLabels = new string[7];

        public DashboardViewModel()
        {
            // Initialize empty charts
            PrepareLineChart();
            PrepareBarChart();
            _ = LoadDataAsync();
        }

        public void PlayAnimation()
        {
            PrepareLineChart();
            PrepareBarChart();
        }

        private async Task LoadDataAsync()
        {
            var apiService = new frontend_csharp.Services.ApiService();

            // 1. Tổng thu/chi hôm nay
            var baoCaoNgay = await apiService.GetBaoCaoNgayAsync(DateTime.Now);
            decimal tongThu = 0;
            decimal tongChi = 0;
            if (baoCaoNgay != null)
            {
                foreach (var item in baoCaoNgay)
                {
                    tongThu += item.TongThu;
                    tongChi += item.TongChi;
                }
            }
            
            App.Current.Dispatcher.Invoke(() => {
                TongThuTrongNgay = tongThu;
                TongChiTrongNgay = tongChi;
            });

            // 2. Danh sách sổ
            var danhSachSo = await apiService.GetDanhSachSoTietKiemAsync();
            if (danhSachSo != null)
            {
                var dangMo = danhSachSo.Count(s => !string.Equals(s.TrangThai, "Đã đóng", StringComparison.OrdinalIgnoreCase));
                
                var topMoiNhat = danhSachSo.OrderByDescending(s => s.NgayMo).Take(7).ToList();

                App.Current.Dispatcher.Invoke(() => {
                    SoLuongSoDangMo = dangMo;
                    SoMoiNhatList.Clear();
                    foreach (var s in topMoiNhat) SoMoiNhatList.Add(s);
                });
            }

            var soSapDaoHan = await apiService.GetDanhSachSoSapDaoHanAsync(7); // 7 days
            if (soSapDaoHan != null)
            {
                var topSapDaoHan = soSapDaoHan.OrderBy(s => s.NgayDaoHan).Take(5).ToList();
                App.Current.Dispatcher.Invoke(() => {
                    SoSapDenHanList.Clear();
                    foreach (var s in topSapDaoHan) SoSapDenHanList.Add(s);
                });
            }

            // 3. Biến động số dư 7 ngày qua
            for (int i = 0; i < 7; i++)
            {
                DateTime date = DateTime.Now.AddDays(-6 + i);
                _lineLabels[i] = date.ToString("dd/MM");
                
                var dataNgay = await apiService.GetBaoCaoNgayAsync(date);
                decimal sum = 0;
                if (dataNgay != null)
                {
                    foreach (var item in dataNgay) sum += (item.TongThu - item.TongChi);
                }
                _rawLineData[i] = (double)sum / 1000000.0; // Show in millions
            }

            // 4. Mở/đóng sổ theo tháng (năm nay)
            int currentYear = DateTime.Now.Year;
            var tasks = new List<Task<(int month, List<frontend_csharp.Models.BaoCaoModel.BaoCaoThangDTO> data)>>();
            for (int m = 1; m <= 12; m++)
            {
                int month = m;
                tasks.Add(Task.Run(async () => {
                    var api = new frontend_csharp.Services.ApiService();
                    var data = await api.GetBaoCaoThangAsync(month, currentYear);
                    return (month, data);
                }));
            }

            var results = await Task.WhenAll(tasks);
            foreach (var res in results.OrderBy(r => r.month))
            {
                int moSo = 0;
                int dongSo = 0;
                if (res.data != null)
                {
                    foreach(var item in res.data)
                    {
                        moSo += item.SoSoMo;
                        dongSo += item.SoSoDong;
                    }
                }
                _rawMoSoData[res.month - 1] = moSo;
                _rawDongSoData[res.month - 1] = dongSo;
            }

            App.Current.Dispatcher.Invoke(() => {
                PrepareLineChart();
                PrepareBarChart();
            });
        }

        private void PrepareLineChart()
        {
            double rawMin = _rawLineData.Min();
            double rawMax = _rawLineData.Max();

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
            if (maxVal == 0) maxVal = 10;

            double magnitude = System.Math.Floor(System.Math.Log10(maxVal));
            double roundingStep = System.Math.Pow(10, magnitude);
            if (maxVal / roundingStep < 2.5) roundingStep /= 2;
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
}