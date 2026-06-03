using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace frontend_csharp.ViewModels
{
    public class SavingsBookLookupViewModel : INotifyPropertyChanged
    {
        private List<SavingsBookModel> _allSavingsBooks = new List<SavingsBookModel>();

        private ObservableCollection<SavingsBookModel> _savingsBooks;
        public ObservableCollection<SavingsBookModel> SavingsBooks
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

        // State quản lý giao dịch Gửi / Rút tiền
        private SavingsBookModel _transactionTargetBook;

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
            SavingsBooks = new ObservableCollection<SavingsBookModel>();
        }

        public async Task LoadDataAsync()
        {
            var newData = await Task.Run(() =>
            {
                var list = new List<SavingsBookModel>();
                for (int i = 0; i < 15; i++)
                {
                    list.Add(new SavingsBookModel
                    {
                        Id = $"FIG-12{i}",
                        SavingsType = i % 2 == 0 ? "3 tháng" : "Không kỳ hạn",
                        CustomerName = $"Nguyễn Văn A",
                        Balance = "500.000 VNĐ",
                        MaturityDate = "05/12/2026",
                        InterestRate = "10%",
                        Status = "Hoạt động"
                    });
                }
                return list;
            });

            _allSavingsBooks = newData;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SavingsBooks = new ObservableCollection<SavingsBookModel>(_allSavingsBooks);
                return;
            }

            var query = SearchText.Trim().ToLower();

            var filteredList = _allSavingsBooks.Where(b =>
                (!string.IsNullOrEmpty(b.Id) && b.Id.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(b.CustomerName) && b.CustomerName.ToLower().Contains(query))
            ).ToList();

            SavingsBooks = new ObservableCollection<SavingsBookModel>(filteredList);
        }

        public void PrepareTransaction(SavingsBookModel savingsBook)
        {
            _transactionTargetBook = savingsBook ?? throw new ArgumentNullException(nameof(savingsBook), "Sổ tiết kiệm không hợp lệ.");
            TransactionAmount = string.Empty;
            ErrorMessage = string.Empty;
        }

        public bool ConfirmDeposit()
        {
            if (!ValidateTransaction(out decimal amount)) return false;

            int index = _allSavingsBooks.FindIndex(b => b.Id == _transactionTargetBook.Id);
            if (index >= 0)
            {
                var target = _allSavingsBooks[index];

                // Trích xuất số dư hiện tại để tính toán (Ví dụ chuỗi "500.000 VNĐ" -> 500000)
                decimal currentBalance = ParseBalanceString(target.Balance);
                decimal newBalance = currentBalance + amount;

                // Cập nhật bất biến (Immutable update) thay vì biến đổi trực tiếp phần tử cũ
                _allSavingsBooks[index] = new SavingsBookModel
                {
                    Id = target.Id,
                    SavingsType = target.SavingsType,
                    CustomerName = target.CustomerName,
                    Balance = $"{newBalance:N0} VNĐ".Replace(',', '.'),
                    MaturityDate = target.MaturityDate,
                    InterestRate = target.InterestRate,
                    Status = target.Status
                };

                ApplyFilter();
            }
            return true;
        }

        public bool ConfirmWithdraw()
        {
            if (!ValidateTransaction(out decimal amount)) return false;

            int index = _allSavingsBooks.FindIndex(b => b.Id == _transactionTargetBook.Id);
            if (index >= 0)
            {
                var target = _allSavingsBooks[index];
                decimal currentBalance = ParseBalanceString(target.Balance);

                if (amount > currentBalance)
                {
                    ErrorMessage = "Số dư trong sổ không đủ để thực hiện rút tiền!";
                    return false;
                }

                decimal newBalance = currentBalance - amount;

                // Cập nhật bất biến (Immutable update)
                _allSavingsBooks[index] = new SavingsBookModel
                {
                    Id = target.Id,
                    SavingsType = target.SavingsType,
                    CustomerName = target.CustomerName,
                    Balance = $"{newBalance:N0} VNĐ".Replace(',', '.'),
                    MaturityDate = target.MaturityDate,
                    InterestRate = target.InterestRate,
                    Status = newBalance == 0 ? "Đã tất toán" : target.Status
                };

                ApplyFilter();
            }
            return true;
        }

        private bool ValidateTransaction(out decimal amount)
        {
            amount = 0;
            if (_transactionTargetBook == null)
            {
                ErrorMessage = "Không tìm thấy thông tin sổ tiết kiệm!";
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

        private decimal ParseBalanceString(string balanceStr)
        {
            if (string.IsNullOrEmpty(balanceStr)) return 0;
            string cleanString = balanceStr.Replace("VNĐ", "").Replace(".", "").Trim();
            return decimal.TryParse(cleanString, out decimal result) ? result : 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SavingsBookModel
    {
        public string Id { get; set; }
        public string SavingsType { get; set; }
        public string CustomerName { get; set; }
        public string Balance { get; set; }
        public string MaturityDate { get; set; }
        public string InterestRate { get; set; }
        public string Status { get; set; }
    }
}