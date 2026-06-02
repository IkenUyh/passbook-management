using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace frontend_csharp.ViewModels
{
    public class CustomerManagementViewModel : INotifyPropertyChanged
    {
        // Danh sách gốc lưu trữ toàn bộ dữ liệu từ CSDL/API
        private List<CustomerModel> _allCustomers = new List<CustomerModel>();

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

        // Thuộc tính phục vụ ô tìm kiếm
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilter(); // Tự động lọc mỗi khi người dùng gõ chữ
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

        // Edit Properties
        private CustomerModel _editingCustomer;

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

        // Add Savings Book Properties
        private CustomerModel _savingsBookTargetCustomer;

        public ObservableCollection<string> SavingsTypes { get; } = new ObservableCollection<string>
        {
            "3 tháng",
            "6 tháng",
            "Không kỳ hạn"
        };

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

            _allCustomers = newData;
            ApplyFilter();
        }

        /// <summary>
        /// Logic lọc dữ liệu đa năng: Mã, Tên, CCCD, Số điện thoại
        /// </summary>
        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Customers = new ObservableCollection<CustomerModel>(_allCustomers);
                return;
            }

            var query = SearchText.Trim().ToLower();

            var filteredList = _allCustomers.Where(c =>
                (!string.IsNullOrEmpty(c.Id) && c.Id.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(c.FullName) && c.FullName.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(c.CitizenId) && c.CitizenId.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.ToLower().Contains(query))
            ).ToList();

            // Cập nhật bất biến tập hợp hiển thị để WPF Grid/Cards đồng bộ chính xác
            Customers = new ObservableCollection<CustomerModel>(filteredList);
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

            var newCustomer = new CustomerModel
            {
                Id = $"KH-{1000 + _allCustomers.Count + 1}",
                FullName = NewFullName.Trim(),
                CitizenId = NewCitizenId.Trim(),
                PhoneNumber = NewPhoneNumber.Trim(),
                TotalBooks = 0
            };

            _allCustomers.Add(newCustomer);
            ApplyFilter(); // Làm mới danh sách hiển thị sau khi thêm

            return true;
        }

        public void PrepareEdit(CustomerModel customer)
        {
            _editingCustomer = customer;
            EditFullName = customer.FullName;
            EditCitizenId = customer.CitizenId;
            EditPhoneNumber = customer.PhoneNumber;
            ErrorMessage = string.Empty;
        }

        public bool ConfirmEdit()
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

            int index = _allCustomers.FindIndex(c => c.Id == _editingCustomer.Id);
            if (index >= 0)
            {
                _allCustomers[index] = new CustomerModel
                {
                    Id = _editingCustomer.Id,
                    FullName = EditFullName.Trim(),
                    CitizenId = EditCitizenId.Trim(),
                    PhoneNumber = EditPhoneNumber.Trim(),
                    TotalBooks = _editingCustomer.TotalBooks
                };
                ApplyFilter(); // Làm mới danh sách hiển thị sau khi sửa
            }

            return true;
        }

        public void PrepareAddSavingsBook(CustomerModel customer)
        {
            _savingsBookTargetCustomer = customer;
            SelectedSavingsType = "3 tháng";
            InitialDeposit = string.Empty;
            ErrorMessage = string.Empty;
        }

        public bool ConfirmAddSavingsBook()
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

            int index = _allCustomers.FindIndex(c => c.Id == _savingsBookTargetCustomer.Id);
            if (index >= 0)
            {
                _allCustomers[index] = new CustomerModel
                {
                    Id = _savingsBookTargetCustomer.Id,
                    FullName = _savingsBookTargetCustomer.FullName,
                    CitizenId = _savingsBookTargetCustomer.CitizenId,
                    PhoneNumber = _savingsBookTargetCustomer.PhoneNumber,
                    TotalBooks = _savingsBookTargetCustomer.TotalBooks + 1
                };
                ApplyFilter(); // Làm mới danh sách hiển thị sau khi tăng số sổ
            }

            /* * TODO: Kết nối API gọi xuống Cơ sở dữ liệu bảng LOAITIETKIEM & SỐ TIẾT KIỆM tại đây.
             * Ví dụ: 
             * var response = await _apiService.CreateSavingsBookAsync(new { 
             * CustomerId = _savingsBookTargetCustomer.Id, 
             * Type = SelectedSavingsType, 
             * Amount = depositAmount 
             * });
             */

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