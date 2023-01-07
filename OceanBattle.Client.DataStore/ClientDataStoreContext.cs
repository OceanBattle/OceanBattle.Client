using Microsoft.EntityFrameworkCore;

namespace OceanBattle.Client.DataStore
{
    public class ClientDataStoreContext : DbContext
    {       
        public ClientDataStoreContext(DbContextOptions<ClientDataStoreContext> options)
            : base(options)
        {
        }

        public DbSet<LogInData> LogInData { get; set; }
        public DbSet<Settings> Settings { get; set; }
    }
}
