using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.SignalR.Client;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game.Abstractions;
using ReactiveUI;

namespace OceanBattle.Client.ViewModels
{
	public class BattleViewModel : ViewModelBase
	{
		private readonly HubConnection _connection;

		private ObservableCollection<Weapon>? _weapons;
		public ObservableCollection<Weapon>? Weapons
		{
			get => _weapons;
			set => this.RaiseAndSetIfChanged(ref _weapons, value);
		}

		private Weapon? _selectedWeapon;
		public Weapon? SelectedWeapon
		{
			get => _selectedWeapon;
			set => this.RaiseAndSetIfChanged(ref _selectedWeapon, value);
		}

		private BattlefieldDto? _playerBattlefield;
		public BattlefieldDto? PlayerBattlefield
		{
			get => _playerBattlefield;
			set
			{
				if (value is not null && 
					value.Ships is not null)
				{
					List<Weapon> weapons = new List<Weapon>();

                    foreach (Warship warship in value.Ships)                   
						if (warship.Weapons is not null)
							weapons.AddRange(warship.Weapons);

					Weapons = new ObservableCollection<Weapon>(weapons);
                }
				
				this.RaiseAndSetIfChanged(ref _playerBattlefield, value);
			}
		}

		private BattlefieldDto? _oponentBattlefield;
		public BattlefieldDto? OponentBattlefield
		{
			get => _oponentBattlefield;
			set => this.RaiseAndSetIfChanged(ref _oponentBattlefield, value);
		}

		public BattleViewModel(
			HubConnection connection,
			BattlefieldDto oponentBattlefield, 
			BattlefieldDto playerBattlefield) 
		{
			_connection = connection;

			OponentBattlefield = oponentBattlefield;
			PlayerBattlefield = playerBattlefield;			
		}

		public void GotHit(BattlefieldDto battlefield, int x, int y)
		{
			PlayerBattlefield = battlefield;
		}

		public async void Hit(int x, int y)
		{
			if (SelectedWeapon is null)
				return;

			bool result =
				await _connection.InvokeAsync<bool>("CanBeHit", x, y);

			if (!result)
				return;

			BattlefieldDto? battlefield =
				await _connection.InvokeAsync<BattlefieldDto?>("Hit", x, y, SelectedWeapon);

			if (battlefield is null)
				return;

			OponentBattlefield = battlefield;
		}
	}
}