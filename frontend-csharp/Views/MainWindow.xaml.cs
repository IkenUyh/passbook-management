using System;
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

        public MainWindow()
        {
            InitializeComponent();

            // Đăng ký nhận sự kiện từ SidePanel
            MenuSidePanel.OnMenuChanged += MenuSidePanel_OnMenuChanged;
        }

        private void MenuSidePanel_OnMenuChanged(string menuName)
        {
            // Kiểm tra tên menu và đổi View tương ứng
            switch (menuName)
            {
                case "Trang chủ":
                    MainContent.Content = new TrangChu(); 
                    break;
                case "Tra cứu sổ":
                    MainContent.Content = new TraCuuSo();
                    break;
                case "Khách hàng":
                    // MainContent.Content = new KhachHangView();
                    break;
                case "Báo cáo":
                    // MainContent.Content = new BaoCaoView();
                    break;
                case "Quy định":
                    // MainContent.Content = new QuyDinhView();
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