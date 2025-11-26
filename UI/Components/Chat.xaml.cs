using Parmigiano.ViewModel;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            ViewModel.ChatSettingUpdated += () =>
            {
                Application.Current.Dispatcher.Invoke(SetChatBackground);
            };
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

        private async void ProfileHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is Parmigiano.ViewModel.ChatViewModel vm && vm.SelectedUser != null)
            {
                await UserProfileModal.ShowProfile(vm.SelectedUser.UserUid, vm);
            }
        }

        private void ChatBackground_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is Parmigiano.ViewModel.ChatViewModel vm && vm.SelectedUser != null)
            {
                ChatSettingModal.Show(vm.SelectedUser.Id);
            }
        }

        public void SetChatBackground()
        {
            if (ViewModel.ChatSetting != null && !string.IsNullOrEmpty(ViewModel.ChatSetting.CustomBackground))
            {
                ChatScrollViewer.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(ViewModel.ChatSetting.CustomBackground, UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.UniformToFill,
                    Opacity = 0.55,
                };
            }
            else
            {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                var img1 = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Public/assets/chat-bg-1.png")),
                    Stretch = Stretch.UniformToFill
                };

                Grid.SetColumn(img1, 0);
                RenderOptions.SetBitmapScalingMode(img1, BitmapScalingMode.HighQuality);

                var img2 = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Public/assets/chat-bg-1.png")),
                    Stretch = Stretch.UniformToFill
                };

                Grid.SetColumn(img2, 1);
                RenderOptions.SetBitmapScalingMode(img2, BitmapScalingMode.HighQuality);

                grid.Children.Add(img1);
                grid.Children.Add(img2);

                ChatScrollViewer.Background = new VisualBrush
                {
                    Visual = grid,
                    Opacity = 0.085,
                    Stretch = Stretch.UniformToFill
                };
            }
        }
    }
}
