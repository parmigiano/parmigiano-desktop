using Parmigiano.Interface;
using Parmigiano.Repository;
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
    /// Логика взаимодействия для EmailConfirmedWindow.xaml
    /// </summary>
    public partial class EmailConfirmedWindow : Window
    {
        private readonly IAuthApiRepository _authApi = new AuthApiRepository();

        public EmailConfirmedWindow()
        {
            InitializeComponent();
        }

        private async void EmailConfirmReq_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = await this._authApi.AuthEmailConfirmReq();
        }
    }
}
