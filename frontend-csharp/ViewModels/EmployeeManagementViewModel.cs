using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using frontend_csharp.Services;
using frontend_csharp.Models.NhanVienModel;

namespace frontend_csharp.ViewModels
{
    public class EmployeeManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private List<NhanVien> _allEmployees = new List<NhanVien>();

        private ObservableCollection<NhanVien> _employees;
        public ObservableCollection<NhanVien> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        // Add Properties
        private string _newFullName;
        public string NewFullName
        {
            get => _newFullName;
            set { _newFullName = value; OnPropertyChanged(); }
        }

        private string _newCitizenId;
        public string NewCitizenId
        {
            get => _newCitizenId;
            set { _newCitizenId = value; OnPropertyChanged(); }
        }

        private string _newPhoneNumber;
        public string NewPhoneNumber
        {
            get => _newPhoneNumber;
            set { _newPhoneNumber = value; OnPropertyChanged(); }
        }

        // Add Error Properties
        private string _newFullNameError;
        public string NewFullNameError
        {
            get => _newFullNameError;
            set { _newFullNameError = value; OnPropertyChanged(); }
        }

        private string _newCitizenIdError;
        public string NewCitizenIdError
        {
            get => _newCitizenIdError;
            set { _newCitizenIdError = value; OnPropertyChanged(); }
        }

        private string _newPhoneNumberError;
        public string NewPhoneNumberError
        {
            get => _newPhoneNumberError;
            set { _newPhoneNumberError = value; OnPropertyChanged(); }
        }

        // Properties hiển thị popup kết quả thêm mới
        private string _newUsername;
        public string NewUsername
        {
            get => _newUsername;
            set { _newUsername = value; OnPropertyChanged(); }
        }

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        // Properties hiển thị popup reset mật khẩu
        private string _resetTargetName;
        public string ResetTargetName
        {
            get => _resetTargetName;
            set { _resetTargetName = value; OnPropertyChanged(); }
        }

        private string _resetTargetUsername;
        public string ResetTargetUsername
        {
            get => _resetTargetUsername;
            set { _resetTargetUsername = value; OnPropertyChanged(); }
        }

        private string _resetNewPassword;
        public string ResetNewPassword
        {
            get => _resetNewPassword;
            set { _resetNewPassword = value; OnPropertyChanged(); }
        }

        // Edit Properties
        private NhanVien _editingEmployee;

        private string _editFullName;
        public string EditFullName
        {
            get => _editFullName;
            set { _editFullName = value; OnPropertyChanged(); }
        }

        private string _editCitizenId;
        public string EditCitizenId
        {
            get => _editCitizenId;
            set { _editCitizenId = value; OnPropertyChanged(); }
        }

        private string _editPhoneNumber;
        public string EditPhoneNumber
        {
            get => _editPhoneNumber;
            set { _editPhoneNumber = value; OnPropertyChanged(); }
        }

        // Edit Error Properties
        private string _editFullNameError;
        public string EditFullNameError
        {
            get => _editFullNameError;
            set { _editFullNameError = value; OnPropertyChanged(); }
        }

        private string _editCitizenIdError;
        public string EditCitizenIdError
        {
            get => _editCitizenIdError;
            set { _editCitizenIdError = value; OnPropertyChanged(); }
        }

        private string _editPhoneNumberError;
        public string EditPhoneNumberError
        {
            get => _editPhoneNumberError;
            set { _editPhoneNumberError = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public EmployeeManagementViewModel()
        {
            _apiService = new ApiService();
            Employees = new ObservableCollection<NhanVien>();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                List<NhanVien> apiData = await _apiService.GetDanhSachNhanVienAsync();

                if (apiData == null)
                {
                    throw new InvalidOperationException("Không thể kết nối đến máy chủ hoặc dữ liệu trả về trống.");
                }

                _allEmployees = apiData.Select(n => new NhanVien
                {
                    Id = n.Id,
                    HoTen = n.HoTen,
                    Cccd = n.Cccd,
                    SoDienThoai = n.SoDienThoai,
                    Username = n.Username,
                    Role = n.Role
                }).ToList();

                ApplyFilter();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải danh sách nhân viên: {ex.Message}";
                Console.WriteLine($"[ERROR] LoadDataAsync thất bại: {ex}");
                throw;
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Employees = new ObservableCollection<NhanVien>(_allEmployees);
                return;
            }

            var query = SearchText.Trim().ToLower();

            var filteredList = _allEmployees.Where(e =>
                e.Id.ToString().Contains(query) ||
                (!string.IsNullOrEmpty(e.HoTen) && e.HoTen.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(e.Cccd) && e.Cccd.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(e.SoDienThoai) && e.SoDienThoai.ToLower().Contains(query))
            ).ToList();

            Employees = new ObservableCollection<NhanVien>(filteredList);
        }

        public void ResetForm()
        {
            NewFullName = string.Empty;
            NewCitizenId = string.Empty;
            NewPhoneNumber = string.Empty;

            NewFullNameError = string.Empty;
            NewCitizenIdError = string.Empty;
            NewPhoneNumberError = string.Empty;
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmAddAsync()
        {
            NewFullNameError = string.Empty;
            NewCitizenIdError = string.Empty;
            NewPhoneNumberError = string.Empty;
            ErrorMessage = string.Empty;

            bool hasError = false;

            if (string.IsNullOrWhiteSpace(NewFullName))
            {
                NewFullNameError = "Vui lòng nhập họ và tên.";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(NewCitizenId))
            {
                NewCitizenIdError = "Vui lòng nhập số CCCD.";
                hasError = true;
            }
            else if (!Regex.IsMatch(NewCitizenId.Trim(), @"^\d{12}$"))
            {
                NewCitizenIdError = "Số CCCD không hợp lệ (Phải đúng 12 chữ số).";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(NewPhoneNumber))
            {
                NewPhoneNumberError = "Vui lòng nhập số điện thoại.";
                hasError = true;
            }
            else
            {
                string cleanPhone = Regex.Replace(NewPhoneNumber.Trim(), @"[\s\-\(\)]", "");
                if (!Regex.IsMatch(cleanPhone, @"^\d+$"))
                {
                    NewPhoneNumberError = "Số điện thoại chỉ được chứa chữ số.";
                    hasError = true;
                }
                else if (cleanPhone.Length < 10 || cleanPhone.Length > 11)
                {
                    NewPhoneNumberError = "Số điện thoại phải có từ 10 - 11 chữ số.";
                    hasError = true;
                }
                else if (!cleanPhone.StartsWith("0"))
                {
                    NewPhoneNumberError = "Số điện thoại phải bắt đầu từ số 0.";
                    hasError = true;
                }
            }

            if (hasError) return false;

            var request = new NhanVienRequest
            {
                HoTen = NewFullName.Trim(),
                Cccd = NewCitizenId.Trim(),
                SoDienThoai = NewPhoneNumber.Trim(),
                Role = "NHAN_VIEN"
            };

            var response = await _apiService.CreateNhanVienAsync(request);
            if (response == null)
            {
                ErrorMessage = "Thêm nhân viên thất bại. Kiểm tra trùng lặp dữ liệu SĐT/CCCD.";
                return false;
            }

            NewUsername = response.Username;
            NewPassword = response.Password;

            await LoadDataAsync();
            return true;
        }

        public void PrepareEdit(NhanVien employee)
        {
            _editingEmployee = employee;
            EditFullName = employee.HoTen;
            EditCitizenId = employee.Cccd;
            EditPhoneNumber = employee.SoDienThoai;

            EditFullNameError = string.Empty;
            EditCitizenIdError = string.Empty;
            EditPhoneNumberError = string.Empty;
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmEditAsync()
        {
            if (_editingEmployee == null)
            {
                ErrorMessage = "Không tìm thấy thông tin nhân viên cần chỉnh sửa!";
                return false;
            }

            EditFullNameError = string.Empty;
            EditCitizenIdError = string.Empty;
            EditPhoneNumberError = string.Empty;
            ErrorMessage = string.Empty;

            bool hasError = false;

            if (string.IsNullOrWhiteSpace(EditFullName))
            {
                EditFullNameError = "Vui lòng nhập họ và tên.";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(EditCitizenId))
            {
                EditCitizenIdError = "Vui lòng nhập số CCCD.";
                hasError = true;
            }
            else if (!Regex.IsMatch(EditCitizenId.Trim(), @"^\d{12}$"))
            {
                EditCitizenIdError = "Số CCCD không hợp lệ (Phải đúng 12 chữ số).";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(EditPhoneNumber))
            {
                EditPhoneNumberError = "Vui lòng nhập số điện thoại.";
                hasError = true;
            }
            else
            {
                string cleanPhone = Regex.Replace(EditPhoneNumber.Trim(), @"[\s\-\(\)]", "");
                if (!Regex.IsMatch(cleanPhone, @"^\d+$"))
                {
                    EditPhoneNumberError = "Số điện thoại chỉ được chứa chữ số.";
                    hasError = true;
                }
                else if (cleanPhone.Length < 10 || cleanPhone.Length > 11)
                {
                    EditPhoneNumberError = "Số điện thoại phải có từ 10 - 11 chữ số.";
                    hasError = true;
                }
                else if (!cleanPhone.StartsWith("0"))
                {
                    EditPhoneNumberError = "Số điện thoại phải bắt đầu từ số 0.";
                    hasError = true;
                }
            }

            if (hasError) return false;

            var request = new CapNhatNhanVienRequest
            {
                HoTen = EditFullName.Trim(),
                Cccd = EditCitizenId.Trim(),
                SoDienThoai = EditPhoneNumber.Trim()
            };

            bool isSuccess = await _apiService.UpdateNhanVienAsync(_editingEmployee.Id, request);
            if (!isSuccess)
            {
                ErrorMessage = "Cập nhật thất bại. Kiểm tra trùng lặp dữ liệu SĐT/CCCD.";
                return false;
            }

            await LoadDataAsync();
            return true;
        }

        public void PrepareResetPassword(NhanVien employee)
        {
            _editingEmployee = employee;
            ResetTargetName = employee.HoTen;
            ResetTargetUsername = employee.Username;
            ResetNewPassword = string.Empty;
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmResetPasswordAsync()
        {
            if (_editingEmployee == null)
            {
                ErrorMessage = "Không tìm thấy thông tin nhân viên!";
                return false;
            }

            try
            {
                ErrorMessage = string.Empty;
                string result = await _apiService.ResetMatKhauNhanVienAsync(_editingEmployee.Id);

                if (!string.IsNullOrEmpty(result) && !result.Contains("Lỗi"))
                {
                    ResetNewPassword = result; // Lưu toàn bộ nội dung chuỗi thông báo trả về
                    return true;
                }

                ErrorMessage = string.IsNullOrEmpty(result) ? "Đặt lại mật khẩu thất bại." : result;
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}