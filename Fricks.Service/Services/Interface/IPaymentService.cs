using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.OrderDetailModels;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.BusinessModel.ProductModels;
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
        public Task<CreatePaymentResult> CreatePaymentLinkOrder(int totalPrice, Order order);
    }
}
