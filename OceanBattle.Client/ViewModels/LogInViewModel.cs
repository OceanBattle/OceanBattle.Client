using Avalonia.Metadata;
using OceanBattle.Client.Core.Abstractions;
using OceanBattle.Client.DataStore;
using OceanBattle.Client.Factories;
using OceanBattle.DataModel.DTOs;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace OceanBattle.Client.ViewModels
{
    public class LogInViewModel : ViewModelBase
	{
        private readonly ViewChanger _viewChanger;
        private readonly IRegisterViewModelFactory _registerViewModelFactory;
        private readonly IGameHubViewModelFactory _gameHubViewModelFactory;
        private readonly IAuthApiClient _authApi;
        private readonly IUserApiClient _userApi;
        private readonly IClientDataStore _clientDataStore;

        private bool _saveLogInData;
        public bool SaveLogInData
        {
            get => _saveLogInData;
            set => this.RaiseAndSetIfChanged(ref _saveLogInData, value);
        }

        public bool IsErrorVisible => !string.IsNullOrEmpty(Error);

        private string? _error;
        public string? Error
        {
            get => _error;
            set
            {
                this.RaiseAndSetIfChanged(ref _error, value);
                this.RaisePropertyChanged(nameof(IsErrorVisible));
            }
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public LogInViewModel(
            ViewChanger viewChanger,
            IRegisterViewModelFactory registerViewModelFactory,
            IGameHubViewModelFactory gameHubViewModelFactory,
            IAuthApiClient authApi,
            IUserApiClient userApi,
            IClientDataStore clientDataStore)
        {
            _viewChanger = viewChanger;
            _registerViewModelFactory = registerViewModelFactory;
            _gameHubViewModelFactory = gameHubViewModelFactory;
            _authApi = authApi;
            _userApi = userApi;
            _clientDataStore = clientDataStore;
        }

        public async Task LogIn()
        {
            await _clientDataStore.EnsureDataStoreCreatedAsync();

            Settings? settings = null;

            if (await _clientDataStore.HasSettingsAsync())
                settings = await _clientDataStore.GetSettingsAsync();
            else
                settings = new Settings();

            LogInRequest logInRequest = new LogInRequest
            {
                Email = Email,
                Password = Password
            };

            AuthResponse? response = null;

            try
            {
                response = await _authApi.PostLogIn(logInRequest);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return;
            }

            UserDto? user = null;

            try
            {
                user = await _userApi.GetUser();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return;
            }

            if (SaveLogInData)
            {
                LogInData logInData = new LogInData
                {
                    BearerToken = response!.BearerToken,
                    RefreshToken = response!.RefreshToken
                };

                await _clientDataStore.SaveLogInDataAsync(logInData);
            }

            settings!.SaveLogInData = SaveLogInData;

            await _clientDataStore.SaveSettingsAsync(settings);

            _viewChanger(_gameHubViewModelFactory.Create(user!));
        }

        [DependsOn(nameof(Password))]
        [DependsOn(nameof(Email))]
        public bool CanLogIn(object parameter)
        {
            return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        }

        public void Register()
        {
            _viewChanger(_registerViewModelFactory.Create(this, this));
        }
    }
}