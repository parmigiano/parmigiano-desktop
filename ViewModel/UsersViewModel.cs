using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Parmigiano.ViewModel
{
    public class UsersViewModel
    {
        private readonly IUserApiRepository _userApi = new UserApiRepository();

        public ObservableCollection<UserMinimalWithLMessageModel> Users { get; set; }

        public UsersViewModel()
        {
            Users = new ObservableCollection<UserMinimalWithLMessageModel>();

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                List<UserMinimalWithLMessageModel> users = await this._userApi.GetUsersMinimalWithLMessage();

                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (JsonException ex)
            {
                Logger.Error($"Ошибка парсинга JSON: {ex.Message}");
            }
        }
    }
}
