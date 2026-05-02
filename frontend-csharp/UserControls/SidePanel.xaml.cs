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
