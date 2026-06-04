using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        // Properties hiển thị popup kết quả
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
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmAddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewFullName) ||
                string.IsNullOrWhiteSpace(NewCitizenId) ||
                string.IsNullOrWhiteSpace(NewPhoneNumber))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ các trường thông tin!";
                return false;
            }

            ErrorMessage = string.Empty;

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
                ErrorMessage = "Thêm nhân viên thất bại. Vui lòng kiểm tra trùng lặp dữ liệu SĐT/CCCD.";
                return false;
            }

            // Đồng bộ dữ liệu tài khoản từ Server gửi về để điền vào Popup
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
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmEditAsync()
        {
            if (_editingEmployee == null)
            {
                ErrorMessage = "Không tìm thấy thông tin nhân viên cần chỉnh sửa!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(EditFullName) ||
                string.IsNullOrWhiteSpace(EditCitizenId) ||
                string.IsNullOrWhiteSpace(EditPhoneNumber))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ các trường thông tin!";
                return false;
            }

            ErrorMessage = string.Empty;

            var request = new CapNhatNhanVienRequest
            {
                HoTen = EditFullName.Trim(),
                Cccd = EditCitizenId.Trim(),
                SoDienThoai = EditPhoneNumber.Trim()
            };

            bool isSuccess = await _apiService.UpdateNhanVienAsync(_editingEmployee.Id, request);
            if (!isSuccess)
            {
                ErrorMessage = "Cập nhật thông tin thất bại. Vui lòng thử lại.";
                return false;
            }

            await LoadDataAsync();
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}