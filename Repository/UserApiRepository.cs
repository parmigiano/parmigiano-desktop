﻿using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Repository
{
    public class UserApiRepository : IUserApiRepository
    {
        private readonly HttpClientService _httpClient = new(Config.Current.HTTP_SERVER_ADDR);

        private readonly string _apiPath = "users";

        public async Task<UserInfoModel?> GetUserMe()
        {
            return await this._httpClient.GetAsync<UserInfoModel>($"{this._apiPath}/me");
        }

        public async Task<List<UserMinimalWithLMessageModel>?> GetUsersMinimalWithLMessage()
        {
            return await this._httpClient.GetAsync<List<UserMinimalWithLMessageModel>>($"{this._apiPath}/last-messages");
        }

        public async Task<string?> UploadAvatar(string filePath)
        {
            return await this._httpClient.UploadFile($"{this._apiPath}/upload/avatar", filePath);
        }
    }
}
