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

            if (_viewModel.Customers.Count == 0)
            {
                await _viewModel.LoadDataAsync();
            }
        }

        private void OpenAddCustomerPopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                _viewModel.ResetForm();

                var popupUI = (FrameworkElement)this.FindResource("AddCustomerPopupUI");
                popupUI.DataContext = _viewModel;

                mainWindow.ShowPopup(popupUI);
            }
        }

        private void CloseAddCustomerPopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.HidePopup();
            }
        }

        private void ConfirmAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ConfirmAdd())
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.HidePopup();
                }
            }
        }

        private void Placeholder_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb?.Parent is Grid grid && grid.Children[0] is TextBlock placeholder)
            {
                placeholder.Visibility = string.IsNullOrEmpty(tb.Text) ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}