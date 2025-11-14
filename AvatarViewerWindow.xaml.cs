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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для AvatarViewerWindow.xaml
    /// </summary>
    public partial class AvatarViewerWindow : Window
    {
        private double _scale = 1.0;
        private const double ScaleStep = 0.1;

        public AvatarViewerWindow(ImageSource source)
        {
            InitializeComponent();

            ImageControl.Source = source;
            ImageControl.RenderTransform = new ScaleTransform(_scale, _scale);
            ImageControl.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            this.PreviewMouseWheel += AvatarViewerWindow_PreviewMouseWheel;
        }

        private void AvatarViewerWindow_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (e.Delta > 0)
                {
                    _scale += ScaleStep;
                }
                else
                {
                    _scale = Math.Max(0.1, _scale - ScaleStep);
                }

                var st = ImageControl.RenderTransform as ScaleTransform;
                if (st == null)
                {
                    st = new ScaleTransform(_scale, _scale);
                    ImageControl.RenderTransform = st;
                }
                else
                {
                    st.ScaleX = _scale;
                    st.ScaleY = _scale;
                }

                e.Handled = true;
            }
            catch { }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
