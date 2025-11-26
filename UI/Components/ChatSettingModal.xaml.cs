using Microsoft.Win32;
using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.Utilities;
using SharpCompress.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Parmigiano.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для ChatSettingModal.xaml
    /// </summary>
    public partial class ChatSettingModal : UserControl
    {
        private static readonly IChatApiRepository _chatApi = new ChatApiRepository();
        private static ImageUtilities _imageUtilities = new ImageUtilities();

        private static ChatSettingModal _instance;

        private static ulong _chatId = 0;

        public ChatSettingModal()
        {
            InitializeComponent();
            _instance = this;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            HideOverlay();
        }

        private void Root_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsClickInsideDialog(e))
            {
                HideOverlay();
            }
        }

        private bool IsClickInsideDialog(System.Windows.Input.MouseButtonEventArgs e)
        {
            var clickedElement = e.OriginalSource as DependencyObject;

            while (clickedElement != null)
            {
                if (clickedElement == Dialog)
                {
                    return true;
                }

                clickedElement = VisualTreeHelper.GetParent(clickedElement);
            }

            return false;
        }

        public static async void Show(ulong chatId)
        {
            if (_instance == null) return;

            _chatId = chatId;

            ChatSettingModel? chatSetting = await _chatApi.GetChatSetting(chatId);

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance == null) return;

                if (chatSetting != null)
                {
                    _instance.DataContext = chatSetting;
                }

                if (!string.IsNullOrEmpty(chatSetting.CustomBackground))
                {
                    _imageUtilities.LoadImageAsync(chatSetting.CustomBackground, _instance.ChatBackgroundImage);
                }
                else
                {
                    var defaultImage = new BitmapImage(new Uri("pack://application:,,,/Public/assets/chat-bg-1.png"));
                    _instance.ChatBackgroundImage.ImageSource = defaultImage;
                }

                _instance.Visibility = Visibility.Visible;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(100)));
                _instance.BeginAnimation(OpacityProperty, fadeIn);
            });
        }

        public static void HideOverlay()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance == null) return;

                var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(70)));
                fadeOut.Completed += (s, e) =>
                {
                    _instance.Visibility = Visibility.Collapsed;
                };

                _instance.BeginAnimation(OpacityProperty, fadeOut);
            });
        }

        private async void ChatBackground_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Выберите фотографию",
                    Filter = "Изображения|*.png;*.jpg;*.jpeg;*.gif;*.bmp",
                    Multiselect = false
                };

                if (dialog.ShowDialog() != true)
                {
                    return;
                }

                string filePath = dialog.FileName;

                string? url = await _chatApi.ChatUpdateCustomBackground(_chatId, filePath);

                if (url != null)
                {
                    var bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(url);
                    bitmap.EndInit();

                    ChatBackgroundRect.Fill = new ImageBrush(bitmap)
                    {
                        Stretch = Stretch.UniformToFill
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка: {ex.Message}");
            }
        }

        private async void ChatBackgroundClear_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try {
                _ = await _chatApi.ChatUpdateCustomBackground(_chatId, null);

                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Public/assets/chat-bg-1.png"));
                bitmap.EndInit();

                ChatBackgroundRect.Fill = new ImageBrush(bitmap)
                {
                    Stretch = Stretch.UniformToFill
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка: {ex.Message}");
            }
        }
    }
}
