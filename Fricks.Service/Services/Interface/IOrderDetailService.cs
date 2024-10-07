using Fricks.Service.BusinessModel.OrderDetailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IOrderDetailService
    {
        public Task<List<OrderDetailModel>> GetByOrderId(int orderId);
        public Task<List<OrderDetailProcessModel>> CreateOrderDetail(List<OrderDetailProcessModel> orderDetail);
    }
}
