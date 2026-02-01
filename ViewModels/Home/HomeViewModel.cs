using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SyncLight.Common;
using SyncLight.Enums;
using SyncLight.Models;
using SyncLight.Views.Pages;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace SyncLight.ViewModels.Home
{
    public partial class HomeViewModel : ObservableObject, IHomeViewModel
    {
        private readonly ISnackbarService _snackbarService;
        private readonly Adalight _adalight;
        public HomeViewModel(ISnackbarService snackbarService,Adalight adalight)
        {
            _snackbarService = snackbarService;
            _adalight = adalight;
            Dev_IsOpen = _adalight.Connected;
            Dev_Port = _adalight.Port;
            Dev_BaudRate = _adalight.Speed;

            InitAllScreens();
            InitLightEffects();
            InitLightEffectColors();
        }

        #region 设备设置

        [ObservableProperty]
        private string dev_Port;

        [ObservableProperty]
        private int dev_BaudRate;

        [ObservableProperty]
        private bool dev_IsOpen;

        [RelayCommand]
        private void SwitchAdaLight() 
        {
            if (_adalight.Connected)
            {
                _adalight.Disconnect();
                _adalight.Dispose();
                SelectedItem = null;
            }
            else 
            {
                _adalight.Connect();
                //SendSolidColor(255, 255, 255);
                //_adalight.UpdatePixel(0, Color.White, true);
            }

            Dev_IsOpen = _adalight.Connected;

            //通知
            if (Dev_IsOpen)
            {
                _snackbarService.Show(
                    title: "设备连接",
                    message: "连接成功",
                    appearance: ControlAppearance.Secondary,
                    icon: null,
                    timeout: TimeSpan.FromSeconds(3));
            }
            else 
            {
                _snackbarService.Show(
                    title: "设备连接",
                    message: "连接断开",
                    appearance: ControlAppearance.Secondary,
                    icon: null,
                    timeout: TimeSpan.FromSeconds(3));
            }
        }

        #endregion

        private void SendSolidColor(byte r, byte g, byte b)
        {
            if (_adalight?.Connected != true) return;

            var colors = new List<System.Drawing.Color>();
            for (int i = 0; i < 180; i++)
            {
                colors.Add(System.Drawing.Color.FromArgb(r, g, b));
            }

            _adalight.UpdateColors(colors, update: true);
        }

        #region 追光设置

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedEdgeDisplayText))]
        private EdgeEnum _selectedEdge = EdgeEnum.ThreeSides;

        public string SelectedEdgeDisplayText => SelectedEdge switch
        {
            EdgeEnum.ThreeSides => "三边",
            EdgeEnum.FourSides => "四边",
            _ => ""
        };

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedDirectionDisplayText))]
        private DirectionEnum _selectedDirection = DirectionEnum.LeftToRight;

        public string SelectedDirectionDisplayText => SelectedDirection switch
        {
            DirectionEnum.LeftToRight => "从左到右",
            DirectionEnum.RightToLeft => "从右到左",
            _ => "",
        };

        [ObservableProperty]
        private int _lightAmount;

        [ObservableProperty]
        private ObservableCollection<ScreenItem> _allScreens;

        [ObservableProperty]
        private ScreenItem _selectedScreen;

        private void InitAllScreens()
        {
            var screens = Screen.AllScreens
                .Select((screen, index) => new ScreenItem
                {
                    Index = index + 1,
                    DeviceName = screen.DeviceName,
                    DisplayName = $"屏幕 {index + 1}",
                    Bounds = screen.Bounds,
                    WorkingArea = screen.WorkingArea,
                    IsPrimary = screen.Primary,
                    Resolution = $"{screen.Bounds.Width}x{screen.Bounds.Height}"
                })
                .ToList();

            AllScreens = new ObservableCollection<ScreenItem>(screens);

            var primary = screens.FirstOrDefault(s => s.IsPrimary) ?? screens.FirstOrDefault();
            _selectedScreen = primary;
        }

        #endregion

        #region 灯效设置

        [ObservableProperty]
        private ObservableCollection<LightEffect> lightEffects;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedTabIndex))]
        private LightEffect? _selectedItem;

        private void InitLightEffects() 
        {
            LightEffects = new ObservableCollection<LightEffect>() 
            {
                new LightEffect(){ Id=0, Icon="ScreenSearch24", Name="屏幕追光",Type=LightEffectsEnum.SyncMode },
                new LightEffect(){ Id=1, Icon="MicPulse24", Name="拾音律动",Type=LightEffectsEnum.MicMode },
                new LightEffect(){ Id=2, Icon="CloudEdit24", Name="彩虹跳跃",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=3, Icon="Beach24", Name="海浪潮汐",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=4, Icon="Flash24", Name="电闪雷鸣",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=5, Icon="HeartPulse24", Name="呼吸心跳",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=6, Icon="Fire24", Name="火焰燃烧",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=7, Icon="BuildingLightHouse20", Name="南极光彩",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=8, Icon="Blur24", Name="粒子碰撞",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=9, Icon="TreeEvergreen20", Name="圣诞节日",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=10, Icon="ArrowSync24", Name="变换莫测",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=11, Icon="Color24", Name="纯色灯光",Type=LightEffectsEnum.ColorMode },
                new LightEffect(){ Id=12, Icon="WeatherSunny24", Name="浓情夏日",Type=LightEffectsEnum.EmbeddedMode },
                new LightEffect(){ Id=13, Icon="SoundSource24", Name="北极冰山",Type=LightEffectsEnum.EmbeddedMode },
            };
        }

        [RelayCommand]
        private void GetEmbeddedLightEffect(LightEffect item)
        {

            if (item == null || item?.Id == null)
                return;

            if (!_adalight.Connected) 
            {
                SelectedItem = null;
                return;
            }

            //需要分情况
            //四种情况 屏幕追光、麦克风、纯色、内置灯效
            LightEffect_BorderIsVisible = Visibility.Hidden;
            switch (item.Type) 
            {
                case LightEffectsEnum.EmbeddedMode:
                    _adalight.GetEmbeddedMode(item.Id);
                    break;
                case LightEffectsEnum.ColorMode:
                    LightEffect_BorderIsVisible = Visibility.Visible;
                    break;
            }
        }

        [ObservableProperty]
        private Visibility lightEffect_BorderIsVisible = Visibility.Hidden;

        public int SelectedTabIndex => SelectedItem?.Type switch 
        {
            LightEffectsEnum.SyncMode => 0,
            LightEffectsEnum.MicMode => 1,
            LightEffectsEnum.ColorMode => 2,
            _ => 0
        };

        [ObservableProperty]
        private ObservableCollection<Color> _lightEffectColors;

        private void InitLightEffectColors() 
        {
            var converter = new ColorConverter();

            LightEffectColors = new ObservableCollection<Color>()
            {
                (Color)converter.ConvertFromString("#FF0000"),
                (Color)converter.ConvertFromString("#FFF700"),
                (Color)converter.ConvertFromString("#00FF00"),
                (Color)converter.ConvertFromString("#00d3ff"),
                (Color)converter.ConvertFromString("#0000ff"),
                (Color)converter.ConvertFromString("#ce00ff"),
                (Color)converter.ConvertFromString("#ff5a00"),
                (Color)converter.ConvertFromString("#ffffff")
            };
        }

        #endregion
    }
}
