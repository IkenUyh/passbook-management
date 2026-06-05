using System;
using System.ComponentModel;
using System.Linq;
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

        public async Task LoadDataAsync()
        {
            Username = AppSession.LoggedInUsername ?? "N/A";
            Role = AppSession.CurrentRole ?? "N/A";

            try
            {
                var employees = await _apiService.GetDanhSachNhanVienAsync();

                if (employees == null || employees.Count == 0)
                {
                    ErrorMessage = "Không thể tải danh sách hoặc tài khoản không có quyền xem thông tin nhân viên.";
                    return;
                }

                // Khắc phục lỗi so sánh chuỗi phân biệt hoa thường (Case-Sensitive)
                var currentEmployee = employees.FirstOrDefault(x =>
                    string.Equals(x.Username, AppSession.LoggedInUsername, StringComparison.OrdinalIgnoreCase));

                if (currentEmployee != null)
                {
                    FullName = currentEmployee.HoTen ?? "Chưa cập nhật";
                    PhoneNumber = currentEmployee.SoDienThoai ?? "Chưa cập nhật";
                    CitizenId = currentEmployee.Cccd ?? "Chưa cập nhật";
                    ErrorMessage = string.Empty;
                }
                else
                {
                    ErrorMessage = $"Không tìm thấy hồ sơ chi tiết cho tài khoản '{AppSession.LoggedInUsername}'.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống khi tải thông tin: {ex.Message}";
            }
        }

        public async Task<bool> ConfirmChangePasswordAsync(string oldPassword, string newPassword, string confirmPassword)
        {
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

            string result = await _apiService.DoiMatKhauAsync(request);

            if (result == "Thành công")
            {
                ErrorMessage = string.Empty;
                return true;
            }

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