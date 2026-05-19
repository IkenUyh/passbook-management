using System;
using System.Threading.Tasks;
using System.Windows;
using frontend_csharp.Services; // Nhớ using cái này để dùng ApiService
using frontend_csharp.UserControls; // Đảm bảo khai báo đúng namespace chứa các Views

namespace frontend_csharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 1. Khai báo sẵn các biến chứa UserControl
        private readonly Dashboard _dashboard;
        private readonly SavingsBookLookup _savingsBookLookup;
        // private readonly KhachHangView _khachHangView;
        // private readonly BaoCaoView _baoCaoView;
        // private readonly QuyDinhView _quyDinhView;

        public MainWindow()
        {
            InitializeComponent();

            // 2. Khởi tạo tất cả UserControl (Load sẵn)
            _dashboard = new Dashboard();
            _savingsBookLookup = new SavingsBookLookup();
            // _khachHangView = new KhachHangView();
            // _baoCaoView = new BaoCaoView();
            // _quyDinhView = new QuyDinhView();

            // Đăng ký nhận sự kiện từ SidePanel
            MenuSidePanel.OnMenuChanged += MenuSidePanel_OnMenuChanged;

            // Mặc định hiển thị Dashboard khi mở app bằng instance đã tạo
            MainContent.Content = _dashboard;
        }

        // Đổi thành async void để có thể dùng await
        private async void MenuSidePanel_OnMenuChanged(string menuName)
        {
            // Nhường UI thread 300ms để animation của SidePanel chạy mượt mà không bị block
            // await Task.Delay(300);

            // 3. Gọi lại các instance đã khởi tạo sẵn thay vì dùng "new"
            switch (menuName)
            {
                case "Trang chủ":
                    MainContent.Content = _dashboard;
                    break;
                case "Tra cứu sổ":
                    MainContent.Content = _savingsBookLookup;
                    break;
                case "Khách hàng":
                    // MainContent.Content = _khachHangView;
                    break;
                case "Báo cáo":
                    // MainContent.Content = _baoCaoView;
                    break;
                case "Quy định":
                    // MainContent.Content = _quyDinhView;
                    break;
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            // Hoặc this.Close(); nếu bạn chỉ muốn đóng cửa sổ này
        }
    }
}