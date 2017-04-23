using RouterManagement.Models;

namespace RouterManagement.Logic.Repositories.Interfaces
{
    public interface IDataContextUoW : IUoW
    {
        IGenericRepository<CommonSetting> CommonSettingsRepository { get; }
        IGenericRepository<ConfigSetting> ConfigSettingsRepository { get; }
        IGenericRepository<RouterAccesData> RouterAccesDatasRepository { get; }
    }
}
