using OceanBattle.Client.Core.Abstractions;
using OceanBattle.DataModel.DTOs;
using System.Net.Http.Json;

namespace OcenBattle.Client.Core.Services
{
    public class UserApiClient : ApiClientBase, IUserApiClient
    {
        public UserApiClient(HttpClient httpClient)
            : base(httpClient)
        {            
        }

        public async Task<UserDto?> GetUser()
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync($"user/user");

            response.EnsureSuccessStatusCode();

            UserDto? user = 
                await response.Content.ReadAsAsync<UserDto>(_formatters);

            return user;
        }
            

        public async Task PostRegister(RegisterRequest registerRequest)
        {
            ObjectContent<RegisterRequest> content = new(registerRequest, _jsonFormatter);

            await _httpClient.PostAsync($"user/register", content);
        }            
    }
}
