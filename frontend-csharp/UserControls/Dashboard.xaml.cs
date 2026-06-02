using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using frontend_csharp.ViewModels;

namespace frontend_csharp.UserControls
{
    public partial class Dashboard : UserControl
    {
        private readonly DashboardViewModel _viewModel;

        public Dashboard()
        {
            InitializeComponent();

            _viewModel = new DashboardViewModel();
            this.DataContext = _viewModel;

            this.Loaded += Dashboard_Loaded;
            this.Unloaded += Dashboard_Unloaded;
        }

        private async void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.PlayAnimation();

            MainContent.Visibility = Visibility.Visible;
        }

        private void Dashboard_Unloaded(object sender, RoutedEventArgs e)
        {
            MainContent.Visibility = Visibility.Hidden;
        }
    }
}