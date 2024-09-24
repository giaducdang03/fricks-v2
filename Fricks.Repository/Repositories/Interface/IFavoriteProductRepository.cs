using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IFavoriteProductRepository : IGenericRepository<FavoriteProduct>
    {
        public Task<Pagination<FavoriteProduct>> GetFavoriteProductPaging(PaginationParameter paginationParameter);
    }
}
