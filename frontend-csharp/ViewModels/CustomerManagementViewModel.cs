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

namespace frontend_csharp.ViewModels
{
    public class CustomerManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private List<KhachHang> _allCustomers = new List<KhachHang>();

        private ObservableCollection<KhachHang> _customers;
        public ObservableCollection<KhachHang> Customers
        {
            get => _customers;
            set { _customers = value; OnPropertyChanged(); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilter(); }
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

        public ObservableCollection<string> SavingsTypes { get; } = new ObservableCollection<string> { "KKH", "3T", "6T" };

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
        }

        public async Task LoadDataAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                var apiData = await _apiService.GetDanhSachKhachHangAsync();

                if (apiData == null) throw new InvalidOperationException("Lỗi kết nối máy chủ.");

                _allCustomers = apiData.ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi: {ex.Message}";
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
            var filtered = _allCustomers.Where(c =>
                c.Id.ToString().Contains(query) ||
                (!string.IsNullOrEmpty(c.Ten) && c.Ten.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(c.Cmnd) && c.Cmnd.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(c.Sdt) && c.Sdt.ToLower().Contains(query))
            ).ToList();

            Customers = new ObservableCollection<KhachHang>(filtered);
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
            if (string.IsNullOrWhiteSpace(NewFullName) || string.IsNullOrWhiteSpace(NewCitizenId) || string.IsNullOrWhiteSpace(NewPhoneNumber))
            {
                ErrorMessage = "Vui lòng nhập đủ thông tin!";
                return false;
            }

            var request = new KhachHangRequest
            {
                Ten = NewFullName.Trim(),
                Cmnd = NewCitizenId.Trim(),
                Sdt = NewPhoneNumber.Trim(),
                DiaChi = ""
            };

            if (!await _apiService.CreateKhachHangAsync(request))
            {
                ErrorMessage = "Thêm mới thất bại.";
                return false;
            }

            await LoadDataAsync();
            return true;
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
            if (_editingCustomer == null || string.IsNullOrWhiteSpace(EditFullName))
            {
                ErrorMessage = "Thông tin không hợp lệ!";
                return false;
            }

            var request = new KhachHangRequest
            {
                Ten = EditFullName.Trim(),
                Cmnd = EditCitizenId.Trim(),
                Sdt = EditPhoneNumber.Trim(),
                DiaChi = _editingCustomer.DiaChi
            };

            if (!await _apiService.UpdateKhachHangAsync(_editingCustomer.Id, request))
            {
                ErrorMessage = "Cập nhật thất bại.";
                return false;
            }

            await LoadDataAsync();
            return true;
        }

        public void PrepareAddSavingsBook(KhachHang customer)
        {
            _savingsBookTargetCustomer = customer;
            SelectedSavingsType = "KKH";
            InitialDeposit = string.Empty;
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmAddSavingsBookAsync()
        {
            if (_savingsBookTargetCustomer == null || !decimal.TryParse(InitialDeposit, out decimal amount) || amount <= 0)
            {
                ErrorMessage = "Số tiền không hợp lệ!";
                return false;
            }

            var request = new MoSoRequest
            {
                TenKhachHang = _savingsBookTargetCustomer.Ten,
                Cmnd = _savingsBookTargetCustomer.Cmnd,
                DiaChi = _savingsBookTargetCustomer.DiaChi ?? "",
                LoaiTietKiem = SelectedSavingsType,
                SoTienGuiBanDau = amount
            };

            if (!await _apiService.MoSoTietKiemAsync(request))
            {
                ErrorMessage = "Mở sổ thất bại.";
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}