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
                MessageBox.Show("Thay đổi mật khẩu hệ thống thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                PbOldPassword.Clear();
                PbNewPassword.Clear();
                PbConfirmPassword.Clear();
            }
        }
    }
}