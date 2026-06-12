using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using frontend_csharp.ViewModels;
using frontend_csharp.Models.SoTietKiemModel;

namespace frontend_csharp.UserControls
{
    public partial class SavingsBookLookup : UserControl
    {
        private readonly SavingsBookLookupViewModel _viewModel;

        public SavingsBookLookup()
        {
            InitializeComponent();

            _viewModel = new SavingsBookLookupViewModel();
            this.DataContext = _viewModel;

            this.Loaded += SavingsBookLookup_Loaded;
        }

        private async void SavingsBookLookup_Loaded(object sender, RoutedEventArgs e)
        {
            ViewToggleListBox.SelectedIndex = 0;

            if (dgvSavingsBooks.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dgvSavingsBooks.ItemsSource);
                if (view != null && view.SortDescriptions.Count > 0)
                {
                    view.SortDescriptions.Clear();
                }

                foreach (var column in dgvSavingsBooks.Columns)
                {
                    column.SortDirection = null;
                }
            }

            if (_viewModel.SavingsBooks.Count == 0)
            {
                await _viewModel.LoadDataAsync();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void DepositMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is SoTietKiem savingsBook)
            {
                _viewModel.PrepareTransaction(savingsBook);

                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    var popupUI = (FrameworkElement)this.FindResource("DepositPopupUI");
                    popupUI.DataContext = _viewModel;
                    mainWindow.ShowPopup(popupUI);
                }
            }
        }

        private void WithdrawMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is SoTietKiem savingsBook)
            {
                _viewModel.PrepareTransaction(savingsBook);

                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    var popupUI = (FrameworkElement)this.FindResource("WithdrawPopupUI");
                    popupUI.DataContext = _viewModel;
                    mainWindow.ShowPopup(popupUI);
                }
            }
        }

        private async void ConfirmDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (await _viewModel.ConfirmDepositAsync())
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.HidePopup();
            }
        }

        private async void ConfirmWithdraw_Click(object sender, RoutedEventArgs e)
        {
            if (await _viewModel.ConfirmWithdrawAsync())
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.HidePopup();
            }
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.HidePopup();
        }
    }
}