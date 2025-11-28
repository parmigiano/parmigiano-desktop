using Parmigiano.ViewModel;
using System;
using System.Collections.Specialized;
using System.Linq;
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

        private bool _firstLoadDone = false;
        private bool _isUserNearBottom = true;

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
            this._firstLoadDone = false;

            ChatScrollViewer.ScrollChanged += ChatScrollViewer_ScrollChanged;

            if (DataContext is Parmigiano.ViewModel.ChatViewModel vm)
            {
                vm.PropertyChanged += (s, ev) =>
                {
                    if (ev.PropertyName == nameof(vm.SelectedUser))
                    {
                        this.SubscribeToMessagesCollection(vm);
                    }
                };

                this.SubscribeToMessagesCollection(vm);
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

        private void ScrollToBottomButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChatScrollViewer.ScrollToEnd();
        }

        private async void ChatScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double distanceToBottom = ChatScrollViewer.ScrollableHeight - ChatScrollViewer.VerticalOffset;

            this._isUserNearBottom = distanceToBottom < 50;

            this.TryMarkVisibleMessagesAsRead();

            this.ScrollToBottomButton.Visibility = _isUserNearBottom ? Visibility.Collapsed : Visibility.Visible;

            if (this._firstLoadDone && ChatScrollViewer.VerticalOffset < 20)
            {
                if (DataContext is ChatViewModel vm)
                {
                    double prevHeight = ChatScrollViewer.ExtentHeight;
                    await vm.LoadOlderMessagesAsync();

                    ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ExtentHeight - prevHeight);
                }
            }
        }

        private void TryMarkVisibleMessagesAsRead()
        {
            if (DataContext is not ChatViewModel vm) return;

            foreach (var msg in vm.Messages)
            {
                if (msg.IsMine) continue;
                if (msg.ReadAt != null) continue;

                var container = MessagesListView.ItemContainerGenerator.ContainerFromItem(msg) as FrameworkElement;
                if (container == null) continue;

                if (this.IsElementVisible(container, ChatScrollViewer))
                {
                    msg.ReadAt = DateTime.Now;

                    _ = vm.MarkMessageAsRead(msg);
                }
            }
        }

        private bool IsElementVisible(FrameworkElement element, ScrollViewer scroll)
        {
            if (!element.IsVisible) return false;

            try
            {
                GeneralTransform transform = element.TransformToAncestor(scroll);
                Rect rect = transform.TransformBounds(new Rect(new Point(0, 0), element.RenderSize));

                Rect viewport = new Rect(new Point(0, 0), new Size(scroll.ViewportWidth, scroll.ViewportHeight));

                return rect.Bottom >= viewport.Top && rect.Top <= viewport.Bottom;
            }
            catch
            {
                return false;
            }
        }

        private void PinnedMessage_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not ChatViewModel vm) return;

            if (vm?.PinnedMessage == null) return;

            MessagesListView.UpdateLayout();

            var item = MessagesListView.ItemContainerGenerator.ContainerFromItem(vm.PinnedMessage) as FrameworkElement;
            if (item == null) return;

            var point = item.TransformToAncestor(this.ChatScrollViewer).Transform(new Point(0, 0));

            this.ChatScrollViewer.ScrollToVerticalOffset(point.Y);
        }

        private void SubscribeToMessagesCollection(ChatViewModel vm)
        {
            this._firstLoadDone = false;
            this._isUserNearBottom = true;

            if (vm.Messages is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += (s, ev) =>
                {
                    if (ev.Action == NotifyCollectionChangedAction.Add)
                    {
                        if (this._isUserNearBottom) this.ChatScrollViewer.ScrollToEnd();
                    }

                    if (!this._firstLoadDone && vm.Messages.Any())
                    {
                        this._firstLoadDone = true;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.ChatScrollViewer.ScrollToEnd();
                        }), System.Windows.Threading.DispatcherPriority.Background);
                    }
                };
            }

            if (vm.Messages.Any())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.ChatScrollViewer.ScrollToEnd();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

    }
}
