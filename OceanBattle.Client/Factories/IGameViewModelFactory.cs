using OceanBattle.Client.ViewModels;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;

namespace OceanBattle.Client.Factories
{
    public interface IGameViewModelFactory
    {
        GameViewModel Create(UserDto player, Level level);
    }
}
