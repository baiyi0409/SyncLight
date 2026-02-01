using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace SyncLight.Services.SyncSnackbarService
{
    public class SyncSnackbarService : ISnackbarService
    {
        private SnackbarPresenter? _presenter;

        private Snackbar? _snackbar;

        /// <inheritdoc />
        public TimeSpan DefaultTimeOut { get; set; } = TimeSpan.FromSeconds(5);

        /// <inheritdoc />
        public void SetSnackbarPresenter(SnackbarPresenter contentPresenter)
        {
            _presenter = contentPresenter;
        }

        /// <inheritdoc />
        public SnackbarPresenter? GetSnackbarPresenter()
        {
            return _presenter;
        }

        /// <inheritdoc />
        public void Show(
            string title,
            string message,
            ControlAppearance appearance,
            IconElement? icon,
            TimeSpan timeout
        )
        {
            if (_presenter is null)
            {
                throw new InvalidOperationException($"The SnackbarPresenter was never set");
            }

            _snackbar ??= new Snackbar(_presenter);

            //修改Snackbar的样式
            _snackbar.Style = (Style)App.Current.FindResource("SyncSnackbarStyle");

            _snackbar.SetCurrentValue(Snackbar.TitleProperty, title);
            _snackbar.SetCurrentValue(System.Windows.Controls.ContentControl.ContentProperty, message);
            _snackbar.SetCurrentValue(Snackbar.AppearanceProperty, appearance);
            _snackbar.SetCurrentValue(Snackbar.IconProperty, icon);
            _snackbar.SetCurrentValue(
                Snackbar.TimeoutProperty,
                timeout.TotalSeconds == 0 ? DefaultTimeOut : timeout
            );

            _snackbar.Show(true);
        }
    }
}
