using System;
using System.Collections.Generic;
using OceanBattle.Client.Factories;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
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

		public GameViewModel(
			IDeploymentViewModelFactory deploymentViewModelFactory,
			UserDto player,
			Level level)
		{
			_deploymentViewModelFactory = deploymentViewModelFactory;
			Content = _deploymentViewModelFactory.Create(player, level);
		}
	}
}