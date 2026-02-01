using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using SyncLight.Services.LocalizationService;

namespace SyncLight.Extensions
{
    public class TranslateExtension : MarkupExtension
    {
        private readonly string _key;

        public TranslateExtension(string key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var localization = App.Services?.GetService<ILocalizationService>();
            if (localization == null)
                return $"[Missing: {_key}]";

            var source = new TranslationBindingSource(localization, _key);
            var binding = new System.Windows.Data.Binding(nameof(TranslationBindingSource.Value))
            {
                Source = source,
                Mode = BindingMode.OneWay  // 或 OneWay，但 OneTime 更高效（因为值不会变，除非语言切换）
            };

            return binding.ProvideValue(serviceProvider);
        }

        internal class TranslationBindingSource : INotifyPropertyChanged
        {
            private readonly ILocalizationService _localization;
            private readonly string _key;

            public TranslationBindingSource(ILocalizationService localization, string key)
            {
                _localization = localization;
                _key = key;
                _localization.LanguageChanged += OnLanguageChanged;
            }

            public string Value => _localization[_key];

            public event PropertyChangedEventHandler? PropertyChanged;

            private void OnLanguageChanged()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }

            // 让 WPF 自动取 .Value 属性
            public static object GetBindableValue(object target) => ((TranslationBindingSource)target).Value;
        }
    }
}
