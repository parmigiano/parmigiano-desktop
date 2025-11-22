using Parmigiano.Models;
using Parmigiano.Utilities;
using Parmigiano.ViewModel;
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

        public UsersViewModel ViewModel { get; private set; }

        public event Action<ChatMinimalWithLMessageModel>? UserSelected;

        public UsersList()
        {
            InitializeComponent();

            this.ViewModel = new UsersViewModel();
            this.DataContext = ViewModel;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is ChatMinimalWithLMessageModel user)
            {
                UserSelected?.Invoke(user);
            }
        }

        private void UserProfile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UserMyProfileModal.ShowProfile();
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
