using System;
using System.Linq;
using System.Windows;

namespace Parmigiano.Services
{
    public static class ThemeService
    {
        public static void SetTheme(string theme)
        {
            string path = $"Themes/{theme}.xaml";

            var app = Application.Current;

            var newDict = new ResourceDictionary
            {
                Source = new Uri(path, UriKind.Relative),
            };

            var oldDict = app.Resources.MergedDictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Theme"));

            if (oldDict != null)
            {
                app.Resources.MergedDictionaries.Remove(oldDict);
            }

            app.Resources.MergedDictionaries.Add(newDict);
        }
    }
}
