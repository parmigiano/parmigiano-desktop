using Parmigiano.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Parmigiano.Services
{
    public static class ToastService
    {
        private static ToastControl? _toast;

        public static void Initialize(ToastControl toast)
        {
            _toast = toast;
        }

        public static void Show(string message)
        {
            if (_toast == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                _toast.Show(message);
            });
        }
    }
}
