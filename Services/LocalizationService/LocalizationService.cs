using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SyncLight.Services.LocalizationService
{
    public class LocalizationService : ILocalizationService
    {
        private JObject? _currentResources;
        public string this[string key]
        {
            get
            {
                if (_currentResources?.GetValue(key) is JToken token)
                    return token.ToString();
                return $"[{key}]"; // 未找到时显示 [Key]
            }
        }

        public event Action? LanguageChanged;

        //初始化操作
        public void SetCulture(string cultureName)
        {
            LoadResource(cultureName);
            LanguageChanged?.Invoke(); // 通知 UI 刷新
        }

        private void LoadResource(string cultureName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"WpfIocDemo.Resources.Languages.{cultureName}.json";

            // 方式2：从输出目录读取（如果设为 Content）
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Languages", $"{cultureName}.json");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                //解析Json
                _currentResources = JObject.Parse(json);
                return;
            }

            throw new FileNotFoundException($"Language file not found: {resourceName} or {filePath}");
        }
    }
}
