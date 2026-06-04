using frontend_csharp.Models.QuyDinhModel;
using frontend_csharp.Services;
using System.Windows;
using System.Windows.Controls;

namespace frontend_csharp.UserControls
{
    public partial class Regulation : UserControl
    {
        public Regulation()
        {
            InitializeComponent();
        }

        // Load dữ liệu khi tab Regulation hiển thị lên
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ViewModels.RegulationViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
            }
        }

        // Lưu thay đổi các Quy định chung
        private async void BtnLuuThayDoiThamSo_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ViewModels.RegulationViewModel viewModel)
            {
                bool result = await viewModel.SaveThamSoAsync();
                if (result)
                {
                    MessageBox.Show("Cập nhật quy định chung thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Cập nhật quy định thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Add Term Popup Handlers
        private void OpenAddTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                var popupUI = (FrameworkElement)this.FindResource("AddTermPopupUI");
                popupUI.DataContext = this.DataContext;
                mainWindow.ShowPopup(popupUI);
            }
        }

        private void CloseAddTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.HidePopup();
            }
        }

        // Lưu kỳ hạn MỚI khi bấm nút Thêm trên AddTermPopupUI
        private async void SaveAddTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                var popupUI = (FrameworkElement)this.FindResource("AddTermPopupUI");

                // Lấy các textbox trong popup để đọc dữ liệu
                var txtKyHan = popupUI.FindName("txtKyHan") as TextBox;
                var txtLaiSuat = popupUI.FindName("txtLaiSuat") as TextBox;
                // var txtGhiChu = popupUI.FindName("txtGhiChu") as TextBox;

                int kyHan = int.TryParse(txtKyHan?.Text, out int kh) ? kh : 0;
                decimal laiSuat = decimal.TryParse(txtLaiSuat?.Text, out decimal ls) ? ls : 0;

                // Chuẩn bị request
                var request = new LoaiTietKiemRequest
                {
                    MaLoaiTk = "LTK_" + kyHan,
                    TenLoaiTk = kyHan + " Tháng",
                    KyHan = kyHan,
                    LaiSuat = laiSuat
                };

                ApiService apiService = new ApiService();
                bool result = await apiService.SaveLoaiTietKiemAsync(request);

                if (result)
                {
                    MessageBox.Show("Thêm kỳ hạn mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reload lại dữ liệu
                    if (this.DataContext is ViewModels.RegulationViewModel viewModel)
                    {
                        await viewModel.LoadDataAsync();
                    }

                    mainWindow.HidePopup();

                    // Reset textbox
                    if (txtKyHan != null) txtKyHan.Text = "";
                    if (txtLaiSuat != null) txtLaiSuat.Text = "";
                }
                else
                {
                    MessageBox.Show("Thêm kỳ hạn thất bại. Hãy kiểm tra lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Edit Term Popup Handlers
        private void OpenEditTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var kyHan = button.DataContext as ViewModels.KyHanModel;
            var viewModel = this.DataContext as ViewModels.RegulationViewModel;

            if (kyHan != null && viewModel != null)
            {
                viewModel.KyHanDangSua = kyHan;

                var mainWindow = (MainWindow)Window.GetWindow(this);
                if (mainWindow != null)
                {
                    var popupUI = (FrameworkElement)this.FindResource("EditTermPopupUI");
                    popupUI.DataContext = viewModel;
                    mainWindow.ShowPopup(popupUI);
                }
            }
        }

        private void CloseEditTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.HidePopup();
            }
        }

        // Lưu kỳ hạn CŨ đang chỉnh sửa
        private async void SaveEditTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModels.RegulationViewModel;

            if (viewModel != null)
            {
                bool result = await viewModel.SaveKyHanAsync();

                if (result)
                {
                    MessageBox.Show("Cập nhật kỳ hạn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await viewModel.LoadDataAsync(); // Cập nhật lại Grid UI
                }
                else
                {
                    MessageBox.Show("Cập nhật kỳ hạn thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.HidePopup();
            }
        }
    }
}