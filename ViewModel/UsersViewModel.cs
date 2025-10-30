using Parmigiano.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.ViewModel
{
    public class UsersViewModel
    {
        public ObservableCollection<UserMassModel> Users { get; set; }
        public UserMassModel SelectedUser { get; set; }

        public UsersViewModel()
        {
            Users = new ObservableCollection<UserMassModel>
            {
                new UserMassModel { Username = "Elena1", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena2", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena3", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena4", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena5", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena6", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena7", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena8", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena9", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena10", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena11", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena12", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Elena13", Avatar = "https://github.githubassets.com/assets/starstruck-default-b6610abad518.png", LastMessage = "See you tomorrow" },
                new UserMassModel { Username = "Michael14", Avatar = "https://github.githubassets.com/assets/quickdraw-default-39c6aec8ff89.png", LastMessage = "Guys, I love this idea!" },
                new UserMassModel { Username = "Evgeniy15", Avatar = null, LastMessage = "Ok 👍" }
            };
        }
    }
}
