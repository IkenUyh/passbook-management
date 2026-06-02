using frontend_csharp.ViewModels;
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
        private Random _random = new Random();
        private bool _isLoaded = false; // Cờ chặn gọi API khi đang gán giá trị mặc định

        public ReportsManagement()
        {
            InitializeComponent();

            _dailyReports = new ObservableCollection<DailyReportModel>();

            dgvDailyReport.ItemsSource = _dailyReports;

            this.Loaded += ReportsManagement_Loaded;
        }

        private async void ReportsManagement_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                var viewModel = this.Resources["ChartViewModel"] as ReportsManagementViewModel;
                if (viewModel != null)
                {
                    viewModel.PlayAnimation();
                }
                return;
            }

            dpDailyDate.SelectedDate = DateTime.Now;

            _isLoaded = true;

            await LoadDailyDataAsync(DateTime.Now);
        }

        // Tự động lọc khi đổi ngày ở bảng Ngày
        private async void dpDailyDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            DateTime selectedDate = dpDailyDate.SelectedDate ?? DateTime.Now;
            await LoadDailyDataAsync(selectedDate);
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
}