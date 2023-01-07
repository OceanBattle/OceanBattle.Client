using OceanBattle.DataModel.DTOs;

namespace OceanBattle.Client.Core.Abstractions
{
    public interface IAuthApiClient
    {
        Task<AuthResponse?> PostLogIn(LogInRequest request);
        Task<AuthResponse?> PostLogIn(TokenRefreshRequest request);
        Task<AuthResponse?> PostRefresh(TokenRefreshRequest request);
        Task PostLogOut(); 
    }
}
