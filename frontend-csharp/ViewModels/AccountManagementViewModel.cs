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

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public string Role
        {
            get { return _role; }
            set { _role = value; OnPropertyChanged(); }
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

        public AccountManagementViewModel()
        {
            _apiService = new ApiService();
        }

        /// <summary>
        /// Tải thông tin tài khoản cá nhân từ API
        /// </summary>
        public async Task LoadDataAsync()
        {
            Username = AppSession.LoggedInUsername ?? "N/A";
            Role = AppSession.CurrentRole ?? "N/A";

            try
            {
                // GẮN LẠI API: Gọi trực tiếp endpoint lấy thông tin cá nhân của người đang đăng nhập (/v1/nhan-vien/me)
                var currentEmployee = await _apiService.GetCurrentProfileAsync();

                if (currentEmployee != null)
                {
                    FullName = currentEmployee.HoTen ?? "Chưa cập nhật";
                    PhoneNumber = currentEmployee.SoDienThoai ?? "Chưa cập nhật";
                    CitizenId = currentEmployee.Cccd ?? "Chưa cập nhật";

                    // Cập nhật lại Username và Role đồng bộ từ Server nếu cần
                    Username = currentEmployee.Username ?? Username;
                    Role = currentEmployee.Role ?? Role;

                    ErrorMessage = string.Empty;
                }
                else
                {
                    ErrorMessage = "Không thể tải thông tin hồ sơ cá nhân từ hệ thống.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống khi tải thông tin: {ex.Message}";
            }
        }

        /// <summary>
        /// Xử lý thay đổi mật khẩu
        /// </summary>
        public async Task<bool> ConfirmChangePasswordAsync(string oldPassword, string newPassword, string confirmPassword)
        {
            // Kiểm tra và xác thực dữ liệu tại biên
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ tất cả các trường mật khẩu.";
                return false;
            }

            if (newPassword != confirmPassword)
            {
                ErrorMessage = "Mật khẩu mới và nhập lại mật khẩu không khớp.";
                return false;
            }

            var request = new DoiMatKhauRequest
            {
                MatKhauCu = oldPassword,
                MatKhauMoi = newPassword
            };

            // Gọi API thay đổi mật khẩu (/v1/nhan-vien/doi-mat-khau)
            string result = await _apiService.DoiMatKhauAsync(request);

            if (result == "Thành công")
            {
                ErrorMessage = string.Empty;
                return true;
            }

            // Hiển thị thông báo lỗi chi tiết trả về từ Backend
            ErrorMessage = result;
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}