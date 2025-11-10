using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public class NetworkService
    {
        public bool IsAvailable => NetworkInterface.GetIsNetworkAvailable();

        public event Action? NetworkAvailabilityChanged;

        public NetworkService()
        {
            NetworkChange.NetworkAvailabilityChanged += (s, e) =>
            {
                NetworkAvailabilityChanged?.Invoke();
            };
        }
    }
}
