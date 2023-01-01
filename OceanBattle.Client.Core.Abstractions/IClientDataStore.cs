using OceanBattle.Client.DataStore;

namespace OceanBattle.Client.Core.Abstractions
{
    public interface IClientDataStore
    {
        Task<bool> HasLogInDataAsync();
        Task<bool> HasSettingsAsync();
        Task EnsureDataStoreCreatedAsync();
        Task EnsureDataStoreDeletedAsync();
        Task<LogInData?> GetLogInDataAsync();
        Task<Settings?> GetSettingsAsync();
        Task SaveLogInDataAsync(LogInData logInData);
        Task SaveSettingsAsync(Settings settings);
    }
}
