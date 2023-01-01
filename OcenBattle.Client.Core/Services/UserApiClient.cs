using OceanBattle.Client.Core.Abstractions;
using OceanBattle.DataModel.DTOs;
using System.Net.Http.Json;

namespace OcenBattle.Client.Core.Services
{
    public class UserApiClient : IUserApiClient
    {
        private readonly HttpClient _httpClient;

        public UserApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDto?> GetUser() 
            => await _httpClient.GetFromJsonAsync<UserDto>($"user/user");

        public async Task PostRegister(RegisterRequest registerRequest) 
            => await _httpClient.PostAsJsonAsync($"user/register", registerRequest);
    }
}
