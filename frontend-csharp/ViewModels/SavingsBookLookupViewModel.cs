using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace frontend_csharp.ViewModels
{
    public class SavingsBookLookupViewModel : INotifyPropertyChanged
    {
        // 1. ObservableCollection để binding trực tiếp lên UI
        public ObservableCollection<SavingsBookModel> SavingsBooks { get; } = new ObservableCollection<SavingsBookModel>();

        // Property để binding với SelectedIndex của ViewToggleListBox
        private int _viewSelectedIndex;
        public int ViewSelectedIndex
        {
            get => _viewSelectedIndex;
            set
            {
                _viewSelectedIndex = value;
                OnPropertyChanged();
            }
        }

        // Phương thức gọi khi View được Loaded
        public async Task InitializeAsync()
        {
            // Reset về dạng danh sách (Index 0)
            ViewSelectedIndex = 0;

            // Chỉ load lại data nếu danh sách đang trống
            if (SavingsBooks.Count == 0)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            // 1. Giả lập gọi API
            var newDataFromApi = await Task.Run(() =>
            {
                var data = new List<SavingsBookModel>();
                for (int i = 0; i < 15; i++)
                {
                    data.Add(new SavingsBookModel
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
                return data;
            });

            SavingsBooks.Clear();

            // 2. Bơm dữ liệu vào UI Thread ở chế độ nền
            foreach (var item in newDataFromApi)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    SavingsBooks.Add(item);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        // Implement INotifyPropertyChanged
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