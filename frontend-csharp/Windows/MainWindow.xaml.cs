using System;
using System.Threading.Tasks;
using System.Windows;
using frontend_csharp.Services;
using frontend_csharp.Windows;
using frontend_csharp.UserControls;

namespace frontend_csharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 1. Khai báo sẵn các biến chứa UserControl
        private readonly Dashboard _dashboard;
        private readonly EmployeeManagement _employeeManagement;
        private readonly SavingsBookLookup _savingsBookLookup;
        private readonly CustomerManagement _customerManagement;
        private readonly Regulation _regulation;
        private readonly ReportsManagement _reportsManagement;
        private readonly AuditLogManagement _auditLogManagement;

        public MainWindow()
        {
            InitializeComponent();

            // 2. Khởi tạo tất cả UserControl (Load sẵn)
            _dashboard = new Dashboard();
            _employeeManagement = new EmployeeManagement();
            _savingsBookLookup = new SavingsBookLookup();
            _customerManagement = new CustomerManagement();
            _regulation = new Regulation();
            _reportsManagement = new ReportsManagement();
            _auditLogManagement = new AuditLogManagement();

            // Đăng ký nhận sự kiện từ SidePanel
            MenuSidePanel.OnMenuChanged += MenuSidePanel_OnMenuChanged;

            // Mặc định hiển thị Dashboard khi mở app bằng instance đã tạo
            MainContent.Content = _dashboard;
        }

        // Khôi phục lại async void để không block UI thread của SidePanel Animation
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
                case "Nhân viên":
                    MainContent.Content = _employeeManagement;
                    break;
                case "Tra cứu sổ":
                    MainContent.Content = _savingsBookLookup;
                    break;
                case "Khách hàng":
                    MainContent.Content = _customerManagement;
                    break;
                case "Báo cáo":
                    MainContent.Content = _reportsManagement;
                    break;
                case "Quy định":
                    MainContent.Content = _regulation;
                    break;
                case "Nhật ký":
                    MainContent.Content = _auditLogManagement;
                    break;
            }
        }

        // Sự kiện click nút đăng xuất ở góc dưới bên trái
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            ExecuteLogout();
        }

        private void ExecuteLogout()
        {
            // 1. Xóa thông tin phiên đăng nhập hiện tại
            AppSession.CurrentToken = null;
            AppSession.LoggedInUsername = null;

            // 2. Khởi tạo và hiển thị lại màn hình LoginWindow
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            // 3. Đóng màn hình chính hiện tại
            this.Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void ShowPopup(UIElement content)
        {
            GlobalPopupContent.Content = content;
            GlobalPopupOverlay.Visibility = Visibility.Visible;
        }

        public void HidePopup()
        {
            GlobalPopupOverlay.Visibility = Visibility.Collapsed;
            GlobalPopupContent.Content = null;
        }
    }
}