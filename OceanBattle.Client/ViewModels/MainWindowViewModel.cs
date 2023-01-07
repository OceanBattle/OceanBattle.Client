using Avalonia.Controls.ApplicationLifetimes;
using OceanBattle.Client.Core.Abstractions;
using OceanBattle.Client.DataStore;
using OceanBattle.Client.Factories;
using OceanBattle.DataModel.DTOs;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace OceanBattle.Client.ViewModels
{
    public delegate void ViewChanger(ViewModelBase viewModel);

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IAuthApiClient _authApi;
        private readonly IClientDataStore _clientDataStore;
        private readonly IGameHubViewModelFactory _gameHubViewModelFactory;
        private readonly ILogInViewModelFactory _logInViewModelFactory;
        private readonly IUserApiClient _userApi;

        private ViewModelBase? _content;
        public ViewModelBase? Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public MainWindowViewModel(
            ILogInViewModelFactory logInViewModelFactory,
            IGameHubViewModelFactory gameHubViewModelFactory,
            IClientDataStore clientDataStore,
            IAuthApiClient authApi,
            IUserApiClient userApi)
        {

            _authApi = authApi;
            _userApi = userApi;
            _clientDataStore = clientDataStore;
            _gameHubViewModelFactory = gameHubViewModelFactory;
            _logInViewModelFactory = logInViewModelFactory;

            LoadContent();
        }

        private async void LoadContent()
        {
            UserDto? user = null;

            if (await LogInWithToken())
                try
                {
                    user = await _userApi.GetUser();
                }
                catch (Exception ex)
                {
                }

            if (user is not null)
                Content = _gameHubViewModelFactory.Create(user);
            else
                Content = _logInViewModelFactory.Create();
        }

        private async Task<bool> LogInWithToken()
        {
            if (!await _clientDataStore.HasLogInDataAsync())
                return false;

            LogInData? logInData = await _clientDataStore.GetLogInDataAsync();

            if (logInData is null || 
                logInData.BearerToken is null || 
                logInData.RefreshToken is null)
                return false;

            TokenRefreshRequest request = new TokenRefreshRequest
            {
                BearerToken = logInData.BearerToken,
                RefreshToken = logInData.RefreshToken
            };

            AuthResponse? refreshResponse = null;

            try
            {
                refreshResponse = await _authApi.PostLogIn(request);
            }
            catch (Exception ex)
            {
                return false;
            }

            logInData.BearerToken = refreshResponse!.BearerToken;
            logInData.RefreshToken = refreshResponse!.RefreshToken;

            await _clientDataStore.SaveLogInDataAsync(logInData);

            return true;
        }
    }
}