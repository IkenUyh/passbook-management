using frontend_csharp.ViewModels;
using System.Threading.Tasks;
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
            if (_isLoaded)
            {
                var viewModel = this.Resources["ChartViewModel"] as ReportsManagementViewModel;
                if (viewModel != null)
                {
                    viewModel.PlayAnimation();
                }
                return;
            }

            _isLoaded = true;
        }
    }
}