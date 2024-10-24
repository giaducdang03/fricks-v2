using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly FricksContext _context;

        public OrderDetailRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<OrderDetail>> GetAllOrderDetails()
        {
            return await _context.OrderDetails.Include(x => x.Order).Include(x => x.Product).ToListAsync();
        }
    }
}
