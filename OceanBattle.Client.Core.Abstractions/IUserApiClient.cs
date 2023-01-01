using OceanBattle.DataModel.DTOs;

namespace OceanBattle.Client.Core.Abstractions
{
    public interface IUserApiClient
    {
        Task PostRegister(RegisterRequest registerRequest);
        Task<UserDto?> GetUser();
    }
}
