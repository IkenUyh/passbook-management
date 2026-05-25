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
    }
}