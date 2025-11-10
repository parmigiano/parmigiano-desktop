using Parmigiano.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Core
{
    public static class AppSession
    {
        public static UserInfoModel? CurrentUser { get; set; }
    }
}
