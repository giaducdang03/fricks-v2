using AutoMapper;
using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Settings;
using Fricks.Service.Utils;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Fricks.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOSSetting _payOSSetting;
        private readonly IMapper _mapper;
        public PaymentService(IOptions<PayOSSetting> payOSSetting, IMapper mapper)
        {
            _payOSSetting = payOSSetting.Value;
            _mapper = mapper;
        }
        public async Task<CreatePaymentResult> CreatePaymentLink(List<ItemData> listProduct, int totalPrice)
        {
            PayOS payOs = new PayOS(_payOSSetting.ClientId, _payOSSetting.ApiKey, _payOSSetting.ChecksumKey);
            PaymentData paymentData = new PaymentData(
                (new Random()).NextInt64(0, GetRangeLong()),
                totalPrice,
                $"Thanh toán đơn hàng",
                listProduct,
                _payOSSetting.ReturnUrl,
                _payOSSetting.CancelUrl
            );

            return await payOs.createPaymentLink(paymentData);
        }

        public async Task<CreatePaymentResult> CreatePaymentLink(List<ItemData> listProduct, int totalPrice, Order order)
        {
            PayOS payOs = new PayOS(_payOSSetting.ClientId, _payOSSetting.ApiKey, _payOSSetting.ChecksumKey);
            PaymentData paymentData = new PaymentData(
                (new Random()).NextInt64(0, GetRangeLong()),
                totalPrice,
                $"Thanh toán đơn hàng {order.Code}",
                listProduct,
                _payOSSetting.ReturnUrl,
                _payOSSetting.CancelUrl
            );

            return await payOs.createPaymentLink(paymentData);
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkOrder(int totalPrice, Order order)
        {
            PayOS payOs = new PayOS(_payOSSetting.ClientId, _payOSSetting.ApiKey, _payOSSetting.ChecksumKey);
            var listProduct = _mapper.Map<List<ItemData>>(order.OrderDetails);
            PaymentData paymentData = new PaymentData(
                order.OrderCode,
                totalPrice,
                $"Thanh toán đơn hàng",
                listProduct,
                _payOSSetting.ReturnUrl,
                _payOSSetting.CancelUrl
            );

            return await payOs.createPaymentLink(paymentData);
        }

        private long GetRangeLong()
        {
            return (long)Math.Pow(2, 53) - 1;
        }
    }
}
