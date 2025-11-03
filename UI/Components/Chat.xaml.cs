using Parmigiano.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        public ChatViewModel ViewModel { get; } = new ChatViewModel();

        public Chat()
        {
            InitializeComponent();
            DataContext = ViewModel;

            this.Loaded += Chat_Loaded;
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Parmigiano.ViewModel.ChatViewModel vm)
            {
                ((INotifyCollectionChanged)vm.Messages).CollectionChanged += (s, ev) =>
                {
                    ChatScrollViewer?.ScrollToEnd();
                };
            }
        }

        private void MessageContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu && DataContext is Parmigiano.ViewModel.ChatViewModel vm)
            {
                menu.DataContext = vm;
            }
        }

        private void MessageBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                int caret = MessageBox.CaretIndex;
                MessageBox.Text = MessageBox.Text.Insert(caret, Environment.NewLine);
                MessageBox.CaretIndex = caret + Environment.NewLine.Length;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                e.Handled = true;

                if (DataContext is Parmigiano.ViewModel.ChatViewModel vm && vm.SendMessageCommand.CanExecute(null))
                {
                    vm.SendMessageCommand.Execute(null);
                }
            }
        }
    }
}
