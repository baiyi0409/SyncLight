using System;
using System.IO.Ports;
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
using SyncLight.Common;
using SyncLight.Models;
using SyncLight.ViewModels.Home;
using SyncLight.ViewModels.Main;
using SyncLight.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace SyncLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        public MainWindow(IMainViewModel viewModel,INavigationService navigationService, ISnackbarService snackbarService, IContentDialogService contentDialogService,Adalight adalight)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _adalight = adalight;
            this.DataContext = _viewModel;

            navigationService.SetNavigationControl(MyNavigationView);
            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
            contentDialogService.SetDialogHost(RootContentDialogHost);
        }

        private readonly IMainViewModel _viewModel;
        private readonly Adalight _adalight;

        private void FluentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Navigate((MenuBar)MenuBar.SelectedItem);
        }

        private void FluentWindow_Closed(object sender, EventArgs e)
        {
            _adalight.Disconnect();
            _adalight.Dispose();
        }
    }
}