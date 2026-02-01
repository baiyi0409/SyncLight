using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Appearance;

namespace SyncLight.ViewModels.Settings
{
    public partial class SettingsViewModel: ObservableObject,ISettingsViewModel
    {
        public SettingsViewModel()
        {
            CurrentApplicationTheme = ApplicationThemeManager.GetAppTheme();
            ApplicationThemeManager.Changed += OnThemeChanged;
        }

        [ObservableProperty]
        private ApplicationTheme _currentApplicationTheme = ApplicationTheme.Unknown;

        partial void OnCurrentApplicationThemeChanged(ApplicationTheme oldValue, ApplicationTheme newValue)
        {
            ApplicationThemeManager.Apply(newValue);
        }

        private void OnThemeChanged(ApplicationTheme currentApplicationTheme, System.Windows.Media.Color systemAccent)
        {
            // Update the theme if it has been changed elsewhere than in the settings.
            if (CurrentApplicationTheme != currentApplicationTheme)
            {
                CurrentApplicationTheme = currentApplicationTheme;
            }
        }
    }
}
