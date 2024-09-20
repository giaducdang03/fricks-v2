using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UsersRepository { get; }
        int Save();
        void Commit();
        void Rollback();
    }
}
