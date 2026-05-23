using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace frontend_csharp.UserControls
{
    public partial class ReportsManagement : UserControl
    {
        private ObservableCollection<DailyReportModel> _dailyReports;
        private ObservableCollection<MonthlyReportModel> _monthlyReports;
        private Random _random = new Random();
        private bool _isLoaded = false; // Cờ chặn gọi API khi đang gán giá trị mặc định

        public ReportsManagement()
        {
            InitializeComponent();

            _dailyReports = new ObservableCollection<DailyReportModel>();
            _monthlyReports = new ObservableCollection<MonthlyReportModel>();

            dgvDailyReport.ItemsSource = _dailyReports;
            dgvMonthlyReport.ItemsSource = _monthlyReports;

            this.Loaded += ReportsManagement_Loaded;
        }

        private async void ReportsManagement_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded) return;

            cboSavingsType.ItemsSource = new List<string> { "Tất cả", "Không kỳ hạn", "3 tháng", "6 tháng", "12 tháng" };
            cboSavingsType.SelectedIndex = 0;

            dpDailyDate.SelectedDate = DateTime.Now;
            dpMonthlyDate.SelectedDate = DateTime.Now;

            _isLoaded = true;

            await LoadDailyDataAsync(DateTime.Now);
            await LoadMonthlyDataAsync("Tất cả", DateTime.Now);
        }

        // Tự động lọc khi đổi ngày ở bảng Ngày
        private async void dpDailyDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            DateTime selectedDate = dpDailyDate.SelectedDate ?? DateTime.Now;
            await LoadDailyDataAsync(selectedDate);
        }

        // Tự động lọc khi đổi loại tiết kiệm ở bảng Tháng
        private async void cboSavingsType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            string savingsType = cboSavingsType.SelectedItem?.ToString() ?? "Tất cả";
            DateTime selectedMonth = dpMonthlyDate.SelectedDate ?? DateTime.Now;
            await LoadMonthlyDataAsync(savingsType, selectedMonth);
        }

        // Tự động lọc khi đổi tháng ở bảng Tháng
        private async void dpMonthlyDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            string savingsType = cboSavingsType.SelectedItem?.ToString() ?? "Tất cả";
            DateTime selectedMonth = dpMonthlyDate.SelectedDate ?? DateTime.Now;
            await LoadMonthlyDataAsync(savingsType, selectedMonth);
        }

        private async Task LoadDailyDataAsync(DateTime date)
        {
            var data = await Task.Run(() =>
            {
                var list = new List<DailyReportModel>();
                string[] types = { "Không kỳ hạn", "3 tháng", "6 tháng", "12 tháng" };

                for (int i = 0; i < types.Length; i++)
                {
                    decimal totalIn = _random.Next(10, 500) * 1000000;
                    decimal totalOut = _random.Next(5, 300) * 1000000;

                    list.Add(new DailyReportModel
                    {
                        Stt = i + 1,
                        SavingsType = types[i],
                        TotalIn = totalIn,
                        TotalOut = totalOut
                    });
                }
                return list;
            });

            _dailyReports.Clear();
            foreach (var item in data) _dailyReports.Add(item);
        }

        private async Task LoadMonthlyDataAsync(string savingsType, DateTime month)
        {
            var data = await Task.Run(() =>
            {
                var list = new List<MonthlyReportModel>();
                int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);

                for (int i = 1; i <= daysInMonth; i++)
                {
                    int opened = _random.Next(0, 15);
                    int closed = _random.Next(0, 10);

                    list.Add(new MonthlyReportModel
                    {
                        Stt = i,
                        Date = $"{i:00}/{month.Month:00}/{month.Year}",
                        OpenedCount = opened,
                        ClosedCount = closed
                    });
                }
                return list;
            });

            _monthlyReports.Clear();
            foreach (var item in data) _monthlyReports.Add(item);
        }
    }

    public class DailyReportModel
    {
        public int Stt { get; set; }
        public string SavingsType { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal Difference => TotalIn - TotalOut;
        public bool IsPositive => Difference >= 0;
    }

    public class MonthlyReportModel
    {
        public int Stt { get; set; }
        public string Date { get; set; }
        public int OpenedCount { get; set; }
        public int ClosedCount { get; set; }
        public int Difference => OpenedCount - ClosedCount;
        public bool IsPositive => Difference >= 0;
    }
}