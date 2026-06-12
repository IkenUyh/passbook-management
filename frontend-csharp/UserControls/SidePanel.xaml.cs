using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace frontend_csharp.UserControls
{
    /// <summary>
    /// Interaction logic for SidePanel.xaml
    /// </summary>
    public partial class SidePanel : UserControl
    {
        public event Action<string> OnMenuChanged;

        public SidePanel()
        {
            InitializeComponent();
            ApplyRolePermissions();
        }

        /// <summary>
        /// Phân quyền hiển thị Menu dựa trên Role của tài khoản hiện tại
        /// </summary>
        private void ApplyRolePermissions()
        {
            string role = AppSession.CurrentRole;

            if (string.IsNullOrEmpty(role))
            {
                // Bảo mật mặc định: Nếu chưa đăng nhập hoặc không rõ quyền, ẩn toàn bộ menu nhạy cảm
                SetAllMenusVisibility(Visibility.Collapsed);
                MenuTrangChu.Visibility = Visibility.Visible;
                return;
            }

            // Đưa tất cả về trạng thái hiển thị mặc định trước khi lọc
            SetAllMenusVisibility(Visibility.Visible);

            if (role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                // ADMIN: Được bấm vào hết, trừ Tài khoản
                MenuTaiKhoan.Visibility = Visibility.Collapsed;
            }
            else if (role.Equals("NHAN_VIEN", StringComparison.OrdinalIgnoreCase))
            {
                // NHAN_VIEN: Chỉ có trang chủ, tra cứu sổ, khách hàng, báo cáo, quy định, tài khoản
                // -> Ẩn chức năng "Nhân viên" và "Nhật ký"
                MenuNhanVien.Visibility = Visibility.Collapsed;
                MenuNhatKy.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Trường hợp Role lạ không hợp lệ -> Fail loudly để kiểm tra hệ thống
                throw new InvalidOperationException($"Hệ thống phát hiện vai trò không hợp lệ: '{role}'");
            }
        }

        /// <summary>
        /// Hàm bổ trợ cấu hình nhanh trạng thái hiển thị cho toàn bộ menu chính
        /// </summary>
        private void SetAllMenusVisibility(Visibility visibility)
        {
            MenuTrangChu.Visibility = visibility;
            MenuNhanVien.Visibility = visibility;
            MenuTraCuuSo.Visibility = visibility;
            MenuKhachHang.Visibility = visibility;
            MenuBaoCao.Visibility = visibility;
            MenuQuyDinh.Visibility = visibility;
            MenuNhatKy.Visibility = visibility;
            MenuTaiKhoan.Visibility = visibility;
        }

        private void Menu_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true)
            {
                // Bắn sự kiện kèm theo nội dung (Content) của nút
                OnMenuChanged?.Invoke(rb.Content.ToString());
            }
        }
    }
}