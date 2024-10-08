using AutoMapper;
using Fricks.Repository.Entities;
using Fricks.Repository.Utils;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Settings;
using Fricks.Service.Utils.Vnpay;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Fricks.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOSSetting _payOSSetting;
        private readonly VnpaySetting _vnpaySetting;
        private readonly IMapper _mapper;
        public PaymentService(IOptions<PayOSSetting> payOSSetting, IMapper mapper, IOptions<VnpaySetting> vnpaySetting)
        {
            _payOSSetting = payOSSetting.Value;
            _vnpaySetting = vnpaySetting.Value;
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

        public async Task<CreatePaymentResult> CreatePayOsLinkOrder(int totalPrice, Order order)
        {
            PayOS payOs = new PayOS(_payOSSetting.ClientId, _payOSSetting.ApiKey, _payOSSetting.ChecksumKey);
            var listProduct = _mapper.Map<List<ItemData>>(order.OrderDetails);
            PaymentData paymentData = new PaymentData(
                order.Id,
                totalPrice,
                $"Thanh toán đơn hàng",
                listProduct,
                _payOSSetting.ReturnUrl,
                _payOSSetting.CancelUrl
            );

            return await payOs.createPaymentLink(paymentData);
        }

        public CreatePaymentResult CreateVnpayLinkOrder(Order order, HttpContext httpContext)
        {
            // create URL payment

            DateTime timeNow = DateTime.UtcNow.AddHours(7);

            int paymentPrice = order.Total.Value;

            var ipAddress = VnPayUtils.GetIpAddress(httpContext);

            var pay = new VnPayLibrary();

            pay.AddRequestData("vnp_Version", _vnpaySetting.Version);
            pay.AddRequestData("vnp_Command", _vnpaySetting.Command);
            pay.AddRequestData("vnp_TmnCode", _vnpaySetting.TmnCode);
            pay.AddRequestData("vnp_Amount", ((int)paymentPrice * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _vnpaySetting.CurrCode);
            pay.AddRequestData("vnp_IpAddr", ipAddress);
            pay.AddRequestData("vnp_Locale", _vnpaySetting.Locale);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toan cho don hang {order.Code}");
            pay.AddRequestData("vnp_OrderType", "250000");
            pay.AddRequestData("vnp_TxnRef", order.Id.ToString());

            // check server running
            if (ipAddress == "::1")
            {
                pay.AddRequestData("vnp_ReturnUrl", _vnpaySetting.UrlReturnLocal);
            }
            else
            {
                pay.AddRequestData("vnp_ReturnUrl", _vnpaySetting.UrlReturnAzure);
            }

            var paymentUrl = pay.CreateRequestUrl(_vnpaySetting.BaseUrl, _vnpaySetting.HashSecret);

            var createPaymentResult = new CreatePaymentResult("", "VNPAY", order.Total.Value, $"Thanh toan cho don hang {order.Code}", order.Id, "VND","","",paymentUrl,"");

            return createPaymentResult;
        }

        private long GetRangeLong()
        {
            return (long)Math.Pow(2, 53) - 1;
        }
    }
}
