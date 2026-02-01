using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions;

namespace SyncLight.Common
{
    public class DiNavigationViewPageProvider : INavigationViewPageProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public DiNavigationViewPageProvider(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public object? GetPage(Type pageType)
        {
            return _serviceProvider.GetService(pageType);
        }
    }
}
