using ReactiveUI;

namespace OceanBattle.Client.ViewModels
{
	public class EndViewModel : ViewModelBase
	{
		private readonly ViewModelBase _next;
		private readonly ViewChanger _viewChanger;

		private string? _result;
		public string? Result
		{
			get => _result;
			set => this.RaiseAndSetIfChanged(ref _result, value);
		}

		public EndViewModel(
			ViewModelBase next, 
			ViewChanger viewChanger) 
		{
			_next = next;
			_viewChanger = viewChanger;
		}

		public void Won()
		{
			Result = "You won!";
		}

		public void Lost()
		{
			Result = "You lost!";
		}

		public void Close()
		{
			_viewChanger(_next);
		}
	}
}