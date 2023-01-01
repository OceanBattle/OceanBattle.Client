using OceanBattle.Client.ViewModels;
using OceanBattle.DataModel.DTOs;

namespace OceanBattle.Client.Factories
{
    public interface IGameHubViewModelFactory
    {
        GameHubViewModel Create(UserDto user);
    }
}
