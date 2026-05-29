using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace frontend_csharp.ViewModels
{
    public class SavingsBookLookupViewModel : INotifyPropertyChanged
    {
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

        public SavingsBookLookupViewModel()
        {
            SavingsBooks = new ObservableCollection<SavingsBookModel>();
        }

        public async Task LoadDataAsync()
        {
            // Mô phỏng gọi API lấy dữ liệu ở background thread
            var newData = await Task.Run(() =>
            {
                var list = new List<SavingsBookModel>();
                for (int i = 0; i < 15; i++)
                {
                    list.Add(new SavingsBookModel
                    {
                        Id = $"FIG-12{i}",
                        SavingsType = "3 tháng",
                        CustomerName = "Nguyễn Văn A",
                        Balance = "500.000 VNĐ",
                        MaturityDate = "05/12/2026",
                        InterestRate = "10%",
                        Status = "Hoạt động"
                    });
                }
                return list;
            });

            // Sau khi await, context tự động quay về UI Thread để cập nhật dữ liệu an toàn
            SavingsBooks.Clear();
            foreach (var item in newData)
            {
                SavingsBooks.Add(item);
            }
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