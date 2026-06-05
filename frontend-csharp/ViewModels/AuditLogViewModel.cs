using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

                // Cập nhật bất biến (Immutable) bằng cách chiếu dữ liệu sang các object AuditLog mới
                var processedData = data?.Select(log => new AuditLog
                {
                    Id = log.Id,
                    NguoiThucHien = log.NguoiThucHien,
                    ChiTiet = log.ChiTiet,
                    ThoiGian = log.ThoiGian,
                    HanhDong = StandardizeHanhDong(log.HanhDong)
                }).ToList() ?? new List<AuditLog>();

                AuditLogs = new ObservableCollection<AuditLog>(processedData);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải nhật ký hệ thống: {ex.Message}";
            }
        }

        /// <summary>
        /// Chuẩn hóa chuỗi: Chuyển chữ thường, thay '_' thành ' ', viết hoa chữ cái đầu
        /// </summary>
        private string StandardizeHanhDong(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string lowerWithSpaces = input.Replace('_', ' ').ToLower();
            return char.ToUpper(lowerWithSpaces[0]) + lowerWithSpaces.Substring(1);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}