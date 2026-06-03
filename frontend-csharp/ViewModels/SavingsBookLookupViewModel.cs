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
        }

        public async Task LoadDataAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                List<SoTietKiem> apiData = await _apiService.GetDanhSachSoTietKiemAsync();

                if (apiData == null)
                {
                    throw new InvalidOperationException("Không thể kết nối đến máy chủ hoặc dữ liệu trả về trống.");
                }

                _allSavingsBooks = apiData.Select(s => new SoTietKiem
                {
                    Id = s.Id,
                    KhachHang = s.KhachHang,
                    LoaiTietKiem = s.LoaiTietKiem,
                    SoDu = s.SoDu,
                    NgayMo = s.NgayMo,
                    NgayDaoHan = s.NgayDaoHan,
                    TrangThai = s.TrangThai,
                    Version = s.Version
                }).ToList();

                ApplyFilter();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi lấy danh sách sổ: {ex.Message}";
                Console.WriteLine($"[ERROR] LoadDataAsync sổ tiết kiệm thất bại: {ex}");
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
                (b.KhachHang != null && !string.IsNullOrEmpty(b.KhachHang.Ten) && b.KhachHang.Ten.ToLower().Contains(query)) ||
                (b.KhachHang != null && !string.IsNullOrEmpty(b.KhachHang.Cmnd) && b.KhachHang.Cmnd.ToLower().Contains(query))
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
                NgayGui = DateTime.Now
            };

            bool success = await _apiService.GuiTienAsync(request);
            if (!success)
            {
                ErrorMessage = "Giao dịch gửi tiền không thành công. Hãy kiểm tra lại quy định gửi thêm.";
                return false;
            }

            await LoadDataAsync();
            return true;
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
            if (!success)
            {
                ErrorMessage = "Giao dịch rút tiền thất bại. Hãy kiểm tra lại thời gian gửi tối thiểu.";
                return false;
            }

            await LoadDataAsync();
            return true;
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
                ErrorMessage = "Vui lòng cung cấp số tiền thực hiện giao dịch!";
                return false;
            }

            if (!decimal.TryParse(TransactionAmount, out amount) || amount <= 0)
            {
                ErrorMessage = "Giá trị số tiền nhập vào không hợp lệ!";
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