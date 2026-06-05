using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using frontend_csharp.Models.AuditLogModel;
using frontend_csharp.Services;

namespace frontend_csharp.ViewModels
{
    public class AuditLogViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<AuditLog> _auditLogs;
        private string _errorMessage;

        public ObservableCollection<AuditLog> AuditLogs
        {
            get => _auditLogs;
            set { _auditLogs = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public AuditLogViewModel()
        {
            _apiService = new ApiService();
            AuditLogs = new ObservableCollection<AuditLog>();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                var data = await _apiService.GetDanhSachAuditLogAsync();
                AuditLogs = new ObservableCollection<AuditLog>(data ?? new List<AuditLog>());
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải nhật ký hệ thống: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}