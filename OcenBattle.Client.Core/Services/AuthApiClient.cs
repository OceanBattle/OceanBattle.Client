using OceanBattle.Client.Core.Abstractions;
using OceanBattle.Client.DataStore;
using OceanBattle.DataModel.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reactive.Linq;

namespace OcenBattle.Client.Core.Services
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IClientDataStore _clientDataStore;

        public AuthApiClient(
            HttpClient httpClient,
            IClientDataStore clientDataStore) 
        {
            _httpClient = httpClient;
            _clientDataStore = clientDataStore;
        }

        public async Task<AuthResponse?> PostLogIn(TokenRefreshRequest request)
        {
            AuthResponse? auth = await PostRefresh(request);

            if (auth is not null)
                await StartRecurringRefreshes(auth);

            return auth;
        }

        public async Task<AuthResponse?> PostLogIn(LogInRequest request)
        {
            var response = 
                await _httpClient.PostAsJsonAsync($"auth/login", request);

            AuthResponse? auth = 
                await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (auth is not null)
                await StartRecurringRefreshes(auth);
                
            return auth;
        }

        public Task PostLogOut()
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponse?> PostRefresh(TokenRefreshRequest request)
        {
            var response = 
                await _httpClient.PostAsJsonAsync($"auth/refresh", request);

            AuthResponse? auth = 
                await response.Content.ReadFromJsonAsync<AuthResponse>();

            return auth;
        }

        #region private helpers

        private async Task StartRecurringRefreshes(AuthResponse? auth)
        {
            if (auth is null)
                return;

            _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", auth.BearerToken);

            var sequence = Observable.Timer(
                TimeSpan.FromMinutes(15),
                TimeSpan.FromMinutes(15));

            Settings? settings = null;

            if (await _clientDataStore.HasSettingsAsync())
                settings = await _clientDataStore.GetSettingsAsync();

            sequence.Subscribe(async arg =>
            {
                TokenRefreshRequest request = new TokenRefreshRequest
                {
                    BearerToken = auth.BearerToken,
                    RefreshToken = auth.RefreshToken
                };

                AuthResponse? response = null;

                try
                {
                    response = await PostRefresh(request);
                }
                catch (Exception ex)
                {
                }

                if (response is not null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", response.BearerToken);

                    if (settings is not null && settings.SaveLogInData)
                    {
                        LogInData logInData = new LogInData
                        {
                            BearerToken = response.BearerToken,
                            RefreshToken = response.RefreshToken
                        };

                        await _clientDataStore.SaveSettingsAsync(settings);
                    }
                }
            });
        }

        #endregion
    }
}
