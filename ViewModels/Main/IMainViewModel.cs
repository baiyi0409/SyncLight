using CommunityToolkit.Mvvm.Input;
using SyncLight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLight.ViewModels.Main
{
    public interface IMainViewModel
    {
        void Navigate(MenuBar item);

        void ChangeLanguage();
    }
}
