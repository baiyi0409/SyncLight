using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using WpfDIDemo.ViewModels.Main;
using WpfDIDemo.ViewModels;
using WpfDIDemo.Services.LocalizationService;
using System.ComponentModel;
using WpfDIDemo.Services.ConfigurationLoader;
using WpfDIDemo.Options;
using Microsoft.Extensions.Options;
using WpfDIDemo.Views;
using System.Net.Security;
using WpfDIDemo.ViewModels.AutoUpdate;
using WpfDIDemo.Services.AutoUpdateService;

namespace WpfDIDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //提供给外部使用
        public static IServiceProvider? Services { get; private set; }

        //重写启动方法
        protected override void OnStartup(StartupEventArgs e)
        {
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

            //自动更新
            var autoUpdater = Services.GetRequiredService<AutoUpdateService>();
            autoUpdater.Start("http://localhost:4555/AutoUpdate.json");
        }

        private void ConfigureServices(IServiceCollection services) 
        {
            //注册服务
            var loader = new ConfigurationLoader();
            services.AddSingleton<IConfigurationLoader>(sp => loader);
            services.Configure<LocalizationOption>(
                loader.Configuration.GetSection("Localization")
            );

            //自动更新服务
            services.AddSingleton<AutoUpdateService>();

            //本地化语言服务
            services.AddSingleton<ILocalizationService, LocalizationService>();

            //注册视图模型
            services.AddSingleton<IMainViewModel, MainViewModel>();
            services.AddSingleton<IAutoUpdateViewModel, AutoUpdateViewModel>();

            //注册视图
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AutoUpdateView>();
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
