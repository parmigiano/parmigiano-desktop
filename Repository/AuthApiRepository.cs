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

        private readonly string _apiPath = "auth";

        public Task<string?> AuthCreate(AuthCreateModel model)
        {
            return this._httpClient.PostAsync<string>($"{this._apiPath}/create", model);
        }

        public Task<string?> AuthLogin(AuthLoginModel model)
        {
            return this._httpClient.PostAsync<string>($"{this._apiPath}/login", model);
        }
    }
}
