using Parmigiano.Services;
using System.Windows;
using System.Windows.Input;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для UpdateAvailableWindow.xaml
    /// </summary>
    public partial class UpdateAvailableWindow : Window
    {
        private string _dwnlUpdate;

        public UpdateAvailableWindow(string downloadUrl)
        {
            InitializeComponent();

            this._dwnlUpdate = downloadUrl;
        }

        private async void UpdButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await UpdateAppService.DownloadAndUpdateAsync(this._dwnlUpdate);
        }
    }
}
