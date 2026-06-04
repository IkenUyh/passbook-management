using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using frontend_csharp.Services;
using frontend_csharp.Models.BaoCaoModel;

namespace frontend_csharp.ViewModels
{
    public class ReportsManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        // ==============================================================================
        // 1. BIẾN CỤC BỘ & DỮ LIỆU TĨNH
        // ==============================================================================
        public List<string> SavingsTypes { get; set; } = new List<string> { "Tất cả" };

        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>
        { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        public ObservableCollection<int> Years { get; set; } = new ObservableCollection<int>();

        // ==============================================================================
        // 2. THUỘC TÍNH BÁO CÁO THÁNG (BIỂU ĐỒ)
        // ==============================================================================
        public ISeries[] BarSeries { get; set; }
        public Axis[] BarXAxes { get; set; }
        public Axis[] BarYAxes { get; set; }

        public ObservableCollection<double> MoSoValues { get; set; } = new ObservableCollection<double>();
        public ObservableCollection<double> DongSoValues { get; set; } = new ObservableCollection<double>();
        public ObservableCollection<string> ChartLabels { get; set; } = new ObservableCollection<string>();

        private string _selectedSavingsType = "Tất cả";
        public string SelectedSavingsType
        {
            get => _selectedSavingsType;
            set { _selectedSavingsType = value; OnPropertyChanged(); TriggerDataLoad(); }
        }

        private string _selectedMonth;
        public string SelectedMonth
        {
            get => _selectedMonth;
            set { _selectedMonth = value; OnPropertyChanged(); TriggerDataLoad(); }
        }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set { _selectedYear = value; OnPropertyChanged(); TriggerDataLoad(); }
        }


        // ==============================================================================
        // 3. THUỘC TÍNH BÁO CÁO NGÀY (BẢNG DATAGRID)
        // ==============================================================================
        public ObservableCollection<DailyReportModel> DailyReports { get; set; } = new ObservableCollection<DailyReportModel>();

        private DateTime _selectedDailyDate = DateTime.Now;
        public DateTime SelectedDailyDate
        {
            get => _selectedDailyDate;
            set
            {
                _selectedDailyDate = value;
                OnPropertyChanged();
                _ = LoadDailyDataAsync(value);
            }
        }

        // ==============================================================================
        // 4. TRẠNG THÁI KIỂM TRA DỮ LIỆU RỖNG
        // ==============================================================================
        private bool _isDailyEmpty = true;
        private bool _isMonthlyEmpty = true;

        // Trạng thái cho thông báo "Không có dữ liệu"
        public Visibility DailyNoDataVisibility => _isDailyEmpty ? Visibility.Visible : Visibility.Collapsed;
        public Visibility MonthlyNoDataVisibility => _isMonthlyEmpty ? Visibility.Visible : Visibility.Collapsed;

        // Trạng thái cho nội dung (Bảng / Biểu đồ)
        public Visibility DailyContentVisibility => _isDailyEmpty ? Visibility.Collapsed : Visibility.Visible;
        public Visibility MonthlyContentVisibility => _isMonthlyEmpty ? Visibility.Collapsed : Visibility.Visible;


        // ==============================================================================
        // 5. HÀM KHỞI TẠO (CONSTRUCTOR)
        // ==============================================================================
        public ReportsManagementViewModel()
        {
            _apiService = new ApiService();

            int currentYear = DateTime.Now.Year;
            for (int y = currentYear - 2; y <= currentYear + 2; y++) Years.Add(y);

            _selectedMonth = DateTime.Now.ToString("MM");
            _selectedYear = currentYear;

            SetupChart();
        }

        public async Task InitializeAsync()
        {
            var dsLoaiTietKiem = await _apiService.GetDanhSachLoaiTietKiemAsync();
            if (dsLoaiTietKiem != null && dsLoaiTietKiem.Any())
            {
                var types = new List<string> { "Tất cả" };
                types.AddRange(dsLoaiTietKiem.Select(x => x.TenLoaiTk));
                SavingsTypes = types;
                OnPropertyChanged(nameof(SavingsTypes));
            }

            await LoadDailyDataAsync(SelectedDailyDate);
            TriggerDataLoad();
        }

        // ==============================================================================
        // 6. LOGIC BIỂU ĐỒ (CHART & ANIMATION)
        // ==============================================================================
        private void TriggerDataLoad()
        {
            if (int.TryParse(SelectedMonth, out int month) && SelectedYear > 0)
            {
                _ = LoadMonthlyDataAsync(SelectedSavingsType, month, SelectedYear);
            }
        }

        private void SetupChart()
        {
            var moSoColor = new SKColor(0x10, 0xB9, 0x81);
            var dongSoColor = new SKColor(0x1F, 0x29, 0x37);

            BarSeries = new ISeries[] {
                new ColumnSeries<double> { Values = MoSoValues, Name = "Mở sổ", Fill = new SolidColorPaint(moSoColor), MaxBarWidth = 35 },
                new ColumnSeries<double> { Values = DongSoValues, Name = "Đóng sổ", Fill = new SolidColorPaint(dongSoColor), MaxBarWidth = 35 }
            };
        }

        public void PlayAnimation()
        {
            SetupChart();
            OnPropertyChanged(nameof(BarSeries));
        }

        // ==============================================================================
        // 7. LOGIC XỬ LÝ & TẢI DỮ LIỆU TỪ API THẬT
        // ==============================================================================
        private async Task LoadDailyDataAsync(DateTime date)
        {
            var data = await _apiService.GetBaoCaoNgayAsync(date);

            DailyReports.Clear();
            if (data != null && data.Any())
            {
                _isDailyEmpty = false;
                int stt = 1;
                foreach (var item in data)
                {
                    DailyReports.Add(new DailyReportModel
                    {
                        Stt = stt++,
                        SavingsType = item.LoaiTietKiem,
                        TotalIn = item.TongThu,
                        TotalOut = item.TongChi
                    });
                }
            }
            else
            {
                _isDailyEmpty = true;
            }

            OnPropertyChanged(nameof(DailyNoDataVisibility));
            OnPropertyChanged(nameof(DailyContentVisibility));
        }

        private async Task LoadMonthlyDataAsync(string savingsType, int month, int year)
        {
            var data = await _apiService.GetBaoCaoThangAsync(month, year);

            MoSoValues.Clear();
            DongSoValues.Clear();
            ChartLabels.Clear();

            if (data != null && data.Any())
            {
                IEnumerable<BaoCaoThangDTO> filteredData = data;
                if (!string.IsNullOrEmpty(savingsType) && savingsType != "Tất cả")
                {
                    filteredData = data.Where(d => d.LoaiTietKiem == savingsType);
                }

                if (filteredData.Any())
                {
                    _isMonthlyEmpty = false;
                    double maxVal = 0;
                    foreach (var item in filteredData)
                    {
                        MoSoValues.Add(item.SoSoMo);
                        DongSoValues.Add(item.SoSoDong);
                        ChartLabels.Add(item.LoaiTietKiem ?? item.MaLoaiTk);

                        maxVal = Math.Max(maxVal, Math.Max(item.SoSoMo, item.SoSoDong));
                    }

                    double magnitude = Math.Floor(Math.Log10(maxVal == 0 ? 1 : maxVal));
                    double roundingStep = Math.Pow(10, magnitude);
                    if (roundingStep == 0) roundingStep = 1;
                    if (maxVal / roundingStep < 2.5) roundingStep /= 2;
                    if (roundingStep == 0) roundingStep = 1;

                    double targetMax = Math.Ceiling((maxVal + 1) / roundingStep) * roundingStep;
                    if (targetMax < 5) targetMax = 5;

                    List<double> ySeparators = new List<double>();
                    for (double i = 0; i <= targetMax; i += Math.Max(1, roundingStep)) ySeparators.Add(i);

                    BarXAxes = new Axis[] { new Axis { Labels = ChartLabels, TextSize = 12 } };
                    BarYAxes = new Axis[] { new Axis { MinLimit = 0, MaxLimit = targetMax, CustomSeparators = ySeparators.ToArray(), TextSize = 12 } };

                    OnPropertyChanged(nameof(BarXAxes));
                    OnPropertyChanged(nameof(BarYAxes));
                }
                else
                {
                    _isMonthlyEmpty = true;
                }
            }
            else
            {
                _isMonthlyEmpty = true;
            }

            OnPropertyChanged(nameof(MonthlyNoDataVisibility));
            OnPropertyChanged(nameof(MonthlyContentVisibility));
        }

        // ==============================================================================
        // 8. SỰ KIỆN CẬP NHẬT GIAO DIỆN (INOTIFYPROPERTYCHANGED)
        // ==============================================================================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // ==============================================================================
    // 9. CÁC LỚP MODEL DỮ LIỆU CỦA BẢNG
    // ==============================================================================
    public class DailyReportModel
    {
        public int Stt { get; set; }
        public string SavingsType { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal Difference => TotalIn - TotalOut;
        public bool IsPositive => Difference >= 0;
    }
}