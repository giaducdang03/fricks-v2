using Fricks.Repository.Entities;
using Fricks.Repository.Repositories;
using Fricks.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FricksContext _context;
        private IDbContextTransaction _transaction;
        private IGenericRepository<User> _userRepository;

        public UnitOfWork(FricksContext context) 
        { 
            _context = context;
        }
        public IGenericRepository<User> UsersRepository
        {
            get
            {
                return _userRepository ??= new GenericRepository<User>(_context);

            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _transaction?.Commit();
            }
            catch (Exception)
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
            }
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
