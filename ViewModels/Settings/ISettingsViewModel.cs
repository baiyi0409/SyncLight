using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Appearance;

namespace SyncLight.ViewModels.Settings
{
    public interface ISettingsViewModel
    {
        ApplicationTheme CurrentApplicationTheme { get; set; }
    }
}
