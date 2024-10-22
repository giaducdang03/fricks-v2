using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Service.BusinessModel.WalletModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Fricks.Controllers
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IClaimsService _claimsService;

        public WalletsController(IWalletService walletService, IClaimsService claimsService) 
        {
            _walletService = walletService;
            _claimsService = claimsService;
        }

        [HttpGet("store")]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> GetAccountById()
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var data = await _walletService.GetWalletStoreAsync(currentEmail);
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        HttpCode = 404,
                        Message = "Không tìm thấy ví của cửa hàng này"
                    });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }
        [HttpGet("store/transactions")]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> GetTransactionsWallet([FromQuery] PaginationParameter paginationParameter, [FromQuery] TransactionFilter transactionFilter)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _walletService.GetTransationsWalletPaginationAsync(paginationParameter, currentEmail, transactionFilter);
                if (result == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status404NotFound,
                        Message = "Không có giao dịch"
                    });
                }
                var metadata = new
                {
                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("withdraw/request")]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> RequestWithdrawStore(CreateWithdrawModel createWithdrawModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentEmail = _claimsService.GetCurrentUserEmail;
                    var result = await _walletService.RequestWithdrawStoreAsync(createWithdrawModel, currentEmail);
                    return Ok(result);
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPut("withdraw/confirm")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ConfirmWithdrawStore(UpdateWithdrawModel withdrawModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentEmail = _claimsService.GetCurrentUserEmail;
                    var result = await _walletService.ConfirmWithdrawStoreAsync(withdrawModel, currentEmail);
                    return Ok(result);
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPut("withdraw/process")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ProcessWithdrawStore(UpdateWithdrawModel withdrawModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _walletService.ProcessWithdrawStoreAsync(withdrawModel);
                    return Ok(result);
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

    }
}
