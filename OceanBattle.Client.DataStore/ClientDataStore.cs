using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OceanBattle.Client.Core.Abstractions;
using System.Reflection.Metadata.Ecma335;

namespace OceanBattle.Client.DataStore
{
    /// <summary>
    /// Store for saving data on users machine
    /// </summary>
    public class ClientDataStore : IClientDataStore
    {
        /// <summary>
        /// Local instance of ClientDataStoreContext DbContext.
        /// </summary>
        private readonly ClientDataStoreContext _dbContext;

        /// <summary>
        /// Local instance of logger
        /// </summary>
        private readonly ILogger<ClientDataStore> _logger;

        /// <summary>
        /// Local instance of OS specific data protector do encrypt and decrypt secret data.
        /// </summary>
        private readonly IDataProtector _dataProtector;

        /// <summary>
        /// ClientDataStore's constructor
        /// </summary>
        /// <param name="dbContext">Instance of ClientDataStoreContext DbContext</param>
        /// <param name="logger">Instance of logger</param>
        /// <param name="dataProtectionProvider">Instance of DataProtectionProvider</param>
        public ClientDataStore(
            ClientDataStoreContext dbContext, 
            IDataProtectionProvider dataProtectionProvider,
            ILogger<ClientDataStore> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _dataProtector = dataProtectionProvider.CreateProtector("OceanBattle.Client");
        }

        /// <summary>
        /// Checks existance of LogIn data.
        /// </summary>
        /// <returns>bool result of search for LogIn data.</returns>
        public async Task<bool> HasLogInDataAsync() 
            => (await GetLogInDataAsync()) != null;

        /// <summary>
        /// Checks existance of Settings.
        /// </summary>
        /// <returns>bool reesult of search for settings.</returns>
        public async Task<bool> HasSettingsAsync() 
            => (await GetSettingsAsync()) != null;

        /// <summary>
        /// Ensures that local database has been created, if it has not, creates one, if it has been, does nothing.
        /// </summary>
        /// <returns>Task</returns>
        public async Task EnsureDataStoreCreatedAsync() 
            => await _dbContext.Database.EnsureCreatedAsync();

        /// <summary>
        /// Ensures that local database has been deleted, if it has not, deletes one, if it has been, does nothing.
        /// </summary>
        /// <returns>Task</returns>
        public async Task EnsureDataStoreDeletedAsync()
            => await _dbContext.Database.EnsureDeletedAsync();

        /// <summary>
        /// Gets saved LogIn data.
        /// </summary>
        /// <returns>Instance of LogIn data.</returns>
        public async Task<LogInData?> GetLogInDataAsync()
        {
            try
            {
                LogInData? logInData = await _dbContext.LogInData
                    .AsNoTracking()
                    .OrderBy(d => d.Id)
                    .FirstOrDefaultAsync();

                if (logInData is not null &&
                    logInData.RefreshToken is not null &&
                    logInData.BearerToken is not null)
                {

                    string unprotectedRefreshToken = 
                        _dataProtector.Unprotect(logInData.RefreshToken);
                    string unprotectedBearerToken = 
                        _dataProtector.Unprotect(logInData.BearerToken);

                    logInData.RefreshToken = unprotectedRefreshToken;
                    logInData.BearerToken = unprotectedBearerToken;
                }

                return logInData;
            }
            catch (Exception ex)
            {
                //await HandleDbException(ex);
                return null;//await GetLogInDataAsync();
            }
        }

        /// <summary>
        /// Gets saved settings
        /// </summary>
        /// <returns>Instance of Settings</returns>
        public async Task<Settings?> GetSettingsAsync()
        {
            try
            {
                return await _dbContext.Settings
                    .AsNoTracking()
                    .OrderBy(s => s.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                //await HandleDbException(ex);
                return null;//await GetSettingsAsync();
            }      
        }

        /// <summary>
        /// Encrypts and saves LogIn data, overwriting previous entry if it exists
        /// </summary>
        /// <param name="logInData">instance of LogIn data to be saved</param>
        /// <returns>Task</returns>
        public async Task SaveLogInDataAsync(LogInData logInData)
        {
            _dbContext.LogInData.RemoveRange(_dbContext.LogInData);

            if (logInData is not null &&
                logInData.RefreshToken is not null &&
                logInData.BearerToken is not null)
            {
                string protectedRefreshToken = 
                    _dataProtector.Protect(logInData.RefreshToken);
                string protectedBearerToken = 
                    _dataProtector.Protect(logInData.BearerToken);

                logInData.RefreshToken = protectedRefreshToken;
                logInData.BearerToken = protectedBearerToken;
            }

            await _dbContext.LogInData.AddAsync(logInData!);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Saves settings, overwriting previous entry if it exists
        /// </summary>
        /// <param name="settings">instance of Settings to be saved</param>
        /// <returns>Task</returns>
        public async Task SaveSettingsAsync(Settings settings)
        {
            _dbContext.Settings.RemoveRange(_dbContext.Settings);

            await _dbContext.Settings.AddAsync(settings);
            await _dbContext.SaveChangesAsync();
        }

        #region private helpers

        private async Task HandleDbException(SqliteException ex)
        {
            _logger.LogError(ex, ex.Message);

            await _dbContext.Database.EnsureDeletedAsync();
            await EnsureDataStoreCreatedAsync();
        }

        #endregion
    }
}
