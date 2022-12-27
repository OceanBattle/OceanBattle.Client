using Avalonia.Metadata;
using OceanBattle.Client.Factories;
using ReactiveUI;

namespace OceanBattle.Client.ViewModels
{
    public class LogInViewModel : ViewModelBase
	{
        private readonly ViewChanger _viewChanger;
        private readonly IRegisterViewModelFactory _registerViewModelFactory;
        private readonly GameHubViewModel _gameHubViewModel;

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
            GameHubViewModel gameHubViewModel)
        {
            _viewChanger = viewChanger;
            _registerViewModelFactory = registerViewModelFactory;
            _gameHubViewModel = gameHubViewModel;
        }

        public void LogIn()
        {
            _viewChanger(_gameHubViewModel);
        }

        [DependsOn(nameof(Password))]
        [DependsOn(nameof(Email))]
        public bool CanLogIn(object parameter)
        {
            return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        }

        public void Register()
        {
            _viewChanger(_registerViewModelFactory.Create(this));
        }
    }
}