using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.OrderModels;
using Microsoft.AspNetCore.Http;
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
        public Task<CreatePaymentResult> RequestPaymentOrderAsync(PaymentOrderModel paymentOrderModel, HttpContext httpContext);
        public Task<Pagination<OrderModel>> GetOrderByUserEmail(string email);
        public Task<Pagination<OrderModel>> GetOrderByStoreId(int storeId);
        public Task<CreatePaymentResult> ConfirmOrderAsync(ConfirmOrderModel orderModel, string email);
        public Task<bool> CancelOrderAsync(ConfirmOrderModel orderModel, string email);
        public Task<CalculateOrderModel> CalculateOrderAsync(CreateOrderModel orderModel);
    }
}
