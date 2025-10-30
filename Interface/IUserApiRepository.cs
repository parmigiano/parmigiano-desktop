﻿using Parmigiano.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Interface
{
    public interface IUserApiRepository
    {
        Task<UserInfoModel?> GetUserMe();
    }
}
