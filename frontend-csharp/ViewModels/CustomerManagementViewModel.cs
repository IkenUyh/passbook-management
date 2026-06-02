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

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public CustomerManagementViewModel()
        {
            Customers = new ObservableCollection<CustomerModel>();
        }

        public async Task LoadDataAsync()
        {
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

        public void ResetForm()
        {
            NewFullName = string.Empty;
            NewCitizenId = string.Empty;
            NewPhoneNumber = string.Empty;
            ErrorMessage = string.Empty;
        }

        public bool ConfirmAdd()
        {
            if (string.IsNullOrWhiteSpace(NewFullName) ||
                string.IsNullOrWhiteSpace(NewCitizenId) ||
                string.IsNullOrWhiteSpace(NewPhoneNumber))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ các trường thông tin!";
                return false;
            }

            ErrorMessage = string.Empty;

            Customers.Add(new CustomerModel
            {
                Id = $"KH-{1000 + Customers.Count + 1}",
                FullName = NewFullName.Trim(),
                CitizenId = NewCitizenId.Trim(),
                PhoneNumber = NewPhoneNumber.Trim(),
                TotalBooks = 0
            });

            return true;
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