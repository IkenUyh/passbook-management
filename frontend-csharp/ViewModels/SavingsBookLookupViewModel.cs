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
        // Danh sách gốc lưu toàn bộ sổ tiết kiệm lấy từ database/API
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

        // Thuộc tính nhận từ ô TextBox tìm kiếm
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilter(); // Tự động lọc khi người dùng nhập dữ liệu
            }
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

        /// <summary>
        /// Bộ lọc tìm kiếm theo Mã số sổ và Tên khách hàng
        /// </summary>
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

            // Thay thế bất biến tập hợp hiển thị giúp UI cập nhật đồng bộ chính xác cả DataGrid lẫn Cards View
            SavingsBooks = new ObservableCollection<SavingsBookModel>(filteredList);
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