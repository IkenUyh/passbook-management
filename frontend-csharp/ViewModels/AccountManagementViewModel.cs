using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using frontend_csharp.Models.NhanVienModel;
using frontend_csharp.Services;

namespace frontend_csharp.ViewModels
{
    public class AccountManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        private string _username;
        private string _role;
        private string _fullName;
        private string _phoneNumber;
        private string _citizenId;
        private string _errorMessage;

        // Các thuộc tính binding lỗi cho từng ô nhập liệu
        private string _oldPasswordError;
        private string _newPasswordError;
        private string _confirmPasswordError;

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public string Role
        {
            get { return _role; }
            set
            {
                // CONVERT: Chuyển đổi NHAN_VIEN thành Nhân Viên, còn lại (ADMIN) giữ nguyên
                _role = (value == "NHAN_VIEN") ? "Nhân Viên" : value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        public string CitizenId
        {
            get { return _citizenId; }
            set { _citizenId = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public string OldPasswordError
        {
            get { return _oldPasswordError; }
            set { _oldPasswordError = value; OnPropertyChanged(); }
        }

        public string NewPasswordError
        {
            get { return _newPasswordError; }
            set { _newPasswordError = value; OnPropertyChanged(); }
        }

        public string ConfirmPasswordError
        {
            get { return _confirmPasswordError; }
            set { _confirmPasswordError = value; OnPropertyChanged(); }
        }

        public AccountManagementViewModel()
        {
            _apiService = new ApiService();
        }

        public async Task LoadDataAsync()
        {
            Username = AppSession.LoggedInUsername ?? "N/A";
            Role = AppSession.CurrentRole ?? "N/A";

            try
            {
                var currentEmployee = await _apiService.GetCurrentProfileAsync();

                if (currentEmployee != null)
                {
                    FullName = currentEmployee.HoTen ?? "Chưa cập nhật";
                    PhoneNumber = currentEmployee.SoDienThoai ?? "Chưa cập nhật";
                    CitizenId = currentEmployee.Cccd ?? "Chưa cập nhật";
                    ErrorMessage = string.Empty;

                    // Đồng bộ lại thông tin từ Server
                    Username = currentEmployee.Username ?? Username;
                    Role = currentEmployee.Role ?? Role;
                }
                else
                {
                    ErrorMessage = "Không thể tải thông tin hồ sơ cá nhân.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống khi tải thông tin: {ex.Message}";
            }
        }

        public async Task<bool> ConfirmChangePasswordAsync(string oldPassword, string newPassword, string confirmPassword)
        {
            // Làm sạch các trạng thái lỗi cũ trước khi kiểm tra lượt mới
            OldPasswordError = string.Empty;
            NewPasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;
            ErrorMessage = string.Empty;

            bool hasError = false;

            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                OldPasswordError = "Vui lòng nhập mật khẩu hiện tại.";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                NewPasswordError = "Vui lòng nhập mật khẩu mới.";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ConfirmPasswordError = "Vui lòng xác nhận mật khẩu mới.";
                hasError = true;
            }

            if (hasError) return false;

            if (newPassword != confirmPassword)
            {
                ConfirmPasswordError = "Mật khẩu mới và nhập lại mật khẩu không khớp.";
                return false;
            }

            var request = new DoiMatKhauRequest
            {
                MatKhauCu = oldPassword,
                MatKhauMoi = newPassword
            };

            string result = await _apiService.DoiMatKhauAsync(request);

            if (result == "Thành công")
            {
                return true;
            }

            if (result.Contains("Mật khẩu cũ") || result.Contains("hiện tại"))
            {
                OldPasswordError = result;
            }
            else
            {
                ErrorMessage = result;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}