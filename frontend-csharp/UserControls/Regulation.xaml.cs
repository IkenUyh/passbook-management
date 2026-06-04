using frontend_csharp.Models.QuyDinhModel;
using frontend_csharp.Services;
using System;
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

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ViewModels.RegulationViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
            }
        }

        // 1. LƯU THAY ĐỔI CÁC QUY ĐỊNH CHUNG
        private async void BtnLuuThayDoiThamSo_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ViewModels.RegulationViewModel viewModel)
            {
                if (viewModel.RegulationInfo.TienGuiToiThieu <= 0 ||
                    viewModel.RegulationInfo.TienGuiThemToiThieu <= 0 ||
                    viewModel.RegulationInfo.ThoiGianGuiToiThieuNgay <= 0)
                {
                    MessageBox.Show("Các giá trị quy định (Tiền gửi, Thời gian) phải là số lớn hơn 0!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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

        // 2. LƯU KỲ HẠN MỚI
        private async void SaveAddTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModels.RegulationViewModel;
            if (viewModel == null) return;

            if (string.IsNullOrWhiteSpace(viewModel.KyHanMoi) || string.IsNullOrWhiteSpace(viewModel.LaiSuatMoi))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin Kỳ hạn và Lãi suất!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(viewModel.KyHanMoi.Trim(), out int kyHan) || kyHan < 0)
            {
                MessageBox.Show("Kỳ hạn không hợp lệ! Vui lòng nhập SỐ (nhập 0 cho 'Không kỳ hạn').", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(viewModel.LaiSuatMoi.Trim(), out decimal laiSuat) || laiSuat <= 0)
            {
                MessageBox.Show("Lãi suất không hợp lệ! Vui lòng nhập số lớn hơn 0.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new LoaiTietKiemRequest
            {
                MaLoaiTk = "LTK_" + kyHan,
                TenLoaiTk = kyHan == 0 ? "Không kỳ hạn" : kyHan + " Tháng",
                KyHan = kyHan,
                LaiSuat = laiSuat
            };

            ApiService apiService = new ApiService();
            bool result = await apiService.SaveLoaiTietKiemAsync(request);

            if (result)
            {
                MessageBox.Show("Thêm kỳ hạn mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                await viewModel.LoadDataAsync();

                var mainWindow = (MainWindow)Window.GetWindow(this);
                if (mainWindow != null)
                {
                    mainWindow.HidePopup();
                }

                viewModel.KyHanMoi = "";
                viewModel.LaiSuatMoi = "";
            }
            else
            {
                MessageBox.Show("Thêm kỳ hạn thất bại. Hãy kiểm tra lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

        // 3. LƯU CHỈNH SỬA KỲ HẠN
        private async void SaveEditTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModels.RegulationViewModel;

            if (viewModel != null && viewModel.KyHanDangSua != null)
            {
                string inputKyHan = viewModel.KyHanDangSua.TenKyHan?.Trim();
                int parsedKyHan = -1;

                if (string.Equals(inputKyHan, "không kỳ hạn", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(inputKyHan, "khong ky han", StringComparison.OrdinalIgnoreCase))
                {
                    parsedKyHan = 0;
                }
                else if (int.TryParse(inputKyHan, out int val) && val >= 0)
                {
                    parsedKyHan = val;
                }
                else
                {
                    MessageBox.Show("Kỳ hạn không hợp lệ! Vui lòng nhập số tháng hoặc để chữ 'Không kỳ hạn'.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                viewModel.KyHanDangSua.KyHan = parsedKyHan;
                viewModel.KyHanDangSua.TenKyHan = parsedKyHan == 0 ? "Không kỳ hạn" : parsedKyHan.ToString();

                if (viewModel.KyHanDangSua.LaiSuat <= 0)
                {
                    MessageBox.Show("Lãi suất phải là số lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool result = await viewModel.SaveKyHanAsync();

                if (result)
                {
                    MessageBox.Show("Cập nhật kỳ hạn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await viewModel.LoadDataAsync();

                    var mainWindow = (MainWindow)Window.GetWindow(this);
                    if (mainWindow != null)
                    {
                        mainWindow.HidePopup();
                    }
                }
                else
                {
                    MessageBox.Show("Cập nhật kỳ hạn thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}