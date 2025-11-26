using Parmigiano.Core;
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
            UserInfoModel? user = await this._httpClient.GetAsync<UserInfoModel?>($"{this._apiPath}/me");

            // set uid in session
            if (user != null)
            {
                AppSession.CurrentUser = user;
            }

            return user;
        }

        public async Task<UserInfoModel?> GetUserProfile(ulong uid)
        {
            return await this._httpClient.GetAsync<UserInfoModel?>($"{this._apiPath}/{uid}");
        }

        public async Task<List<ChatMinimalWithLMessageModel>?> GetUsersFindByUsername(string username)
        {
            return await this._httpClient.GetAsync<List<ChatMinimalWithLMessageModel>?>($"{this._apiPath}/find/{username}");
        }

        public async Task<string> UpdateUserProfile(UserProfileUpdModel user)
        {
            return await this._httpClient.PatchAsync<string?>($"{this._apiPath}/me", user);
        }

        public async Task<string?> UploadAvatar(string filePath)
        {
            return await this._httpClient.UploadFile($"{this._apiPath}/upload/avatar", filePath, "avatar");
        }
    }
}
