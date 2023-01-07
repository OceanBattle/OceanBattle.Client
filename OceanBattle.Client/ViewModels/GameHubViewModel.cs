using Microsoft.AspNetCore.SignalR.Client;
using OceanBattle.Client.Factories;
using OceanBattle.Client.Models;
using OceanBattle.DataModel.ClientData;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Ships;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OceanBattle.Client.ViewModels
{
	public delegate void UpdateActivePlayers(IEnumerable<UserDto> players);
	public delegate void SetBattlefield(BattlefieldDto battlefield);

	public class GameHubViewModel : ViewModelBase
	{
		private UpdateActivePlayers? playersUpdater = null;

		private readonly HubConnection _connection;
        private readonly ViewChanger _viewChanger;
		private readonly ICreatorViewModelFactory _creatorViewModelFactory;
		private readonly IGameViewModelFactory _gameviewModelFactory;
		private readonly IBattleViewModelFactory _battleViewModelFactory;
		private readonly IEndViewModelFactory _endViewModelFactory;

		private UserDto? _player;
		public UserDto? Player
		{
			get => _player;
			set => this.RaiseAndSetIfChanged(ref _player, value);
		}

		private BattlefieldDto? _battlfield;
		public BattlefieldDto? Battlefield
		{
			get => _battlfield;
			set
			{
				this.RaiseAndSetIfChanged(ref _battlfield, value);

				if (GameViewModel is not null)
					GameViewModel.Battlefield = Battlefield;
			}
		}

		private ObservableCollection<Invite>? _invites;
		public ObservableCollection<Invite>? Invites
		{
			get => _invites;
			set => this.RaiseAndSetIfChanged(ref _invites, value);
		}

		public bool IsInviteSelected => SelectedInvite is not null; 

		private Invite? _selectedInvite;
		public Invite? SelectedInvite
		{
			get => _selectedInvite;
			set
			{
				this.RaiseAndSetIfChanged(ref _selectedInvite, value);
				this.RaisePropertyChanged(nameof(IsInviteSelected));
			}
		}

		private GameViewModel? _gameViewModel;
		public GameViewModel? GameViewModel
		{
			get => _gameViewModel;
			set => this.RaiseAndSetIfChanged(ref _gameViewModel, value);
		}

		private BattleViewModel? _battleViewModel;
		public BattleViewModel? BattleViewModel
		{
			get => _battleViewModel;
			set => this.RaiseAndSetIfChanged(ref _battleViewModel, value);
		}

		public GameHubViewModel(
			UserDto user,
			ViewChanger viewChanger,
			ICreatorViewModelFactory creatorViewModelFactory,
			IGameViewModelFactory gameViewModelFactory,
			IBattleViewModelFactory battleViewModelFactory,
			IEndViewModelFactory endViewModelFactory,
			HubConnection connection) 
		{
			_viewChanger = viewChanger;
			_creatorViewModelFactory = creatorViewModelFactory;
			_gameviewModelFactory = gameViewModelFactory;
			_connection = connection;
			_battleViewModelFactory = battleViewModelFactory;
			_endViewModelFactory = endViewModelFactory;
			Player = user;

            Invites = new ObservableCollection<Invite>();

			ConfigureSignalR();
			StartSignalR();
		}

		public void CreateSession()
		{
			SetBattlefield battlefieldSetter = 
				battlefield => Battlefield = battlefield;

			CreatorViewModel vm = 
				_creatorViewModelFactory.Create(_connection!, this, battlefieldSetter);

			playersUpdater = 
				new UpdateActivePlayers(
					players => vm.ActiveUsers = new ObservableCollection<UserDto>(players));

			_viewChanger(vm);
		}

		public void DeclineInvite()
		{
			if (Invites is not null &&
				SelectedInvite is not null &&
				Invites.Contains(SelectedInvite))
				Invites.Remove(SelectedInvite);
		}

		public async Task AcceptInvite()
		{
			if (SelectedInvite is null)
				return;

			UserDto sender = new UserDto
			{
				Email = SelectedInvite.SenderEmail,
				UserName = SelectedInvite.SenderUserName
			};

			try
			{
                Battlefield = 
					await _connection!.InvokeAsync<BattlefieldDto>("AcceptInvite", sender);
            }
			catch (Exception ex)
			{
				return;
			}
		}

		private void ConfigureSignalR()
		{
            _connection.On<IEnumerable<UserDto>>(nameof(IGameClient.UpdateActiveUsersAsync), players =>
			{
				if (playersUpdater is not null)
					playersUpdater(players);
			});

			_connection.On<UserDto>(nameof(IGameClient.InviteAsync), sender =>
			{
				Invites!.Add(new Invite 
				{ 
					SenderUserName = sender.UserName,
					SenderEmail = sender.Email,
				});
			});

			_connection.On<LevelDto>(nameof(IGameClient.StartDeploymentAsync), levelDto =>
			{
				Level level = new Level
				{
					Id = levelDto.Id,
					BattlefieldSize = levelDto.BattlefieldSize,
					AvailableTypes = levelDto.AvailableTypes is null ? null :
					levelDto.AvailableTypes.ToDictionary(kvp => typesReversed[kvp.Key], kvp => kvp.Value)
				};

                GameViewModel = _gameviewModelFactory.Create(Player!, level);
                _viewChanger(GameViewModel);
			});

			_connection.On<BattlefieldDto>(nameof(IGameClient.StartGameAsync), oponentBattlefield =>
			{
				if (GameViewModel is not null && 
					Battlefield is not null)
				{
					BattleViewModel = 
						_battleViewModelFactory.Create(oponentBattlefield, Battlefield);

					GameViewModel.Content = BattleViewModel;
				}
			});

			_connection.On<BattlefieldDto, int, int>(nameof(IGameClient.GotHitAsync), (battlefield, x, y) =>
			{
				Battlefield = battlefield;

				if (BattleViewModel is not null)
					BattleViewModel.GotHit(battlefield, x, y);
			});

			_connection.On(nameof(IGameClient.EndGameAsync), () =>
			{
				EndViewModel endViewModel = _endViewModelFactory.Create(this);

				_viewChanger(endViewModel);

				if (Battlefield is not null && 
					Battlefield.Ships is not null &&
					Battlefield.Ships.Any(s => !s.IsDestroyed))
					endViewModel.Won();
				else
					endViewModel.Lost();
			});
        }

		private async void StartSignalR()
		{
			if (_connection is null)
				return;

            try
            {
                await _connection.StartAsync();
				await _connection.InvokeAsync("MakeInactive");
                await _connection.InvokeAsync("MakeActive");
            }
            catch (Exception ex)
            {
				return;
            }
        }

        private readonly Dictionary<string, Type> typesReversed = new Dictionary<string, Type>
        {
            { nameof(Corvette), typeof(Corvette) },
            { nameof(Frigate), typeof(Frigate) },
            { nameof(Destroyer), typeof(Destroyer) },
            { nameof(Cruiser), typeof(Cruiser) },
            { nameof(Battleship), typeof(Battleship) }
        };
    }
}