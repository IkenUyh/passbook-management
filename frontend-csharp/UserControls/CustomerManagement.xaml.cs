using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using frontend_csharp.ViewModels;

namespace frontend_csharp.UserControls
{
    public partial class CustomerManagement : UserControl
    {
        private readonly CustomerManagementViewModel _viewModel;

        public CustomerManagement()
        {
            InitializeComponent();

            _viewModel = new CustomerManagementViewModel();
            this.DataContext = _viewModel;

            this.Loaded += CustomerManagement_Loaded;
        }

        private async void CustomerManagement_Loaded(object sender, RoutedEventArgs e)
        {
            ViewToggleListBox.SelectedIndex = 0;

            // Giữ nguyên xử lý giao diện: Xóa sạch trạng thái Sort cũ của DataGrid
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

            // Gọi hàm xử lý logic từ ViewModel
            if (_viewModel.Customers.Count == 0)
            {
                await _viewModel.LoadDataAsync();
            }
        }
    }
}