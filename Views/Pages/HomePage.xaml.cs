using SyncLight.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Abstractions.Controls;

namespace SyncLight.Views.Pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : System.Windows.Controls.UserControl
    {
        public HomePage(IHomeViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void EdgeButton_Click(object sender, RoutedEventArgs e)
        {
            EdgeFlyout.IsOpen = !EdgeFlyout.IsOpen;
        }

        private void DirectionButton_Click(object sender, RoutedEventArgs e)
        {
            DirectionFlyout.IsOpen = !DirectionFlyout.IsOpen;
        }

        private void AmountButton_Click(object sender, RoutedEventArgs e)
        {
            AmountFlyout.IsOpen = !AmountFlyout.IsOpen;
        }

        private void ScreenButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenFlyout.IsOpen = !ScreenFlyout.IsOpen;
        }
    }
}
