using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories
{
    public class ProductUnitRepository : GenericRepository<ProductUnit>, IProductUnitRepository
    {
        private FricksContext _context;
        public ProductUnitRepository(FricksContext context) : base(context)
        {
            _context = context;
        }
    }
}
