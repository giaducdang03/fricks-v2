using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task<TEntity> AddAsync(TEntity entity);
        void UpdateAsync(TEntity entity);
        void SoftDeleteAsync(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);
        void SoftDeleteRangeAsync(List<TEntity> entities);
        void PermanentDeletedAsync(TEntity entity);
        void PermanentDeletedListAsync(List<TEntity> entities);
        Task<Pagination<TEntity>> ToPagination(PaginationParameter paginationParameter);
    }
}
