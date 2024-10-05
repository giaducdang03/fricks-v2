using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Settings;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOSSetting _payOSSetting;
        public PaymentService(IOptions<PayOSSetting> payOSSetting)
        {
            _payOSSetting = payOSSetting.Value;
        }
        public async Task<CreatePaymentResult> CreatePaymentLink(List<ItemData> listProduct, int totalPrice)
        {
            PayOS payOs = new PayOS(_payOSSetting.ClientId, _payOSSetting.ApiKey, _payOSSetting.ChecksumKey);
            PaymentData paymentData = new PaymentData(
                BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0),
                totalPrice,
                $"Thanh toan don hang",
                listProduct,
                _payOSSetting.ReturnUrl,
                _payOSSetting.CancelUrl
            );

            return await payOs.createPaymentLink(paymentData);
        }
    }
}
