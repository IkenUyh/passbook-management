using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace frontend_csharp.ViewModels
{
    public class ReportsManagementViewModel : INotifyPropertyChanged
    {
        private Random _random = new Random();

        public List<string> SavingsTypes { get; set; } = new List<string> { "Tất cả", "Không kỳ hạn", "3 tháng", "6 tháng", "12 tháng" };

        // 1. Chia lại danh sách siêu ngắn: 12 tháng (dạng chuỗi cho đẹp) và vài năm
        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>
        { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        public ObservableCollection<int> Years { get; set; } = new ObservableCollection<int>();

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
            set
            {
                _selectedSavingsType = value;
                OnPropertyChanged();
                TriggerDataLoad();
            }
        }

        // 2. Thuộc tính riêng cho Tháng
        private string _selectedMonth;
        public string SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
                OnPropertyChanged();
                TriggerDataLoad();
            }
        }

        // 3. Thuộc tính riêng cho Năm
        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                TriggerDataLoad();
            }
        }

        public ReportsManagementViewModel()
        {
            // Nạp vài năm gần đây (vd: từ 2 năm trước đến 2 năm sau)
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear - 2; y <= currentYear + 2; y++)
            {
                Years.Add(y);
            }

            // Đặt mặc định là Tháng / Năm hiện tại
            _selectedMonth = DateTime.Now.ToString("MM");
            _selectedYear = currentYear;

            SetupChart();
            TriggerDataLoad();
        }

        // 4. Hàm hỗ trợ để tự động load dữ liệu mỗi khi đổi tháng hoặc năm
        private void TriggerDataLoad()
        {
            if (int.TryParse(SelectedMonth, out int month) && SelectedYear > 0)
            {
                _ = LoadMonthlyDataAsync(SelectedSavingsType, new DateTime(SelectedYear, month, 1));
            }
        }

        private void SetupChart()
        {
            var moSoColor = new SKColor(0x10, 0xB9, 0x81);
            var dongSoColor = new SKColor(0x1F, 0x29, 0x37);

            BarSeries = new ISeries[] {
                new ColumnSeries<double> { Values = MoSoValues, Name = "Mở sổ", Fill = new SolidColorPaint(moSoColor), MaxBarWidth = 10 },
                new ColumnSeries<double> { Values = DongSoValues, Name = "Đóng sổ", Fill = new SolidColorPaint(dongSoColor), MaxBarWidth = 10 }
            };
        }

        public void PlayAnimation()
        {
            SetupChart();
            OnPropertyChanged(nameof(BarSeries));
        }

        private async Task LoadMonthlyDataAsync(string savingsType, DateTime month)
        {
            var data = await Task.Run(() =>
            {
                var list = new List<MonthlyReportModel>();
                int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
                for (int i = 1; i <= daysInMonth; i++)
                {
                    list.Add(new MonthlyReportModel { DayOfMonth = i, OpenedCount = _random.Next(0, 15), ClosedCount = _random.Next(0, 10) });
                }
                return list;
            });

            double maxVal = 0;
            foreach (var item in data)
            {
                maxVal = Math.Max(maxVal, Math.Max(item.OpenedCount, item.ClosedCount));
            }

            double magnitude = Math.Floor(Math.Log10(maxVal == 0 ? 1 : maxVal));
            double roundingStep = Math.Pow(10, magnitude);
            if (maxVal / roundingStep < 2.5) roundingStep /= 2;
            double targetMax = Math.Ceiling((maxVal + 1) / roundingStep) * roundingStep;

            List<double> ySeparators = new List<double>();
            for (double i = 0; i <= targetMax; i += roundingStep) ySeparators.Add(i);

            MoSoValues.Clear();
            DongSoValues.Clear();
            ChartLabels.Clear();

            foreach (var item in data)
            {
                MoSoValues.Add(item.OpenedCount);
                DongSoValues.Add(item.ClosedCount);
                ChartLabels.Add(item.DayOfMonth.ToString());
            }

            BarXAxes = new Axis[] { new Axis { Labels = ChartLabels, TextSize = 10, MinStep = 1, ForceStepToMin = true } };
            BarYAxes = new Axis[] { new Axis { MinLimit = 0, MaxLimit = targetMax, CustomSeparators = ySeparators.ToArray(), TextSize = 10 } };

            OnPropertyChanged(nameof(BarXAxes));
            OnPropertyChanged(nameof(BarYAxes));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MonthlyReportModel
    {
        public int DayOfMonth { get; set; }
        public int OpenedCount { get; set; }
        public int ClosedCount { get; set; }
    }
}