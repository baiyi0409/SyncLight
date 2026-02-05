using CommunityToolkit.Mvvm.ComponentModel;
using SyncLight.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SyncLight.ViewModels.Home
{
    public interface IHomeViewModel
    {
        void UpdateSolidColor(byte r, byte g, byte b);
    }
}
