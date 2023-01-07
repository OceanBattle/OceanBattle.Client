using OceanBattle.Client.ViewModels;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Client.Factories
{
    public interface IDeploymentViewModelFactory
    {
        DeploymentViewModel Create(
            UserDto player, 
            Level level,
            BattlefieldDto battlefield);
    }
}
