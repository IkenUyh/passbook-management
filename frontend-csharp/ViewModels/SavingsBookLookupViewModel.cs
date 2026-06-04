using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using frontend_csharp.Services;
using frontend_csharp.Models.SoTietKiemModel;
using frontend_csharp.Models.GiaoDichModel;

namespace frontend_csharp.ViewModels
{
    public class SavingsBookLookupViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private List<SoTietKiem> _allSavingsBooks = new List<SoTietKiem>();

        private ObservableCollection<SoTietKiem> _savingsBooks;
        public ObservableCollection<SoTietKiem> SavingsBooks
        {
            get => _savingsBooks;
            set
            {
                _savingsBooks = value;
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

        private SoTietKiem _transactionTargetBook;

        private string _transactionAmount;
        public string TransactionAmount
        {
            get => _transactionAmount;
            set { _transactionAmount = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public SavingsBookLookupViewModel()
        {
            _apiService = new ApiService();
            SavingsBooks = new ObservableCollection<SoTietKiem>();
            // Đăng ký lắng nghe sự kiện từ màn hình quản lý khách hàng
            CustomerManagementViewModel.OnSavingsBookAdded += RefreshData;
        }

        private void RefreshData()
        {
            // Sửa tên hàm gọi cho đúng với hàm thực tế bên dưới
            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                var data = await _apiService.GetDanhSachSoTietKiemAsync();
                _allSavingsBooks = data ?? new List<SoTietKiem>();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi lấy danh sách sổ tiết kiệm: {ex.Message}";
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SavingsBooks = new ObservableCollection<SoTietKiem>(_allSavingsBooks);
                return;
            }

            var query = SearchText.Trim().ToLower();

            var filteredList = _allSavingsBooks.Where(b =>
                (!string.IsNullOrEmpty(b.Id) && b.Id.ToLower().Contains(query)) ||
                (b.KhachHang != null && !string.IsNullOrEmpty(b.KhachHang.Ten) && b.KhachHang.Ten.ToLower().Contains(query))
            ).ToList();

            SavingsBooks = new ObservableCollection<SoTietKiem>(filteredList);
        }

        public void PrepareTransaction(SoTietKiem savingsBook)
        {
            _transactionTargetBook = savingsBook ?? throw new ArgumentNullException(nameof(savingsBook), "Sổ tiết kiệm không hợp lệ.");
            TransactionAmount = string.Empty;
            ErrorMessage = string.Empty;
        }

        public async Task<bool> ConfirmDepositAsync()
        {
            if (!ValidateTransaction(out decimal amount)) return false;

            var request = new GuiTienRequest
            {
                MaSoTietKiem = _transactionTargetBook.Id,
                SoTienGui = amount,
                NgayGui = DateTime.Now.ToString("yyyy-MM-dd")
            };

            bool success = await _apiService.GuiTienAsync(request);
            if (success)
            {
                await LoadDataAsync();
                return true;
            }

            ErrorMessage = "Giao dịch gửi thêm tiền thất bại.";
            return false;
        }

        public async Task<bool> ConfirmWithdrawAsync()
        {
            if (!ValidateTransaction(out decimal amount)) return false;

            if (amount > _transactionTargetBook.SoDu)
            {
                ErrorMessage = "Số dư trong sổ không đủ để thực hiện giao dịch rút tiền!";
                return false;
            }

            var request = new RutTienRequest
            {
                MaSoTietKiem = _transactionTargetBook.Id,
                SoTienRut = amount,
                NgayRut = DateTime.Now
            };

            bool success = await _apiService.RutTienAsync(request);
            if (success)
            {
                await LoadDataAsync();
                return true;
            }

            ErrorMessage = "Giao dịch rút tiền/Tất toán thất bại.";
            return false;
        }

        private bool ValidateTransaction(out decimal amount)
        {
            amount = 0;
            if (_transactionTargetBook == null)
            {
                ErrorMessage = "Không tìm thấy thông tin sổ tiết kiệm mục tiêu!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TransactionAmount))
            {
                ErrorMessage = "Vui lòng nhập số tiền thực hiện!";
                return false;
            }

            if (!decimal.TryParse(TransactionAmount, out amount) || amount <= 0)
            {
                ErrorMessage = "Số tiền nhập vào không hợp lệ!";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}