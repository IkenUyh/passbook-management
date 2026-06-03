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
        // Add Term
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

        // Edit term
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

        private void SaveEditTermPopup_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModels.RegulationViewModel;

            if (viewModel != null)
            {
                System.Windows.Data.CollectionViewSource.GetDefaultView(viewModel.DanhSachKyHan).Refresh();
            }

            var mainWindow = (MainWindow)Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.HidePopup();
            }
        }
    }
}