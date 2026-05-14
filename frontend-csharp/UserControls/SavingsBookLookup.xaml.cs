using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace frontend_csharp.UserControls
{
    public partial class SavingsBookLookup : UserControl
    {
        public SavingsBookLookup()
        {
            InitializeComponent();
            this.Loaded += SavingsBookLookup_Loaded;
        }

        private async void SavingsBookLookup_Loaded(object sender, RoutedEventArgs e)
        {
            if (dgvSavingsBooks.ItemsSource != null) return;

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var mockData = await Task.Run(() =>
            {
                var data = new List<SavingsBookModel>();
                for (int i = 0; i < 15; i++)
                {
                    data.Add(new SavingsBookModel
                    {
                        Id = $"FIG-12{i}",
                        SavingsType = "3 tháng",
                        CustomerName = "Nguyễn Văn A",
                        Balance = "500.000 VNĐ",
                        MaturityDate = "05/12/2026",
                        InterestRate = "10%",
                        Status = "Hoạt động"
                    });
                }
                return data;
            });

            // Gán dữ liệu cho cả 2 view
            dgvSavingsBooks.ItemsSource = mockData;
            icSavingsBooksGrid.ItemsSource = mockData;
        }
    }

    public class SavingsBookModel
    {
        public string Id { get; set; }
        public string SavingsType { get; set; }
        public string CustomerName { get; set; }
        public string Balance { get; set; }
        public string MaturityDate { get; set; }
        public string InterestRate { get; set; }
        public string Status { get; set; }
    }
}