using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.UI.Controllers
{
    public class ConnectionStateService : INotifyPropertyChanged
    {
        private static ConnectionStateService _instance;
        public static ConnectionStateService Instance => _instance ??= new ConnectionStateService();

        private string _state = "Онлайн";
        public string State
        {
            get => _state;
            set
            {
                _state = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            }
        }

        public void SetState(string s) => State = s;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
