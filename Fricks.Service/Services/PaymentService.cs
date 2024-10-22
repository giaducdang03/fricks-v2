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
using Fricks.Service.BusinessModel.PaymentModels;
using System.Reflection;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;

namespace Fricks.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOSSetting _payOSSetting;
        private readonly VnpaySetting _vnpaySetting;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IOptions<PayOSSetting> payOSSetting, IMapper mapper, 
            IOptions<VnpaySetting> vnpaySetting, 
            IUnitOfWork unitOfWork)
        {
            _payOSSetting = payOSSetting.Value;
            _vnpaySetting = vnpaySetting.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> ConfirmPayOSPayment(PayOSResponseModel payOSResponse)
        {
            if(payOSResponse == null)
            {
                throw new Exception("Có lỗi trong quá trình thanh toán");
            }
            // get order id
            int orderId = 0;
            _ = int.TryParse(payOSResponse.orderCode, out orderId);
            PayOS payOs = new PayOS(_payOSSetting.ClientId, _payOSSetting.ApiKey, _payOSSetting.ChecksumKey);
            PaymentLinkInformation paymentLinkInformation = await payOs.getPaymentLinkInformation(orderId);
            //Cái payOS có vụ thanh toán thiếu ::)))))))
            //Lý do nó đéo gửi hết data về 1 lượt là vậy á
            bool flag = false;
            //Do là giao dịch chuyển khoản nên PayOS ko có track ngân hàng đầu vào, ko lấy đc bank
            var paymentConfirm = new ConfirmPaymentModel
            {
                TransactionNo = payOSResponse.id,
                PaymentStatus = !payOSResponse.cancel ? PaymentStatus.PAID : PaymentStatus.FAILED,
            };
            flag = await ConfirmPaymentOrderAsync(orderId, paymentConfirm);
            return flag;
        }

        public async Task<bool> ConfirmVnpayPayment(VnPayModel vnPayResponse)
        {
            // get info transaction
            if (vnPayResponse != null)
            {
                // check signature
                var vnpay = new VnPayLibrary();

                // get all data from vnpay model
                foreach (PropertyInfo prop in vnPayResponse.GetType().GetProperties())
                {
                    string name = prop.Name;
                    object value = prop.GetValue(vnPayResponse, null);
                    string valueStr = value?.ToString() ?? string.Empty;
                    vnpay.AddResponseData(name, valueStr);
                }

                // get order id
                int orderId = 0;
                _ = int.TryParse(vnPayResponse.vnp_TxnRef, out orderId);

                var vnpayHashSecret = _vnpaySetting.HashSecret;
                bool validateSignature = vnpay.ValidateSignature(vnPayResponse.vnp_SecureHash, vnpayHashSecret);
                if (validateSignature)
                {
                    if (vnPayResponse.vnp_TransactionStatus == "00")
                    {
                        var paymentConfirm = new ConfirmPaymentModel
                        {
                            BankCode = vnPayResponse.vnp_BankCode,
                            BankTranNo = vnPayResponse.vnp_BankTranNo,
                            TransactionNo = vnPayResponse.vnp_TransactionNo,
                            PaymentStatus = PaymentStatus.PAID
                        };

                        return await ConfirmPaymentOrderAsync(orderId, paymentConfirm);
                    }
                    else
                    {
                        var paymentConfirm = new ConfirmPaymentModel
                        {
                            BankCode = vnPayResponse.vnp_BankCode,
                            BankTranNo = vnPayResponse.vnp_BankTranNo,
                            TransactionNo = vnPayResponse.vnp_TransactionNo,
                            PaymentStatus = PaymentStatus.FAILED
                        };

                        return await ConfirmPaymentOrderAsync(orderId, paymentConfirm);
                    }
                }
                throw new Exception("Chữ ký không hợp lệ");
            }
            throw new Exception("Có lỗi trong quá trình thanh toán");
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
            try
            {
                PayOS payOs = new PayOS(_payOSSetting.ClientId, _payOSSetting.ApiKey, _payOSSetting.ChecksumKey);
                var insertedOrder = await _unitOfWork.OrderRepository.GetOrderById(order.Id);
                if (insertedOrder == null)
                {
                    throw new Exception("Có lỗi trong quá trình tạo đơn hàng");
                }

                List<ItemData> listProducts = new List<ItemData>();
                foreach (var item in insertedOrder.OrderDetails)
                {
                    var itemData = new ItemData(item.Product.Name, item.Quantity.Value, item.Price.Value);
                    listProducts.Add(itemData);
                }

                PaymentData paymentData = new PaymentData(
                    order.Id,
                    totalPrice,
                    $"Thanh toán đơn hàng",
                    listProducts,
                    _payOSSetting.CancelUrl,
                    _payOSSetting.ReturnUrl
                );

                return await payOs.createPaymentLink(paymentData);
            }
            catch
            {
                var insertedOrder = await _unitOfWork.OrderRepository.GetOrderById(order.Id);
                if (insertedOrder != null)
                {
                    _unitOfWork.OrderRepository.PermanentDeletedAsync(insertedOrder);
                }
                throw;
            }
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

            var createPaymentResult = new CreatePaymentResult("", "VNPAY", order.Total.Value, $"Thanh toan cho don hang {order.Code}", order.Id, "VND", "", "", paymentUrl, "");

            return createPaymentResult;
        }

        private async Task<bool> ConfirmPaymentOrderAsync(int orderId, ConfirmPaymentModel confirmPayment)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderById(orderId);
            if (order != null)
            {
                if (order.Status == OrderStatus.PENDING.ToString()
                    && order.PaymentStatus == PaymentStatus.PENDING.ToString())
                {
                    var storeWallet = await _unitOfWork.WalletRepository.GetWalletStoreAsync(order.StoreId.Value);
                    if (storeWallet == null)
                    {
                        throw new Exception("Không tìm thấy ví của cửa hàng");
                    }

                    // calculate commission
                    // default 5% on order
                    double commission = order.Total.Value * 0.05;
                    double transactionAmount = order.Total.Value - commission;

                    if (confirmPayment.PaymentStatus == PaymentStatus.PAID)
                    {
                        // create transaction
                        var newTransaction = new Repository.Entities.Transaction
                        {
                            WalletId = storeWallet.Id,
                            TransactionType = TransactionType.IN.ToString(),
                            Amount = (int)transactionAmount,
                            Description = $"Thanh toán cho đơn hàng {order.Code}",
                            Status = TransactionStatus.SUCCESS.ToString()
                        };

                        // update store wallet
                        storeWallet.Balance += (decimal)transactionAmount;
                        _unitOfWork.WalletRepository.UpdateAsync(storeWallet);

                        // update product stock
                        var orderDetails = order.OrderDetails;
                        List<Product> updateProducts = new List<Product>();

                        foreach (var item in orderDetails)
                        {
                            var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId.Value);
                            if (product != null)
                            {
                                if (product.Quantity >= item.Quantity)
                                {
                                    product.SoldQuantity += item.Quantity;
                                    product.Quantity -= item.Quantity;
                                    updateProducts.Add(product);
                                }
                            }
                        }

                        if (updateProducts.Count > 0)
                        {
                            _unitOfWork.ProductRepository.UpdateRangeProductAsync(updateProducts);
                        }

                        // update order
                        order.Status = OrderStatus.SUCCESS.ToString();
                        order.PaymentStatus = PaymentStatus.PAID.ToString();
                        order.TransactionNo = confirmPayment.TransactionNo;
                        order.BankTranNo = confirmPayment.BankTranNo;
                        order.BankCode = confirmPayment.BankCode;
                        order.PaymentDate = CommonUtils.GetCurrentTime();

                        await _unitOfWork.TransactionRepository.AddAsync(newTransaction);
                        _unitOfWork.OrderRepository.UpdateAsync(order);
                        _unitOfWork.Save();

                        return true;
                    }
                    else
                    {
                        // update order
                        order.Status = OrderStatus.ERROR.ToString();
                        order.PaymentStatus = PaymentStatus.FAILED.ToString();
                        order.PaymentDate = CommonUtils.GetCurrentTime();

                        _unitOfWork.OrderRepository.UpdateAsync(order);
                        _unitOfWork.Save();

                        return false;
                    }
                }
                // payos call api two times
                else if (order.Status == OrderStatus.SUCCESS.ToString()
                    && order.PaymentStatus == PaymentStatus.PAID.ToString() && confirmPayment.PaymentStatus == PaymentStatus.PAID)
                {
                    return true;
                }
                throw new Exception("Không thể cập nhật trạng thái đơn hàng");
            }
            throw new Exception("Đơn hàng không tồn tại");
        }

        private long GetRangeLong()
        {
            return (long)Math.Pow(2, 53) - 1;
        }
    }
}
