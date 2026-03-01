using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using SyncLight.ViewModels.Main;
using SyncLight.ViewModels;
using SyncLight.Services.LocalizationService;
using System.ComponentModel;
using SyncLight.Services.ConfigurationLoader;
using SyncLight.Options;
using Microsoft.Extensions.Options;
using System.Net.Security;
using Velopack;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using SyncLight.ViewModels.Home;
using SyncLight.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using SyncLight.Common;
using Microsoft.Extensions.Configuration;
using SyncLight.ViewModels.Settings;
using SyncLight.Services.SyncSnackbarService;

namespace SyncLight
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        //提供给外部使用
        public static IServiceProvider? Services { get; private set; }

        //重写启动方法
        protected override void OnStartup(StartupEventArgs e)
        {
            //Velopack自动更新
            //逻辑为，将生成好的应用，进行打包
            //生成命令为 dotnet publish SyncLight.csproj -c Release --self-contained -r win-x64 -o .\publish   //根目录中多了publish
            //打包命令为 vpk pack --packId SyncLight --packVersion 1.0.0 --packDir .\publish --mainExe SyncLight.exe //根目录中多了Releases
            //其中 packDir .\publish 应根据实际发布的地址进行修改
            //vpk pack --packId SyncLight --packVersion 1.0.0 --packDir bin\Release\net6.0-windows\publish\win-x64 --mainExe SyncLight.exe
            //vpk pack --packId SyncLight --packVersion 1.0.0 --packDir bin\Release\net6.0-windows\publish\win-x64
            VelopackApp.Build().Run();

            //创建服务集合
            var services = new ServiceCollection();

            //对集合进行配置操作
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            //设置默认语言
            var localizationOption = Services.GetRequiredService<IOptions<LocalizationOption>>();
            var loc = Services.GetService<ILocalizationService>();
            var defaultCulture = localizationOption.Value.DefaultCulture; 
            loc?.SetCulture(defaultCulture);

            //获取主窗口并启动
            var mainWindow = Services.GetRequiredService<MainWindow>();
            this.MainWindow = mainWindow;
            mainWindow.Show();

            #region  自动更新模块

            //_ = Task.Run(async () =>
            //{
            //    var mgr = new UpdateManager("http://localhost:4555/updates");
            //    var newVersion = await mgr.CheckForUpdatesAsync();
            //    if (newVersion == null) return;

            //    //mgr.CurrentVersion

            //    var result = MessageBox.Show(
            //        "发现新版本，是否立即更新？",
            //        "更新提示",
            //        MessageBoxButton.YesNo);

            //    if (result != MessageBoxResult.Yes) return;

            //    //其中有action<int> progress
            //    await mgr.DownloadUpdatesAsync(newVersion);
            //    mgr.ApplyUpdatesAndRestart(newVersion);
            //});

            #endregion
        }

        private void ConfigureServices(IServiceCollection services) 
        {
            //注册服务
            var loader = new ConfigurationLoader();
            services.AddSingleton<IConfigurationLoader>(sp => loader);
            services.Configure<LocalizationOption>(
                loader.Configuration.GetSection("Localization")
            );

            services.AddSingleton<INavigationViewPageProvider, DiNavigationViewPageProvider>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SyncSnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            services.AddSingleton<ScreenEdgeColor>();

            //AdaLight
            services.AddSingleton<Adalight>(sp => 
            {
                //var config = sp.GetRequiredService<IConfiguration>();
                //string port = config["Adalight:Port"] ?? "COM3";
                //int ledCount = config.GetValue<int>("Adalight:LedCount", 60);

                var adalight = new Adalight("COM3", 180);
                return adalight;
            });

            //本地化语言服务
            services.AddSingleton<ILocalizationService, LocalizationService>();

            //注册视图模型
            services.AddTransient<IMainViewModel, MainViewModel>();
            services.AddTransient<IHomeViewModel, HomeViewModel>();
            services.AddTransient<ISettingsViewModel, SettingsViewModel>();

            //注册视图
            services.AddSingleton<MainWindow>();
            services.AddTransient<HomePage>();
            services.AddTransient<SettingsPage>();
        }

        //退出释放
        protected override void OnExit(ExitEventArgs e)
        {
            if (Services is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
