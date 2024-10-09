using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.OrderDetailModels;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.BusinessModel.PaymentModels;
using Fricks.Service.BusinessModel.ProductModels;
using Microsoft.AspNetCore.Http;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IPaymentService
    {
        public Task<CreatePaymentResult> CreatePaymentLink(List<ItemData> listProduct, int totalPrice);
        public Task<CreatePaymentResult> CreatePaymentLink(List<ItemData> listProduct, int totalPrice, Order order);
        public Task<CreatePaymentResult> CreatePayOsLinkOrder(int totalPrice, Order order);
        public CreatePaymentResult CreateVnpayLinkOrder(Order order, HttpContext httpContext);
        public Task<bool> ConfirmVnpayPayment(VnPayModel vnPayResponse);
        public Task<bool> ConfirmPayOSPayment(PayOSResponseModel payOSResponse);
    }
}
