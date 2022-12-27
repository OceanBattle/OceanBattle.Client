using OceanBattle.Client.ViewModels;

namespace OceanBattle.Client.Factories
{
    public interface IRegisterViewModelFactory
    {
        RegisterViewModel Create(ViewModelBase next);
    }
}
