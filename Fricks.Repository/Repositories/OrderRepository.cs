using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Fricks.Repository.Repositories
{
    internal class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private FricksContext _context;
        public OrderRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pagination<Order>> GetOrderPaging(int? userId, PaginationParameter paginationParameter, OrderFilter orderFilter)
        {
            var query = _context.Orders.Include(x => x.Store).Where(x => x.IsDeleted == false).AsQueryable();

            // apply filter
            query = ApplyOrderFiltering(query, orderFilter, userId);

            var itemCount = await _context.Orders.CountAsync();
            var items = await query.Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Order>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _context.Orders.Include(x => x.Store).Include(x => x.OrderDetails).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
        }

        private IQueryable<Order> ApplyOrderFiltering(IQueryable<Order> query, OrderFilter filter, int? userId)
        {
            if (userId != null && userId > 0)
            {
                query = query.Where(s => s.UserId == userId);
            }

            if (filter.StoreId != null && filter.StoreId > 0)
            {
                query = query.Where(s => s.StoreId == filter.StoreId);
            }

            if (filter.OrderStatus != null)
            {
                query = query.Where(s => s.Status == filter.OrderStatus.ToString());
            }

            if (filter.PaymentStatus != null)
            {
                query = query.Where(s => s.PaymentStatus == filter.PaymentStatus.ToString());
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "total":
                        query = filter.Dir?.ToLower() == "desc" ? query.OrderByDescending(s => s.Total) : query.OrderBy(s => s.Total);
                        break;
                    case "date":
                        query = filter.Dir?.ToLower() == "desc" ? query.OrderByDescending(s => s.CreateDate) : query.OrderBy(s => s.CreateDate);
                        break;
                    default:
                        query = query.OrderBy(s => s.Id);
                        break;
                }
            }

            return query;
        }
    }
}
