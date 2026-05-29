using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace frontend_csharp.ViewModels
{
    public class CustomerManagementViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CustomerModel> _customers;
        public ObservableCollection<CustomerModel> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged();
            }
        }

        public CustomerManagementViewModel()
        {
            Customers = new ObservableCollection<CustomerModel>();
        }

        public async Task LoadDataAsync()
        {
            // Mô phỏng gọi API lấy dữ liệu ở background thread
            var newData = await Task.Run(() =>
            {
                var list = new List<CustomerModel>();
                for (int i = 1; i <= 12; i++)
                {
                    list.Add(new CustomerModel
                    {
                        Id = $"KH-{1000 + i}",
                        FullName = $"Nguyễn Văn Thuận {i}",
                        CitizenId = $"07920400{1234 + i}",
                        PhoneNumber = $"090312345{i:D2}",
                        TotalBooks = i % 3 + 1
                    });
                }
                return list;
            });

            Customers.Clear();
            foreach (var customer in newData)
            {
                Customers.Add(customer);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomerModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string CitizenId { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalBooks { get; set; }
    }
}