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
                viewModel.GeneralErrorMessage = string.Empty;
                viewModel.TienGuiToiThieuError = string.Empty;
                viewModel.TienGuiThemToiThieuError = string.Empty;
                viewModel.ThoiGianGuiToiThieuNgayError = string.Empty;

                bool hasError = false;

                if (viewModel.RegulationInfo.TienGuiToiThieu <= 0)
                {
                    viewModel.TienGuiToiThieuError = "Vui lòng nhập số tiền lớn hơn 0";
                    hasError = true;
                }

                if (viewModel.RegulationInfo.TienGuiThemToiThieu <= 0)
                {
                    viewModel.TienGuiThemToiThieuError = "Vui lòng nhập số tiền lớn hơn 0";
                    hasError = true;
                }

                if (viewModel.RegulationInfo.ThoiGianGuiToiThieuNgay < 0)
                {
                    viewModel.ThoiGianGuiToiThieuNgayError = "Thời gian gửi không được là số âm";
                    hasError = true;
                }

                if (hasError)
                {
                    return;
                }

                bool result = await viewModel.SaveThamSoAsync();
                if (result)
                {
                    ShowMessagePopup("Cập nhật quy định chung thành công!", true);
                }
                else
                {
                    ShowMessagePopup("Cập nhật quy định thất bại!", false);
                }
            }
        }

        private void OpenAddTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                var popupUI = (FrameworkElement)this.FindResource("AddTermPopupUI");
                if (this.DataContext is ViewModels.RegulationViewModel viewModel)
                {
                    viewModel.KyHanMoiError = string.Empty;
                    viewModel.LaiSuatMoiError = string.Empty;
                    viewModel.KyHanMoi = "";
                    viewModel.LaiSuatMoi = "";
                }
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
            
            viewModel.KyHanMoiError = string.Empty;
            viewModel.LaiSuatMoiError = string.Empty;

            bool hasError = false;
            int kyHan = 0;
            decimal laiSuat = 0m;

            if (string.IsNullOrWhiteSpace(viewModel.KyHanMoi) || !int.TryParse(viewModel.KyHanMoi.Trim(), out kyHan) || kyHan < 0)
            {
                viewModel.KyHanMoiError = "Vui lòng nhập số >= 0 (nhập 0 cho 'Không kỳ hạn').";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(viewModel.LaiSuatMoi) || !decimal.TryParse(viewModel.LaiSuatMoi.Trim(), out laiSuat) || laiSuat <= 0)
            {
                viewModel.LaiSuatMoiError = "Vui lòng nhập số lớn hơn 0.";
                hasError = true;
            }

            if (hasError) return;

            if (viewModel.DanhSachKyHan != null && viewModel.DanhSachKyHan.Any(k => k.KyHan == kyHan))
            {
                viewModel.KyHanMoiError = "Kỳ hạn này đã tồn tại!";
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
                await viewModel.LoadDataAsync();

                var mainWindow = (MainWindow)Window.GetWindow(this);
                if (mainWindow != null)
                {
                    mainWindow.HidePopup();
                }

                viewModel.KyHanMoi = "";
                viewModel.LaiSuatMoi = "";
                
                ShowMessagePopup("Thêm kỳ hạn mới thành công!", true);
            }
            else
            {
                var mainWindow = (MainWindow)Window.GetWindow(this);
                if (mainWindow != null)
                {
                    mainWindow.HidePopup();
                }
                ShowMessagePopup("Thêm kỳ hạn thất bại. Hãy kiểm tra lại!", false);
            }
        }

        private void OpenEditTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var kyHan = button.DataContext as ViewModels.KyHanModel;
            var viewModel = this.DataContext as ViewModels.RegulationViewModel;

            if (kyHan != null && viewModel != null)
            {
                viewModel.KyHanDangSuaError = string.Empty;
                viewModel.LaiSuatDangSuaError = string.Empty;
                viewModel.KyHanDangSua = kyHan;
                viewModel.EditKyHan = kyHan.TenKyHan;
                viewModel.EditLaiSuat = kyHan.LaiSuat.ToString();

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
                viewModel.KyHanDangSuaError = string.Empty;
                viewModel.LaiSuatDangSuaError = string.Empty;

                bool hasError = false;

                string inputKyHan = viewModel.EditKyHan?.Trim();
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
                    viewModel.KyHanDangSuaError = "Vui lòng nhập số >= 0 hoặc 'Không kỳ hạn'.";
                    hasError = true;
                }

                double laiSuat = 0;
                if (string.IsNullOrWhiteSpace(viewModel.EditLaiSuat) || !double.TryParse(viewModel.EditLaiSuat.Trim(), out laiSuat) || laiSuat <= 0)
                {
                    viewModel.LaiSuatDangSuaError = "Vui lòng nhập số lớn hơn 0.";
                    hasError = true;
                }

                if (hasError) return;

                if (viewModel.DanhSachKyHan != null && viewModel.DanhSachKyHan.Any(k => k.KyHan == parsedKyHan && k.MaLoaiTk != viewModel.KyHanDangSua.MaLoaiTk))
                {
                    viewModel.KyHanDangSuaError = "Kỳ hạn này đã tồn tại!";
                    return;
                }

                viewModel.KyHanDangSua.KyHan = parsedKyHan;
                viewModel.KyHanDangSua.TenKyHan = parsedKyHan == 0 ? "Không kỳ hạn" : parsedKyHan.ToString();
                viewModel.KyHanDangSua.LaiSuat = laiSuat;

                bool result = await viewModel.SaveKyHanAsync();

                if (result)
                {
                    await viewModel.LoadDataAsync();

                    var mainWindow = (MainWindow)Window.GetWindow(this);
                    if (mainWindow != null)
                    {
                        mainWindow.HidePopup();
                    }
                    ShowMessagePopup("Cập nhật kỳ hạn thành công!", true);
                }
                else
                {
                    var mainWindow = (MainWindow)Window.GetWindow(this);
                    if (mainWindow != null)
                    {
                        mainWindow.HidePopup();
                    }
                    ShowMessagePopup("Cập nhật kỳ hạn thất bại!", false);
                }
            }
        }

        private void ShowMessagePopup(string message, bool isSuccess)
        {
            var viewModel = this.DataContext as ViewModels.RegulationViewModel;
            if (viewModel != null)
            {
                viewModel.PopupMessage = message;
            }

            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                string popupName = isSuccess ? "SuccessPopupUI" : "ErrorPopupUI";
                var popupUI = (FrameworkElement)this.FindResource(popupName);
                popupUI.DataContext = this.DataContext;
                mainWindow.ShowPopup(popupUI);
            }
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.HidePopup();
            }
        }
    }
}