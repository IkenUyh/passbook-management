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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void EditCustomerMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CustomerModel customer)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    _viewModel.PrepareEdit(customer);

                    var popupUI = (FrameworkElement)this.FindResource("EditCustomerPopupUI");
                    popupUI.DataContext = _viewModel;

                    mainWindow.ShowPopup(popupUI);
                }
            }
        }

        private void AddSavingsBookMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CustomerModel customer)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    _viewModel.PrepareAddSavingsBook(customer);

                    var popupUI = (FrameworkElement)this.FindResource("AddSavingsBookPopupUI");
                    popupUI.DataContext = _viewModel;

                    mainWindow.ShowPopup(popupUI);
                }
            }
        }

        private void ConfirmEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ConfirmEdit())
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.HidePopup();
                }
            }
        }

        private void ConfirmAddSavingsBook_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ConfirmAddSavingsBook())
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.HidePopup();
                }
            }
        }
    }
}