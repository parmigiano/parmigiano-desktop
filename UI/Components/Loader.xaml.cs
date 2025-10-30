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
    /// Логика взаимодействия для Loader.xaml
    /// </summary>
    public partial class Loader : UserControl
    {
        private readonly Storyboard _rotateStoryboard;

        public Loader()
        {
            InitializeComponent();

            var rotateAnimation = new DoubleAnimation(0, 360, new System.Windows.Duration(TimeSpan.FromSeconds(1)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            _rotateStoryboard = new Storyboard();
            _rotateStoryboard.Children.Add(rotateAnimation);
            Storyboard.SetTarget(rotateAnimation, RotateTransform);
            Storyboard.SetTargetProperty(rotateAnimation, new System.Windows.PropertyPath("Angle"));

            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Start()
        {
            this.Visibility = System.Windows.Visibility.Visible;
            _rotateStoryboard.Begin(this, true); // важно: передаем контрол, чтобы анимация работала
        }

        public void Stop()
        {
            _rotateStoryboard.Stop(this);
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
