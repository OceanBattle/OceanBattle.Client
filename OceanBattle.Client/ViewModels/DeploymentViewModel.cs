using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Metadata;
using Microsoft.AspNetCore.SignalR.Client;
using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using ReactiveUI;

namespace OceanBattle.Client.ViewModels
{
	public class DeploymentViewModel : ViewModelBase
	{
		private readonly HubConnection _connection;

		private Level? _level;
		public Level? Level
		{
			get => _level;
			set => this.RaiseAndSetIfChanged(ref _level, value);
		}

		private ObservableCollection<Ship>? _availableVessels;
		public ObservableCollection<Ship>? AvailableVessels
		{
			get => _availableVessels;
			set => this.RaiseAndSetIfChanged(ref _availableVessels, value);
		}

		private Ship? _selectedVessel;
		public Ship? SelectedVessel
		{
			get => _selectedVessel;
			set 
			{
                this.RaiseAndSetIfChanged(ref _selectedVessel, value);
            }
		}

		private BattlefieldDto? _battlefield;
		public BattlefieldDto? Battlefield
		{
			get => _battlefield;
			set => this.RaiseAndSetIfChanged(ref _battlefield, value);
		}

		public DeploymentViewModel(
			HubConnection connection,
			Level level,
			UserDto player, 
			BattlefieldDto battlefield)
		{
			_connection = connection;
			Level = level;
			Battlefield = battlefield;

			if (player.OwnedVessels is not null &&
				Level.AvailableTypes is not null)
			{

				List<Ship> vessels = new List<Ship>();

				foreach (Type type in Level.AvailableTypes.Keys)
				{
					vessels.AddRange(
						player.OwnedVessels.Where(v => v.GetType() == type)
										   .Take(Level.AvailableTypes[type]));
				}

				AvailableVessels =
					new ObservableCollection<Ship>(
						vessels.OrderBy(
							v => v.GetType().Name, StringComparer.Ordinal));
			}			
		}

		[DependsOn(nameof(AvailableVessels))]
		public bool CanReady(object parameter)
		{ 
			return AvailableVessels is not null && 
				   AvailableVessels.Count < 1;
		}

		public async Task Ready()
		{
			try
			{
                await _connection.InvokeAsync("ConfirmReady");
            }
			catch (Exception ex)
			{
				return;
			}
		}

		public async void PlaceShip(int x, int y)
		{
			if (SelectedVessel is null) 
				return;

			bool result = 
				await _connection.InvokeAsync<bool>("CanPlaceShip", x, y, SelectedVessel);

			if (!result)
				return;

			BattlefieldDto? battlefield = 
				await _connection.InvokeAsync<BattlefieldDto?>("PlaceShip", x, y, SelectedVessel);

			if (battlefield is null)
				return;

			Battlefield = battlefield;

			AvailableVessels!.Remove(SelectedVessel);
		}
	}
}