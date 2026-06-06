using System;
using System.Windows;
using System.Windows.Controls;
using frontend_csharp.ViewModels;

namespace frontend_csharp.UserControls
{
    public partial class AccountManagement : UserControl
    {
        private readonly AccountManagementViewModel _viewModel;

        public AccountManagement()
        {
            InitializeComponent();
            _viewModel = new AccountManagementViewModel();
            this.DataContext = _viewModel;
            this.Loaded += AccountManagement_Loaded;
        }

        private async void AccountManagement_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadDataAsync();
        }

        private async void ConfirmChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu an toàn tại biên từ UI PasswordBox chuyển vào ViewModel xử lý
            bool isSuccess = await _viewModel.ConfirmChangePasswordAsync(
                PbOldPassword.Password,
                PbNewPassword.Password,
                PbConfirmPassword.Password
            );

            if (isSuccess)
            {
                // Xóa dữ liệu các ô nhập liệu sau khi thành công
                PbOldPassword.Clear();
                PbNewPassword.Clear();
                PbConfirmPassword.Clear();

                // Lấy đối tượng Window chính để hiển thị Custom Popup thông báo thành công
                dynamic mainWindow = Window.GetWindow(this);
                if (mainWindow != null)
                {
                    try
                    {
                        var successPopupUI = (FrameworkElement)this.FindResource("ChangePasswordSuccessPopupUI");
                        successPopupUI.DataContext = _viewModel;
                        mainWindow.ShowPopup(successPopupUI);
                    }
                    catch
                    {
                        // Phương án dự phòng nếu Window chính không hỗ trợ phương thức ShowPopup
                        MessageBox.Show("Thay đổi mật khẩu hệ thống thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        /// <summary>
        /// Sự kiện đóng popup thông báo thành công
        /// </summary>
        private void CloseSuccessPopup_Click(object sender, RoutedEventArgs e)
        {
            dynamic mainWindow = Window.GetWindow(this);
            mainWindow?.HidePopup();
        }
    }
}