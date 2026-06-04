using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using frontend_csharp.ViewModels;
using frontend_csharp.Models.NhanVienModel;

namespace frontend_csharp.UserControls
{
    public partial class EmployeeManagement : UserControl
    {
        private readonly EmployeeManagementViewModel _viewModel;

        public EmployeeManagement()
        {
            InitializeComponent();

            _viewModel = new EmployeeManagementViewModel();
            this.DataContext = _viewModel;

            this.Loaded += EmployeeManagement_Loaded;
        }

        private async void EmployeeManagement_Loaded(object sender, RoutedEventArgs e)
        {
            ViewToggleListBox.SelectedIndex = 0;

            if (dgvEmployees.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgvEmployees.ItemsSource);
                if (view != null && view.SortDescriptions.Count > 0)
                {
                    view.SortDescriptions.Clear();
                }

                foreach (var column in dgvEmployees.Columns)
                {
                    column.SortDirection = null;
                }
            }

            if (_viewModel.Employees.Count == 0)
            {
                await _viewModel.LoadDataAsync();
            }
        }

        private void OpenAddEmployeePopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                _viewModel.ResetForm();
                var popupUI = (FrameworkElement)this.FindResource("AddEmployeePopupUI");
                popupUI.DataContext = _viewModel;
                mainWindow.ShowPopup(popupUI);
            }
        }

        private void CloseEmployeePopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.HidePopup();
        }

        private async void ConfirmAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (await _viewModel.ConfirmAddAsync())
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.HidePopup(); // 1. Đóng ẩn popup nhập liệu cũ

                // 2. Mở popup hiển thị tài khoản thành công lấy từ Resource
                var successPopupUI = (FrameworkElement)this.FindResource("AddEmployeeSuccessPopupUI");
                successPopupUI.DataContext = _viewModel;
                mainWindow?.ShowPopup(successPopupUI);
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

        private void EditEmployeeMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is NhanVien employee)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    _viewModel.PrepareEdit(employee);
                    var popupUI = (FrameworkElement)this.FindResource("EditEmployeePopupUI");
                    popupUI.DataContext = _viewModel;
                    mainWindow.ShowPopup(popupUI);
                }
            }
        }

        private async void ConfirmEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (await _viewModel.ConfirmEditAsync())
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.HidePopup();
            }
        }
    }
}