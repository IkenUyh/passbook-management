using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

                // Tải song song danh sách khách hàng và sổ tiết kiệm để tối ưu thời gian phản hồi
                var khachHangTask = _apiService.GetDanhSachKhachHangAsync();
                var soTietKiemTask = _apiService.GetDanhSachSoTietKiemAsync();

                await Task.WhenAll(khachHangTask, soTietKiemTask);

                var data = await khachHangTask;
                var danhSachSo = await soTietKiemTask ?? new List<SoTietKiem>();

                _allCustomers = data ?? new List<KhachHang>();

                // Tính toán số sổ sở hữu động dựa vào Id khách hàng
                foreach (var customer in _allCustomers)
                {
                    customer.TotalBooks = danhSachSo.Count(s => s.KhachHang != null && s.KhachHang.Id == customer.Id);
                }

                ApplyFilter();

                // Tải danh mục các loại kỳ hạn tiết kiệm
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

            ErrorMessage = "Thêm mới khách hàng hệ thống thất bại.";
            return false;
        }

        public void PrepareEdit(KhachHang customer)
        {
            _editingCustomer = customer;
            EditFullName = customer.Ten;
            EditCitizenId = customer.Cmnd;
            EditPhoneNumber = customer.Sdt;
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmEditAsync()
        {
            if (_editingCustomer == null)
            {
                ErrorMessage = "Không tìm thấy thông tin khách hàng cần chỉnh sửa!";
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

            ErrorMessage = "Cập nhật thông tin khách hàng thất bại.";
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
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmAddSavingsBookAsync()
        {
            if (_savingsBookTargetCustomer == null)
            {
                ErrorMessage = "Không tìm thấy khách hàng cần thêm sổ!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(InitialDeposit))
            {
                ErrorMessage = "Vui lòng nhập số tiền gửi ban đầu!";
                return false;
            }

            if (!decimal.TryParse(InitialDeposit, out decimal depositAmount) || depositAmount <= 0)
            {
                ErrorMessage = "Số tiền gửi ban đầu không hợp lệ!";
                return false;
            }

            ErrorMessage = string.Empty;

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
                // Kích hoạt sự kiện thông báo cho màn hình tra cứu
                OnSavingsBookAdded?.Invoke();
                return true;
            }

            ErrorMessage = "Mở sổ tiết kiệm thất bại.";
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}