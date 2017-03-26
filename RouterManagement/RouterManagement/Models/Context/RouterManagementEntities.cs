using System.Data.Entity;

namespace RouterManagement.Models.Context
{
    public class RouterManagementEntities : ApplicationDbContext
    {
        public DbSet<CommonSetting> CommonSettings { get; set; }
        public DbSet<ConfigSetting> ConfigSettings { get; set; }
        public DbSet<RouterAccesData> RouterAccesDatas { get; set; }
    }
}