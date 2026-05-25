using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace frontend_csharp.UserControls
{
    public partial class CustomerManagement : UserControl
    {
        private ObservableCollection<CustomerModel> _customers;

        public CustomerManagement()
        {
            InitializeComponent();

            _customers = new ObservableCollection<CustomerModel>();

            dgvCustomers.ItemsSource = _customers;
            icCustomersGrid.ItemsSource = _customers;

            this.Loaded += CustomerManagement_Loaded;
        }

        private async void CustomerManagement_Loaded(object sender, RoutedEventArgs e)
        {
            ViewToggleListBox.SelectedIndex = 0;

            // Xóa sạch trạng thái Sort cũ khi chuyển tab quay lại
            if (dgvCustomers.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgvCustomers.ItemsSource);
                if (view != null && view.SortDescriptions.Count > 0)
                {
                    view.SortDescriptions.Clear();
                }

                foreach (var column in dgvCustomers.Columns)
                {
                    column.SortDirection = null;
                }
            }

            if (_customers.Count == 0)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            var newData = await Task.Run(() =>
            {
                var list = new System.Collections.Generic.List<CustomerModel>();
                for (int i = 1; i <= 12; i++)
                {
                    list.Add(new CustomerModel
                    {
                        Id = $"KH-{1000 + i}",
                        FullName = $"Nguyễn Văn Thuận {i}",
                        CitizenId = $"07920400{1234 + i}",
                        PhoneNumber = $"090312345{i:D2}",
                        TotalBooks = i % 3 + 1
                    });
                }
                return list;
            });

            _customers.Clear();

            foreach (var customer in newData)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _customers.Add(customer);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }

    public class CustomerModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string CitizenId { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalBooks { get; set; }
    }
}