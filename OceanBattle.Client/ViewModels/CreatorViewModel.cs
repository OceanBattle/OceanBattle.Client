using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Metadata;
using Microsoft.AspNetCore.SignalR.Client;
using OceanBattle.Client.Core.Abstractions;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using ReactiveUI;

namespace OceanBattle.Client.ViewModels
{
	public class CreatorViewModel : ViewModelBase
	{
		private readonly HubConnection _connection;
		private readonly IGameApiClient _gameApi;
		private readonly ViewModelBase _prev;
		private readonly ViewChanger _viewChanger;
		private readonly SetBattlefield _battlefieldSetter;

		private UserDto? _lastInvited;
		public UserDto? LastInvited
		{
			get => _lastInvited;
			set => this.RaiseAndSetIfChanged(ref _lastInvited, value);
		}

		private BattlefieldDto? _battlefield;
		public BattlefieldDto? Battlefield
		{
			get => _battlefield;
			set => this.RaiseAndSetIfChanged(ref _battlefield, value);
		}

		private ObservableCollection<UserDto>? _activeUsers;
		public ObservableCollection<UserDto>? ActiveUsers
		{
			get => _activeUsers;
			set => this.RaiseAndSetIfChanged(ref _activeUsers, value);
		}

		private ObservableCollection<Level>? _levels;
		public ObservableCollection<Level>? Levels
		{
			get => _levels;
			set => this.RaiseAndSetIfChanged(ref _levels, value);
		}

		private UserDto? _selectedOponent;
		public UserDto? SelectedOponent
		{
			get => _selectedOponent;
			set => this.RaiseAndSetIfChanged(ref _selectedOponent, value);
		}

		private Level? _selectedLevel;
		public Level? SelectedLevel
		{
			get => _selectedLevel;
			set => this.RaiseAndSetIfChanged(ref _selectedLevel, value);
		}

		public CreatorViewModel(
			IGameApiClient gameApi,
			ViewChanger viewChanger,
			HubConnection connection,
			ViewModelBase prev,
			SetBattlefield battlefieldSetter)
		{
			_gameApi = gameApi;
			_viewChanger = viewChanger;
			_prev = prev;
			_connection = connection;
			_battlefieldSetter = battlefieldSetter;

			LoadPlayers();
			LoadLevels();
		}

		[DependsOn(nameof(SelectedOponent))]
		[DependsOn(nameof(LastInvited))]
		public bool CanInvite(object parameter)
		{
			return SelectedOponent is not null && SelectedOponent != LastInvited;
		}

		public async Task Invite()
		{
			if (SelectedOponent is null)
				return;

			try
			{
                await _connection.InvokeAsync("InvitePlayer", SelectedOponent);
            }
			catch (Exception ex)
			{
				return;
			}

			LastInvited = SelectedOponent;
		}

		[DependsOn(nameof(SelectedLevel))]
		[DependsOn(nameof(Battlefield))]
		public bool CanCreateSession(object parameter)
		{
			return SelectedLevel is not null && Battlefield is null;
		}

		public async Task CreateSession()
		{
			if (SelectedLevel is null)
				return;

			LevelDto level = new LevelDto
			{
				Id = SelectedLevel.Id,
				BattlefieldSize = SelectedLevel.BattlefieldSize,
				AvailableTypes = SelectedLevel.AvailableTypes is null ? null :
				SelectedLevel.AvailableTypes.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value)
            };

			try
			{
				Battlefield = 
					await _connection.InvokeAsync<BattlefieldDto>("CreateSession", level);

				_battlefieldSetter(Battlefield);
            }
			catch (Exception ex)
			{
				return;
			}		
		}

		public void Close()
		{
			_viewChanger(_prev);
		}

		private async void LoadPlayers()
		{
			try
			{
                IEnumerable<UserDto>? players = 
					await _gameApi.GetActivePlayers();

				if (players is null)
					return;

                ActiveUsers = new ObservableCollection<UserDto>(players);
            }
			catch (Exception ex)
			{
				return;
			}
		}

		private async void LoadLevels()
		{
			try
			{
                IEnumerable<Level>? levels 
					= await _gameApi.GetLevels();

				if (levels is null)
					return;

                Levels = new ObservableCollection<Level>(levels);
            }
			catch (Exception ex)
			{
				return;
			}
		}
	}
}