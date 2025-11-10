using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Services;
using System.Threading.Tasks;

namespace Parmigiano.Repository
{
    public class AuthApiRepository : IAuthApiRepository
    {
        private readonly HttpClientService _httpClient = new(Config.Current.HTTP_SERVER_ADDR);
        private readonly IUserConfigRepository _userConfig = new UserConfigRepository();

        private readonly string _apiPath = "auth";

        public async Task<string?> AuthCreate(AuthCreateModel model)
        {
            return await this._httpClient.PostAsync<string>($"{this._apiPath}/create", model);
        }

        public async Task<string?> AuthLogin(AuthLoginModel model)
        {
            return await this._httpClient.PostAsync<string>($"{this._apiPath}/login", model);
        }

        public async Task<string?> AuthEmailConfirmReq(ReqEmailConfirmModel model)
        {
            return await this._httpClient.PostAsync<string>($"{this._apiPath}/confirm/req", model);
        }

        public async Task<string> AuthDelete()
        {
            string resp = await this._httpClient.DeleteAsync<string>($"{this._apiPath}/delete");

            this._userConfig.DeleteKey("access_token");

            AppSession.CurrentUser = null;

            return resp;
        }
    }
}
