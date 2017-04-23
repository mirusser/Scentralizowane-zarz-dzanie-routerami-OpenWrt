using System;
using System.Data.Entity;

namespace RouterManagement.Logic.Repositories.Interfaces
{
    public interface IUoW : IDisposable
    {
        DbContextTransaction BeginTransaction();

        void Save();
    }
}
