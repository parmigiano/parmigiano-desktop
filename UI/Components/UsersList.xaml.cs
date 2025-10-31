using Parmigiano.Models;
using Parmigiano.Utilities;
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
    /// Логика взаимодействия для UsersList.xaml
    /// </summary>
    public partial class UsersList : UserControl
    {
        private readonly MouseUtilities _mouseUtilities = new();

        public event Action<UserInfoModel>? UserSelected;

        public UsersList()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is UserInfoModel user)
            {
                UserSelected?.Invoke(user);
            }
        }

        /// <summary>
        /// Медленный скрол
        /// </summary>
        private void ListBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            this._mouseUtilities.PreviewMouseWheel(sender, e);
        }
    }
}
