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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Parmigiano.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для Spinner.xaml
    /// </summary>
    public partial class Spinner : UserControl
    {
        public Spinner()
        {
            InitializeComponent();

            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Start()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Stop()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
