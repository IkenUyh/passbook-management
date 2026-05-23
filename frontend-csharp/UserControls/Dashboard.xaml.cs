using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using frontend_csharp.ViewModels;

namespace frontend_csharp.UserControls
{
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();

            this.Loaded += Dashboard_Loaded;
            this.Unloaded += Dashboard_Unloaded;
        }

        private async void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new DashboardViewModel();
            await Task.Delay(50);
            MainContent.Visibility = Visibility.Visible;
        }

        private void Dashboard_Unloaded(object sender, RoutedEventArgs e)
        {
            MainContent.Visibility = Visibility.Hidden;
            this.DataContext = null;
        }
    }
}