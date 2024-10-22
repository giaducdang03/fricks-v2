using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        public Task<Pagination<Order>> GetOrderPaging(int? userId, PaginationParameter paginationParameter, OrderFilter orderFilter);

        public Task<Order> GetOrderById(int id);

        public Task<Order> GetOrderByPaymentCode(long paymentCode);
    }
}
