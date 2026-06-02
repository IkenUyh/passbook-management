using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using frontend_csharp.ViewModels;

namespace frontend_csharp.UserControls
{
    public partial class SavingsBookLookup : UserControl
    {
        private readonly SavingsBookLookupViewModel _viewModel;

        public SavingsBookLookup()
        {
            InitializeComponent();

            // Khởi tạo và liên kết DataContext, XAML sẽ tự động nhận diện dữ liệu thông qua Binding
            _viewModel = new SavingsBookLookupViewModel();
            this.DataContext = _viewModel;

            this.Loaded += SavingsBookLookup_Loaded;
        }

        private async void SavingsBookLookup_Loaded(object sender, RoutedEventArgs e)
        {
            // [Giao diện] Reset về DataGrid khi mở lại tab
            ViewToggleListBox.SelectedIndex = 0;

            // [Giao diện] Xoá bộ lọc sắp xếp cũ
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

            // Gọi API nạp dữ liệu từ ViewModel
            if (_viewModel.SavingsBooks.Count == 0)
            {
                await _viewModel.LoadDataAsync();
            }
        }
    }
}
