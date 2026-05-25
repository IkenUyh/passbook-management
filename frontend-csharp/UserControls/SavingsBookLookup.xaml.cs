using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace frontend_csharp.UserControls
{
    public partial class SavingsBookLookup : UserControl
    {
        // 1. Dùng ObservableCollection thay vì List
        private ObservableCollection<SavingsBookModel> _savingsBooks;

        public SavingsBookLookup()
        {
            InitializeComponent();

            _savingsBooks = new ObservableCollection<SavingsBookModel>();

            // 2. Gán ItemsSource MỘT LẦN DUY NHẤT ở đây
            dgvSavingsBooks.ItemsSource = _savingsBooks;
            icSavingsBooksGrid.ItemsSource = _savingsBooks;

            this.Loaded += SavingsBookLookup_Loaded;
        }

        private async void SavingsBookLookup_Loaded(object sender, RoutedEventArgs e)
        {
            // Reset về dạng danh sách (DataGrid - Index 0) mỗi khi tab được mở lại
            ViewToggleListBox.SelectedIndex = 0;

            // Xóa sạch trạng thái Sort cũ khi chuyển tab quay lại
            if (dgvSavingsBooks.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgvSavingsBooks.ItemsSource);
                if (view != null && view.SortDescriptions.Count > 0)
                {
                    view.SortDescriptions.Clear();
                }

                foreach (var column in dgvSavingsBooks.Columns)
                {
                    column.SortDirection = null;
                }
            }

            // Chỉ load lại data nếu danh sách đang trống để tránh spam gọi API khi chuyển tab
            if (_savingsBooks.Count == 0)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            // 1. Giả lập gọi API (Chạy ngầm không ảnh hưởng UI)
            var newDataFromApi = await Task.Run(() =>
            {
                var data = new System.Collections.Generic.List<SavingsBookModel>();
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

            _savingsBooks.Clear();

            // 2. BƠM DỮ LIỆU VÀO UI THREAD Ở CHẾ ĐỘ NỀN (BACKGROUND)
            foreach (var item in newDataFromApi)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _savingsBooks.Add(item);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
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