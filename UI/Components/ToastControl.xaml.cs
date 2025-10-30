using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Parmigiano.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для ToastControl.xaml
    /// </summary>
    public partial class ToastControl : UserControl
    {
        public ToastControl()
        {
            InitializeComponent();
        }

        public void Show(string message, int durationMs = 3000)
        {
            MessageText.Text = message;
            this.Visibility = Visibility.Visible;

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            var slideIn = new DoubleAnimation(-30, 0, TimeSpan.FromMilliseconds(300));

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromMilliseconds(durationMs)
            };
            var slideOut = new DoubleAnimation(0, -30, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromMilliseconds(durationMs)
            };

            var sb = new Storyboard();
            sb.Children.Add(fadeIn);
            sb.Children.Add(slideIn);
            sb.Children.Add(fadeOut);
            sb.Children.Add(slideOut);

            Storyboard.SetTarget(fadeIn, ToastBorder);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
            Storyboard.SetTarget(fadeOut, ToastBorder);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath("Opacity"));

            Storyboard.SetTarget(slideIn, TranslateTransform);
            Storyboard.SetTargetProperty(slideIn, new PropertyPath("Y"));
            Storyboard.SetTarget(slideOut, TranslateTransform);
            Storyboard.SetTargetProperty(slideOut, new PropertyPath("Y"));

            sb.Completed += (s, e) => this.Visibility = Visibility.Collapsed;
            sb.Begin();
        }
    }
}
