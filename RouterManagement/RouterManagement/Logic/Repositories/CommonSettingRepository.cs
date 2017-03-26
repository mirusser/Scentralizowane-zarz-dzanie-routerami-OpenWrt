using RouterManagement.Logic.Repositories.Interfaces;
using RouterManagement.Models;
using RouterManagement.Models.Context;

namespace RouterManagement.Logic.Repositories
{
    public class CommonSettingRepository : GenericRepository<CommonSetting, RouterManagementEntities>, ICommonSettingRepository
    {
        public CommonSettingRepository(RouterManagementEntities entities) : base(entities)
        {
        }
    }
}