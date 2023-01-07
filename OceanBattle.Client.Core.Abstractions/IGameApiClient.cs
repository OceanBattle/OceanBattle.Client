using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;

namespace OceanBattle.Client.Core.Abstractions
{
    public interface IGameApiClient
    {
        Task<IEnumerable<Level>?> GetLevels();
        Task<IEnumerable<UserDto>?> GetActivePlayers();
    }
}
