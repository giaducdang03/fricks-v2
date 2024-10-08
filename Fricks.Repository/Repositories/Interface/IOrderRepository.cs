using Fricks.Repository.Commons;
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
        public Task<Pagination<Order>> GetByUserId(int userId, PaginationParameter paginationParameter);

        public Task<Order> GetOrderById(int id);
    }
}
