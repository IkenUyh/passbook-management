using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
        private readonly Dashboard _dashboard;
        private readonly EmployeeManagement _employeeManagement;
        private readonly SavingsBookLookup _savingsBookLookup;
        private readonly CustomerManagement _customerManagement;
        private readonly Regulation _regulation;
        private readonly ReportsManagement _reportsManagement;
        private readonly AuditLogManagement _auditLogManagement;
        private readonly AccountManagement _accountManagement;
        private readonly ApiService _apiService;

        public MainWindow()
        {
            InitializeComponent();

            _apiService = new ApiService();

            _dashboard = new Dashboard();
            _employeeManagement = new EmployeeManagement();
            _savingsBookLookup = new SavingsBookLookup();
            _customerManagement = new CustomerManagement();
            _regulation = new Regulation();
            _reportsManagement = new ReportsManagement();
            _auditLogManagement = new AuditLogManagement();
            _accountManagement = new AccountManagement();

            MenuSidePanel.OnMenuChanged += MenuSidePanel_OnMenuChanged;

            MainContent.Content = _dashboard;

            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Hiển thị Avatar ngay lập tức bằng Role có sẵn từ Session mà không cần đợi API phản hồi
            SetAvatarByRole(AppSession.CurrentRole);

            // 2. Tải các thông tin tên và CCCD bất đồng bộ từ API sau
            await LoadUserProfileAsync();
        }

        private void SetAvatarByRole(string role)
        {
            try
            {
                // Xác định đúng Key của bitmap dựa trên quyền (ADMIN hoặc NHAN_VIEN)
                string avatarKey = (role == "ADMIN") ? "Admin_Avt" : "Employee_Avt";

                // Tìm kiếm mở rộng ở cả Window và cấp độ Application toàn cục để đảm bảo tìm thấy Bitmap
                object resource = TryFindResource(avatarKey) ?? Application.Current.TryFindResource(avatarKey);

                if (resource is ImageSource avatarSource)
                {
                    AvatarImg.Source = avatarSource;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gán hình ảnh Avatar: {ex.Message}");
            }
        }

        private async Task LoadUserProfileAsync()
        {
            try
            {
                var profile = await _apiService.GetCurrentProfileAsync();
                if (profile != null)
                {
                    TxtHoTen.Text = profile.HoTen;
                    TxtCccd.Text = $"CCCD: {profile.Cccd}";

                    // Cập nhật lại một lần nữa nhằm đồng bộ chính xác theo thông tin tài khoản mới nhất
                    SetAvatarByRole(profile.Role);
                }
                else
                {
                    TxtHoTen.Text = "Không rõ danh tính";
                    TxtCccd.Text = "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải thông tin cá nhân: {ex.Message}");
                TxtHoTen.Text = "Lỗi kết nối";
                TxtCccd.Text = "";
            }
        }

        private async void MenuSidePanel_OnMenuChanged(string menuName)
        {
            switch (menuName)
            {
                case "Trang chủ":
                    MainContent.Content = new Dashboard();
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
                    MainContent.Content = new ReportsManagement();
                    break;
                case "Quy định":
                    MainContent.Content = _regulation;
                    break;
                case "Nhật ký":
                    MainContent.Content = _auditLogManagement;
                    break;
                case "Tài khoản":
                    MainContent.Content = _accountManagement;
                    break;
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            ExecuteLogout();
        }

        private void ExecuteLogout()
        {
            AppSession.CurrentToken = null;
            AppSession.LoggedInUsername = null;

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

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