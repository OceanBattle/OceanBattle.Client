using System;
using System.Collections.Generic;
using System.Numerics;
using OceanBattle.Client.Factories;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using ReactiveUI;

namespace OceanBattle.Client.ViewModels
{
	public class GameViewModel : ViewModelBase
	{
		private readonly IDeploymentViewModelFactory _deploymentViewModelFactory;

		private ViewModelBase? _content;
		public ViewModelBase? Content
		{
			get => _content;
			set => this.RaiseAndSetIfChanged(ref _content, value);
		}

		private Level? _level;
		public Level? Level
		{
			get => _level;
			set => this.RaiseAndSetIfChanged(ref _level, value);
		}

		private UserDto? _player;
		public UserDto? Player
		{
			get => _player;
			set => this.RaiseAndSetIfChanged(ref _player, value);
		}

		private BattlefieldDto? _battlefield;
		public BattlefieldDto? Battlefield
		{
			get => _battlefield;
			set
			{
                this.RaiseAndSetIfChanged(ref _battlefield, value);
             
				if (Player is not null && 
					Level is not null && 
					Battlefield is not null)
					Content = _deploymentViewModelFactory.Create(Player, Level, Battlefield);
            }
		}

		public GameViewModel(
			IDeploymentViewModelFactory deploymentViewModelFactory,
			UserDto player,
			Level level)
		{
			_deploymentViewModelFactory = deploymentViewModelFactory;
			Player = player;
			Level = level;
		}
	}
}