using System;
using System.Windows;
using System.Windows.Controls;
using frontend_csharp.ViewModels;

namespace frontend_csharp.UserControls
{
    public partial class AuditLogManagement : UserControl
    {
        private readonly AuditLogViewModel _viewModel;

        public AuditLogManagement()
        {
            InitializeComponent();
            _viewModel = new AuditLogViewModel();
            this.DataContext = _viewModel;

            this.Loaded += AuditLogManagement_Loaded;
        }

        private async void AuditLogManagement_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadDataAsync();
        }
    }
}