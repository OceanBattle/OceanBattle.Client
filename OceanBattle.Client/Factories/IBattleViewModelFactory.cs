using OceanBattle.Client.ViewModels;
using OceanBattle.DataModel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Client.Factories
{
    public interface IBattleViewModelFactory
    {
        BattleViewModel Create(
            BattlefieldDto oponentBattlefield,
            BattlefieldDto playerBattlefield);
    }
}
