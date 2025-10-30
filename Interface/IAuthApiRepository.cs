﻿using Parmigiano.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Interface
{
    public interface IAuthApiRepository
    {
        Task<string?> AuthCreate(AuthCreateModel model);

        Task<string?> AuthLogin(AuthLoginModel model);
    }
}
