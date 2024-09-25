using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IProductPriceRepository : IGenericRepository<ProductPrice>
    {
        public Task<List<ProductPrice>> GetByProductId(int id);
    }
}
