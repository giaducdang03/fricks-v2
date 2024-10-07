using Fricks.Repository.Commons;
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
    internal class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private FricksContext _context;
        public OrderRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pagination<Order>> GetByUserId(int userId, PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Orders.CountAsync();
            var items = await _context.Orders.Where(x => x.UserId.Equals(userId))
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Order>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _context.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
