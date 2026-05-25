using frontend_csharp.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace frontend_csharp.UserControls
{
    public partial class SavingsBookLookup : UserControl
    {
        private SavingsBookLookupViewModel _viewModel;

        public SavingsBookLookup()
        {
            InitializeComponent();
            _viewModel = new SavingsBookLookupViewModel();
            this.DataContext = _viewModel;

            this.Loaded += async (s, e) => await _viewModel.InitializeAsync();
        }
    }
}