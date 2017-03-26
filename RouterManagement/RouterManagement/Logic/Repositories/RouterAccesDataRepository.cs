using RouterManagement.Logic.Repositories.Interfaces;
using RouterManagement.Models;
using RouterManagement.Models.Context;

namespace RouterManagement.Logic.Repositories
{
    public class RouterAccesDataRepository : GenericRepository<RouterAccesData, RouterManagementEntities>, IRouterAccesDataRepository
    {
        public RouterAccesDataRepository(RouterManagementEntities entities) : base(entities)
        {
        }
    }
}