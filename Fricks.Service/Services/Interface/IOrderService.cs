using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.OrderModels;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IOrderService 
    {
        public Task<CreatePaymentResult> AddOrder(OrderProcessModel orderProcessModel);
        public Task<Pagination<OrderModel>> GetOrderByUserEmail(string email);
        public Task<Pagination<OrderModel>> GetOrderByStoreId(int storeId);
    }
}
