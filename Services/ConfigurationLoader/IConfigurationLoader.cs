using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLight.Services.ConfigurationLoader
{
    public interface IConfigurationLoader
    {
        IConfiguration Configuration { get; }

        T GetSection<T>(string key) where T : class, new();
    }
}
