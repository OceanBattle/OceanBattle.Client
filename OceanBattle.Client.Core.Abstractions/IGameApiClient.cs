using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;

namespace OceanBattle.Client.Core.Abstractions
{
    public interface IGameApiClient
    {
        IEnumerable<Level> GetLevels();
        IEnumerable<UserDto> GetActivePlayers();
    }
}
