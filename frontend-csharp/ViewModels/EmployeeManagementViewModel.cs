using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace frontend_csharp.ViewModels
{
    public class EmployeeManagementViewModel : INotifyPropertyChanged
    {
        private List<EmployeeModel> _allEmployees = new List<EmployeeModel>();

        private ObservableCollection<EmployeeModel> _employees;
        public ObservableCollection<EmployeeModel> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
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

        // Add Properties
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
        private EmployeeModel _editingEmployee;

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

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public EmployeeManagementViewModel()
        {
            Employees = new ObservableCollection<EmployeeModel>();
        }

        public async Task LoadDataAsync()
        {
            var newData = await Task.Run(() =>
            {
                var list = new List<EmployeeModel>();
                for (int i = 1; i <= 10; i++)
                {
                    list.Add(new EmployeeModel
                    {
                        Id = $"NV-{1000 + i}",
                        FullName = $"Nguyễn Văn Nhân Viên {i}",
                        CitizenId = $"07920400{5678 + i}",
                        PhoneNumber = $"091412345{i:D2}"
                    });
                }
                return list;
            });

            _allEmployees = newData;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Employees = new ObservableCollection<EmployeeModel>(_allEmployees);
                return;
            }

            var query = SearchText.Trim().ToLower();

            var filteredList = _allEmployees.Where(e =>
                (!string.IsNullOrEmpty(e.Id) && e.Id.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(e.FullName) && e.FullName.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(e.CitizenId) && e.CitizenId.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(e.PhoneNumber) && e.PhoneNumber.ToLower().Contains(query))
            ).ToList();

            Employees = new ObservableCollection<EmployeeModel>(filteredList);
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

            var newEmployee = new EmployeeModel
            {
                Id = $"NV-{1000 + _allEmployees.Count + 1}",
                FullName = NewFullName.Trim(),
                CitizenId = NewCitizenId.Trim(),
                PhoneNumber = NewPhoneNumber.Trim()
            };

            _allEmployees.Add(newEmployee);
            ApplyFilter();

            return true;
        }

        public void PrepareEdit(EmployeeModel employee)
        {
            _editingEmployee = employee;
            EditFullName = employee.FullName;
            EditCitizenId = employee.CitizenId;
            EditPhoneNumber = employee.PhoneNumber;
            ErrorMessage = string.Empty;
        }

        public bool ConfirmEdit()
        {
            if (_editingEmployee == null)
            {
                ErrorMessage = "Không tìm thấy thông tin nhân viên cần chỉnh sửa!";
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

            int index = _allEmployees.FindIndex(e => e.Id == _editingEmployee.Id);
            if (index >= 0)
            {
                // Cập nhật bất biến (Immutable update) thay vì sửa trực tiếp phần tử cũ
                _allEmployees[index] = new EmployeeModel
                {
                    Id = _editingEmployee.Id,
                    FullName = EditFullName.Trim(),
                    CitizenId = EditCitizenId.Trim(),
                    PhoneNumber = EditPhoneNumber.Trim()
                };
                ApplyFilter();
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class EmployeeModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string CitizenId { get; set; }
        public string PhoneNumber { get; set; }
    }
}