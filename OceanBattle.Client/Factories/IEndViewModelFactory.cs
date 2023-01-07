using OceanBattle.Client.ViewModels;

namespace OceanBattle.Client.Factories
{
    public interface IEndViewModelFactory
    {
        EndViewModel Create(ViewModelBase next);
    }
}
