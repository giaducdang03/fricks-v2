using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IStoreRepository : IGenericRepository<Store>
    {
        public Task<Pagination<Store>> GetStorePaging(PaginationParameter paginationParameter);
        public Task<Pagination<Store>> GetStoreByManagerIdPaging(PaginationParameter paginationParameter, int id);
    }
}
