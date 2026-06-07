using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using frontend_csharp.Services;

namespace frontend_csharp.Windows
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null) btn.IsEnabled = false;

            try
            {
                string username = txtUsernameSignIn.Text.Trim();
                string password = txtPasswordSignIn.Password;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var response = await _apiService.LoginAsync(username, password);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    AppSession.CurrentToken = response.Token;
                    AppSession.LoggedInUsername = response.Username;

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác hoặc không thể kết nối tới Server!", "Lỗi Đăng Nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                if (btn != null) btn.IsEnabled = true;
            }
        }

        private void GithubBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/IkenUyh/passbook-management",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở liên kết: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowSignUp_Click(object sender, RoutedEventArgs e)
        {
            SignInPanel.IsHitTestVisible = false;
            SignUpPanel.IsHitTestVisible = true;
            OverlaySignUpContent.IsHitTestVisible = false;
            OverlaySignInContent.IsHitTestVisible = true;

            AnimateElement(OverlayTransform, 0);
            AnimateOpacity(OverlaySignUpContent, 0, 0);
            AnimateOpacity(OverlaySignInContent, 1, 1);

            AnimateElement(SignInTransform, 270);
            AnimateOpacity(SignInPanel, 0, 0);

            AnimateElement(SignUpTransform, 270);
            AnimateOpacity(SignUpPanel, 1, 1);

            AnimateGradientColors(Color.FromRgb(63, 81, 181), Color.FromRgb(0, 188, 212));
        }

        private void ShowSignIn_Click(object sender, RoutedEventArgs e)
        {
            SignInPanel.IsHitTestVisible = true;
            SignUpPanel.IsHitTestVisible = false;
            OverlaySignUpContent.IsHitTestVisible = true;
            OverlaySignInContent.IsHitTestVisible = false;

            AnimateElement(OverlayTransform, 400);
            AnimateOpacity(OverlaySignInContent, 0, 0);
            AnimateOpacity(OverlaySignUpContent, 1, 1);

            AnimateElement(SignInTransform, 0);
            AnimateOpacity(SignInPanel, 1, 1);

            AnimateElement(SignUpTransform, 0);
            AnimateOpacity(SignUpPanel, 0, 0);

            AnimateGradientColors(Color.FromRgb(255, 0, 128), Color.FromRgb(255, 140, 0));
        }

        private void AnimateGradientColors(Color color1, Color color2)
        {
            ColorAnimation anim1 = new ColorAnimation
            {
                To = color1,
                Duration = TimeSpan.FromSeconds(0.7),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut }
            };
            ColorAnimation anim2 = new ColorAnimation
            {
                To = color2,
                Duration = TimeSpan.FromSeconds(0.7),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut }
            };

            Stop1.BeginAnimation(GradientStop.ColorProperty, anim1);
            Stop2.BeginAnimation(GradientStop.ColorProperty, anim2);
        }

        private void AnimateElement(TranslateTransform transform, double targetX)
        {
            DoubleAnimation anim = new DoubleAnimation
            {
                To = targetX,
                Duration = TimeSpan.FromSeconds(0.7),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut }
            };
            transform.BeginAnimation(TranslateTransform.XProperty, anim);
        }

        private void AnimateOpacity(UIElement element, double targetOpacity, int zIndex)
        {
            Panel.SetZIndex(element, zIndex);
            DoubleAnimation anim = new DoubleAnimation
            {
                To = targetOpacity,
                Duration = TimeSpan.FromSeconds(0.7),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut }
            };
            element.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void Placeholder_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb?.Parent is Grid grid && grid.Children[0] is TextBlock placeholder)
                placeholder.Visibility = string.IsNullOrEmpty(tb.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void Placeholder_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb?.Parent is Grid grid && grid.Children[0] is TextBlock placeholder)
                placeholder.Visibility = string.IsNullOrEmpty(pb.Password) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}