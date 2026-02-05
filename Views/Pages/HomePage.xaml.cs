using Newtonsoft.Json.Linq;
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
        private readonly IHomeViewModel _viewModel;

        public HomePage(IHomeViewModel viewModel)
        {
            InitializeComponent();
            this._viewModel = viewModel;
            this.DataContext = _viewModel;
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

        private void ColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (string.IsNullOrWhiteSpace(textBox?.Text))
                return;

            string input = textBox.Text.Trim();

            // 确保以 # 开头
            if (!input.StartsWith("#"))
                input = "#" + input;

            // 尝试解析为 RGB 或 ARGB 十六进制颜色
            if (TryParseHexColor(input, out byte r, out byte g, out byte b))
            {
                _viewModel.UpdateSolidColor(r, g, b);
            }
        }

        private bool TryParseHexColor(string hex, out byte r, out byte g, out byte b)
        {
            r = g = b = 0;

            if (string.IsNullOrEmpty(hex) || hex[0] != '#')
                return false;

            string colorPart = hex.Substring(1);

            // 支持 #RGB (3位), #RRGGBB (6位), #AARRGGBB (8位)
            try
            {
                if (colorPart.Length == 3)
                {
                    // #RGB → #RRGGBB
                    r = Convert.ToByte(colorPart[0].ToString() + colorPart[0], 16);
                    g = Convert.ToByte(colorPart[1].ToString() + colorPart[1], 16);
                    b = Convert.ToByte(colorPart[2].ToString() + colorPart[2], 16);
                    return true;
                }
                else if (colorPart.Length == 6)
                {
                    r = Convert.ToByte(colorPart.Substring(0, 2), 16);
                    g = Convert.ToByte(colorPart.Substring(2, 2), 16);
                    b = Convert.ToByte(colorPart.Substring(4, 2), 16);
                    return true;
                }
                else if (colorPart.Length == 8)
                {
                    // 忽略 Alpha，取 RRGGBB
                    r = Convert.ToByte(colorPart.Substring(2, 2), 16);
                    g = Convert.ToByte(colorPart.Substring(4, 2), 16);
                    b = Convert.ToByte(colorPart.Substring(6, 2), 16);
                    return true;
                }
            }
            catch
            {
                // 格式错误，比如包含非十六进制字符
            }

            return false;
        }
    }
}
