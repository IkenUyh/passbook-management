using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using frontend_csharp.Services;
using frontend_csharp.Models.KhachHangModel;
using frontend_csharp.Models.SoTietKiem.SoTietKiemModel;
using frontend_csharp.Models.SoTietKiemModel;

namespace frontend_csharp.ViewModels
{
    public class CustomerManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private List<KhachHang> _allCustomers = new List<KhachHang>();

        private ObservableCollection<KhachHang> _customers;
        public static event Action OnSavingsBookAdded;
        public ObservableCollection<KhachHang> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
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

        // Khai báo lỗi thành phần cho Form thêm mới
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

        private KhachHang _editingCustomer;

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

        // Khai báo lỗi thành phần cho Form chỉnh sửa
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

        private KhachHang _savingsBookTargetCustomer;

        private ObservableCollection<LoaiTietKiem> _savingsTypes;
        public ObservableCollection<LoaiTietKiem> SavingsTypes
        {
            get => _savingsTypes;
            set { _savingsTypes = value; OnPropertyChanged(); }
        }

        private string _selectedSavingsType;
        public string SelectedSavingsType
        {
            get => _selectedSavingsType;
            set { _selectedSavingsType = value; OnPropertyChanged(); }
        }

        private string _initialDeposit;
        public string InitialDeposit
        {
            get => _initialDeposit;
            set { _initialDeposit = value; OnPropertyChanged(); }
        }

        // Khai báo lỗi thành phần cho Form mở sổ
        private string _initialDepositError;
        public string InitialDepositError
        {
            get => _initialDepositError;
            set { _initialDepositError = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public CustomerManagementViewModel()
        {
            _apiService = new ApiService();
            Customers = new ObservableCollection<KhachHang>();
            SavingsTypes = new ObservableCollection<LoaiTietKiem>();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                var khachHangTask = _apiService.GetDanhSachKhachHangAsync();
                var soTietKiemTask = _apiService.GetDanhSachSoTietKiemAsync();

                await Task.WhenAll(khachHangTask, soTietKiemTask);

                var data = await khachHangTask;
                var danhSachSo = await soTietKiemTask ?? new List<SoTietKiem>();

                _allCustomers = data ?? new List<KhachHang>();

                foreach (var customer in _allCustomers)
                {
                    customer.TotalBooks = danhSachSo.Count(s => s.KhachHang != null && s.KhachHang.Id == customer.Id);
                }

                ApplyFilter();

                var loaiTkData = await _apiService.GetDanhSachLoaiTietKiemAsync();
                SavingsTypes = new ObservableCollection<LoaiTietKiem>(loaiTkData ?? new List<LoaiTietKiem>());

                if (SavingsTypes.Any() && string.IsNullOrEmpty(SelectedSavingsType))
                {
                    SelectedSavingsType = SavingsTypes.First().MaLoaiTk;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải dữ liệu hệ thống: {ex.Message}";
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Customers = new ObservableCollection<KhachHang>(_allCustomers);
                return;
            }

            var query = SearchText.Trim().ToLower();

            var filteredList = _allCustomers.Where(c =>
                c.Id.ToString().Contains(query) ||
                (!string.IsNullOrEmpty(c.Ten) && c.Ten.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(c.Cmnd) && c.Cmnd.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(c.Sdt) && c.Sdt.ToLower().Contains(query))
            ).ToList();

            Customers = new ObservableCollection<KhachHang>(filteredList);
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
                NewFullNameError = "Vui lòng nhập họ và tên khách hàng.";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(NewCitizenId))
            {
                NewCitizenIdError = "Vui lòng nhập số CCCD.";
                hasError = true;
            }
            else if (!Regex.IsMatch(NewCitizenId.Trim(), @"^\d{12}$"))
            {
                NewCitizenIdError = "Số CCCD không hợp lệ (Phải chứa đúng 12 chữ số).";
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
                    NewPhoneNumberError = "Số điện thoại chỉ được phép chứa các chữ số.";
                    hasError = true;
                }
                else if (cleanPhone.Length < 10 || cleanPhone.Length > 11)
                {
                    NewPhoneNumberError = "Số điện thoại bắt buộc phải từ 10 - 11 chữ số.";
                    hasError = true;
                }
                else if (!cleanPhone.StartsWith("0"))
                {
                    NewPhoneNumberError = "Số điện thoại liên hệ phải bắt đầu bằng chữ số 0.";
                    hasError = true;
                }
            }

            if (hasError) return false;

            var request = new KhachHangRequest
            {
                Ten = NewFullName.Trim(),
                Cmnd = NewCitizenId.Trim(),
                Sdt = NewPhoneNumber.Trim(),
                DiaChi = string.Empty
            };

            bool success = await _apiService.CreateKhachHangAsync(request);
            if (success)
            {
                await LoadDataAsync();
                return true;
            }

            ErrorMessage = "Thêm mới khách hàng thất bại. Vui lòng kiểm tra lại kết nối mạng hoặc trùng lặp CCCD.";
            return false;
        }

        public void PrepareEdit(KhachHang customer)
        {
            _editingCustomer = customer;
            EditFullName = customer.Ten;
            EditCitizenId = customer.Cmnd;
            EditPhoneNumber = customer.Sdt;

            EditFullNameError = string.Empty;
            EditCitizenIdError = string.Empty;
            EditPhoneNumberError = string.Empty;
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmEditAsync()
        {
            if (_editingCustomer == null)
            {
                ErrorMessage = "Không tìm thấy thông tin khách hàng cần chỉnh sửa!";
                return false;
            }

            EditFullNameError = string.Empty;
            EditCitizenIdError = string.Empty;
            EditPhoneNumberError = string.Empty;
            ErrorMessage = string.Empty;

            bool hasError = false;

            if (string.IsNullOrWhiteSpace(EditFullName))
            {
                EditFullNameError = "Họ và tên khách hàng không được bỏ trống.";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(EditCitizenId))
            {
                EditCitizenIdError = "Số định danh CCCD không được bỏ trống.";
                hasError = true;
            }
            else if (!Regex.IsMatch(EditCitizenId.Trim(), @"^\d{12}$"))
            {
                EditCitizenIdError = "Số CCCD không hợp lệ (Phải chứa đúng 12 chữ số).";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(EditPhoneNumber))
            {
                EditPhoneNumberError = "Số điện thoại liên lạc không được bỏ trống.";
                hasError = true;
            }
            else
            {
                string cleanPhone = Regex.Replace(EditPhoneNumber.Trim(), @"[\s\-\(\)]", "");
                if (!Regex.IsMatch(cleanPhone, @"^\d+$"))
                {
                    EditPhoneNumberError = "Số điện thoại chỉ được phép chứa các chữ số.";
                    hasError = true;
                }
                else if (cleanPhone.Length < 10 || cleanPhone.Length > 11)
                {
                    EditPhoneNumberError = "Số điện thoại bắt buộc phải từ 10 - 11 chữ số.";
                    hasError = true;
                }
                else if (!cleanPhone.StartsWith("0"))
                {
                    EditPhoneNumberError = "Số điện thoại liên hệ phải bắt đầu bằng chữ số 0.";
                    hasError = true;
                }
            }

            if (hasError) return false;

            var request = new KhachHangRequest
            {
                Ten = EditFullName.Trim(),
                Cmnd = EditCitizenId.Trim(),
                Sdt = EditPhoneNumber.Trim(),
                DiaChi = _editingCustomer.DiaChi ?? string.Empty
            };

            bool success = await _apiService.UpdateKhachHangAsync(_editingCustomer.Id, request);
            if (success)
            {
                await LoadDataAsync();
                return true;
            }

            ErrorMessage = "Cập nhật thông tin thất bại. Vui lòng kiểm tra trùng lặp CCCD/SĐT.";
            return false;
        }

        public void PrepareAddSavingsBook(KhachHang customer)
        {
            _savingsBookTargetCustomer = customer;
            if (SavingsTypes.Any())
            {
                SelectedSavingsType = SavingsTypes.First().MaLoaiTk;
            }
            InitialDeposit = string.Empty;
            InitialDepositError = string.Empty;
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmAddSavingsBookAsync()
        {
            InitialDepositError = string.Empty;
            ErrorMessage = string.Empty;

            if (_savingsBookTargetCustomer == null)
            {
                ErrorMessage = "Không tìm thấy khách hàng mục tiêu để mở sổ!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(InitialDeposit))
            {
                InitialDepositError = "Vui lòng nhập số tiền gửi ban đầu.";
                return false;
            }

            if (!decimal.TryParse(InitialDeposit.Trim(), out decimal depositAmount) || depositAmount <= 0)
            {
                InitialDepositError = "Số tiền gửi ban đầu phải là số dương lớn hơn 0.";
                return false;
            }

            var request = new MoSoRequest
            {
                TenKhachHang = _savingsBookTargetCustomer.Ten,
                Cmnd = _savingsBookTargetCustomer.Cmnd,
                DiaChi = _savingsBookTargetCustomer.DiaChi ?? string.Empty,
                LoaiTietKiem = SelectedSavingsType,
                SoTienGuiBanDau = depositAmount
            };

            bool success = await _apiService.MoSoTietKiemAsync(request);
            if (success)
            {
                await LoadDataAsync();
                OnSavingsBookAdded?.Invoke();
                return true;
            }

            ErrorMessage = "Mở sổ tiết kiệm thất bại. Vui lòng kiểm tra quy định số tiền gửi tối thiểu.";
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}