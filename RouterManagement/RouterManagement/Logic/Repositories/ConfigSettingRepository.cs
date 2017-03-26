using RouterManagement.Logic.Repositories.Interfaces;
using RouterManagement.Models;
using RouterManagement.Models.Context;

namespace RouterManagement.Logic.Repositories
{
    public class ConfigSettingRepository : GenericRepository<ConfigSetting, RouterManagementEntities>, IConfigSettingRepository
    {
        public ConfigSettingRepository(RouterManagementEntities entities) : base(entities)
        {
        }
    }
}