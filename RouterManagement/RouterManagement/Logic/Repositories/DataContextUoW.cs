using System;
using System.Data.Entity;
using RouterManagement.Logic.Repositories.Interfaces;
using RouterManagement.Models;
using RouterManagement.Models.Context;

namespace RouterManagement.Logic.Repositories
{
    public class DataContextUoW : IDataContextUoW
    {
        private readonly RouterManagementEntities ctx;
        private bool disposed = false;

        public DataContextUoW(RouterManagementEntities ctx)
        {
            this.ctx = ctx;
        }

        public IGenericRepository<CommonSetting> CommonSettingsRepository => new GenericRepository<CommonSetting, RouterManagementEntities>(ctx);
        public IGenericRepository<ConfigSetting> ConfigSettingsRepository => new GenericRepository<ConfigSetting, RouterManagementEntities>(ctx);
        public IGenericRepository<RouterAccesData> RouterAccesDatasRepository => new GenericRepository<RouterAccesData, RouterManagementEntities>(ctx);

        private IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new GenericRepository<TEntity, RouterManagementEntities>(ctx);
        }

        public DbContextTransaction BeginTransaction()
        {
            return ctx.Database.BeginTransaction();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            ctx.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ctx.Dispose();
                }
            }
            disposed = true;
        }
    }
}