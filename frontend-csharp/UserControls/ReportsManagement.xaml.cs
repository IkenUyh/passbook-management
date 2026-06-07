using frontend_csharp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace frontend_csharp.UserControls
{
    public partial class ReportsManagement : UserControl
    {
        public ReportsManagement()
        {
            InitializeComponent();
            this.Loaded += ReportsManagement_Loaded;
        }

        private async void ReportsManagement_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.Resources["ChartViewModel"] as ReportsManagementViewModel;

            if (viewModel != null)
            {
                await viewModel.InitializeAsync();
                viewModel.PlayAnimation();
            }
        }
    }
}