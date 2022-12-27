using Avalonia.Metadata;
using ReactiveUI;

namespace OceanBattle.Client.ViewModels
{
    public class RegisterViewModel : ViewModelBase
	{
        private readonly ViewChanger _viewChanger;
        private readonly ViewModelBase _next;

		private string? _firstName;
		public string? FirstName
		{
			get => _firstName;
			set => this.RaiseAndSetIfChanged(ref _firstName, value);
		}

        private string? _lastName;
        public string? LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        private string? _userName;
        public string? UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private string? _confirmPassword;
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
        }

        public RegisterViewModel(
            ViewChanger viewChanger,
            ViewModelBase next)
        {
            _viewChanger = viewChanger;
            _next = next;
        }

        public void Register()
        {
            _viewChanger(_next);
        }

        [DependsOn(nameof(Email))]
        [DependsOn(nameof(UserName))]
        [DependsOn(nameof(Password))]
        [DependsOn(nameof(ConfirmPassword))]
        [DependsOn(nameof(FirstName))]
        [DependsOn(nameof(LastName))]
        public bool CanRegister(object parameter)
        {
            return !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(UserName) &&
                   !string.IsNullOrEmpty(Password) &&
                   !string.IsNullOrEmpty(ConfirmPassword) &&
                   !string.IsNullOrEmpty(FirstName) &&
                   !string.IsNullOrEmpty(LastName) &&
                   Password == ConfirmPassword;
        }
    }
}