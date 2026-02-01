using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLight.Services.ConfigurationLoader
{
    public class ConfigurationLoader : IConfigurationLoader
    {
        public IConfiguration Configuration { get; }

        public ConfigurationLoader()
        {
            var configBuilder = new ConfigurationBuilder();

            // 获取应用程序基目录（exe 所在目录）
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            //添加主文件
            configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            //合并其他Json文件
            var configDir = Path.Combine(baseDir, "Configurations");
            if (Directory.Exists(configDir))
            {
                var jsonFiles = Directory.GetFiles(configDir, "*.json", SearchOption.TopDirectoryOnly)
                                         .OrderBy(f => f); // 按文件名排序（确保确定性）

                foreach (var file in jsonFiles)
                {
                    configBuilder.AddJsonFile(file, optional: false, reloadOnChange: true);
                }
            }

            Configuration = configBuilder.Build();
        }

        public T GetSection<T>(string key) where T : class, new()
        {
            var section = Configuration.GetSection(key);
            if (section.Exists())
            {
                var obj = new T();
                section.Bind(obj);
                return obj;
            }
            return new T(); // 返回默认实例
        }
    }
}
