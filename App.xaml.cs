using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using WpfDIDemo.ViewModels.Main;
using WpfDIDemo.ViewModels;

namespace WpfDIDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        //重写启动方法
        protected override void OnStartup(StartupEventArgs e)
        {
            //创建服务集合
            var serviceCollection = new ServiceCollection();
            //对集合进行配置操作
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            //获取主窗口并启动
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services) 
        {
            //注册服务
            //注册视图模型
            services.AddSingleton<IMainViewModel, MainViewModel>();
            //注册视图
            services.AddSingleton<MainWindow>();
        }

        //退出释放
        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
