using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDIDemo.ViewModels.Main
{
    public interface IMainViewModel
    {
        //定义方法
        void SayHello();

        void ChangeLanguage();
    }
}
