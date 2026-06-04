using frontend_csharp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace frontend_csharp.UserControls
{
    public partial class ReportsManagement : UserControl
    {
        private bool _isLoaded = false;

        public ReportsManagement()
        {
            InitializeComponent();
            this.Loaded += ReportsManagement_Loaded;
        }

        private async void ReportsManagement_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.Resources["ChartViewModel"] as ReportsManagementViewModel;

            if (_isLoaded)
            {
                if (viewModel != null) viewModel.PlayAnimation();
                return;
            }

            _isLoaded = true;

            if (viewModel != null)
            {
                await viewModel.InitializeAsync();
                viewModel.PlayAnimation();
            }
        }
    }
}