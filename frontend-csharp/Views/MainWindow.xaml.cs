using System;
using System.Windows;
using frontend_csharp.Services; // Nhớ using cái này để dùng ApiService

namespace frontend_csharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;

        public MainWindow()
        {
            InitializeComponent();
            _apiService = new ApiService(); // Khởi tạo service
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // UI State: Chuyển sang trạng thái đang tải
                btnLoad.IsEnabled = false;
                txtStatus.Text = "Đang kết nối đến Backend Spring Boot...";
                dgSoTietKiem.ItemsSource = null;

                // Gọi API thực tế
                var danhSachSo = await _apiService.GetDanhSachSoTietKiemAsync();

                // UI State: Thành công
                dgSoTietKiem.ItemsSource = danhSachSo;
                txtStatus.Text = $"Đã tải {danhSachSo.Count} sổ tiết kiệm lúc {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                // UI State: Lỗi
                txtStatus.Text = "Lỗi kết nối API!";
                MessageBox.Show($"Không thể lấy dữ liệu.\nChi tiết lỗi: {ex.Message}",
                                "Lỗi Server", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Mở lại nút bấm
                btnLoad.IsEnabled = true;
            }
        }
    }
}