using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Velopack;
using SyncLight.Options;
using SyncLight.Services.LocalizationService;
using SyncLight.ViewModels.Main;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using SyncLight.Models;
using Wpf.Ui;
using SyncLight.Views.Pages;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Navigation;

namespace SyncLight.ViewModels
{
    public partial class MainViewModel : ObservableObject , IMainViewModel
    {
        private readonly ILocalizationService _localizationService;
        private readonly IOptionsMonitor<LocalizationOption> _localizationOption;
        private readonly INavigationService _navigationService;

        public MainViewModel(ILocalizationService localizationService, IOptionsMonitor<LocalizationOption> localizationOption, INavigationService navigationService)
        {
            _localizationService = localizationService;
            _localizationOption = localizationOption;
            _navigationService = navigationService;
            InitMenuBars();
        }

        [ObservableProperty]
        public ObservableCollection<MenuBar> menuBars;

        [ObservableProperty]
        private MenuBar selectedItem;

        [RelayCommand]
        public void InitMenuBars() 
        {
            MenuBars = new ObservableCollection<MenuBar>()
            {
                new MenuBar() { Id=1, Icon="AppsListDetail24", DisplayName="首页",ViewName=typeof(HomePage), Tip="" },
                new MenuBar() { Id=1, Icon="Settings28", DisplayName="设置",ViewName=typeof(SettingsPage), Tip="" }
            };
            SelectedItem = MenuBars.First();
        }

        [RelayCommand]
        public void Navigate(MenuBar item) 
        {
            if (item == null || item.ViewName == null)
                return;
            _navigationService.Navigate(item.ViewName);
        }

        [RelayCommand]
        public void ChangeLanguage()
        {
            var culture = _localizationOption.CurrentValue.DefaultCulture;
            string next = culture == "zh-CN" ? "en-US" : "zh-CN";
            _localizationService.SetCulture(next);
            UpdateDefaultCulture(next);
        }

        /// <summary>
        /// 待优化
        /// </summary>
        /// <param name="newCulture"></param>
        public void UpdateDefaultCulture(string newCulture)
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations", "Localization.json");

            string json = File.ReadAllText(configPath);
            var root = JObject.Parse(json);

            // 安全地设置嵌套值
            if (root["Localization"] == null)
                root["Localization"] = new JObject();

            root["Localization"]["DefaultCulture"] = newCulture;

            File.WriteAllText(configPath, root.ToString(Formatting.Indented));
        }
    }
}
