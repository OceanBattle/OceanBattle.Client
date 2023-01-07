using Microsoft.AspNetCore.SignalR.Client;
using OceanBattle.Client.ViewModels;

namespace OceanBattle.Client.Factories
{
    public interface ICreatorViewModelFactory
    {
        CreatorViewModel Create(
            HubConnection connection, 
            ViewModelBase prev,
            SetBattlefield battlefieldSetter);
    }
}
