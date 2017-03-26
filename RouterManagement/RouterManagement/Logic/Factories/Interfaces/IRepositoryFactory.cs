using RouterManagement.Models.Context;

namespace RouterManagement.Logic.Factories.Interfaces
{
    public interface IRepositoryFactory : IFactory
    {
        T Get<T>(RouterManagementEntities context);
    }
}