using OceanBattle.Client.Factories;
using ReactiveUI;

namespace OceanBattle.Client.ViewModels
{
    public delegate void ViewChanger(ViewModelBase viewModel);

    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase? _content;
        public ViewModelBase? Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public MainWindowViewModel(
            ILogInViewModelFactory logInViewModelFactory)
        {
            Content = logInViewModelFactory.Create();
        }
    }
}