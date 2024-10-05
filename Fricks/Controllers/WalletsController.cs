using Fricks.Repository.Commons;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                    return NotFound(new ResponseModel
                    {
                        HttpCode = 404,
                        Message = "Không tìm thấy ví của cửa hàng này"
                    });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseModel
                {
                    HttpCode = 400,
                    Message = ex.Message
                });
            }
        }
        [HttpGet("store/transactions")]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> GetTransactionsWallet([FromQuery] PaginationParameter paginationParameter)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _walletService.GetTransationsWalletPaginationAsync(paginationParameter, currentEmail);
                if (result == null)
                {
                    return NotFound(new ResponseModel
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
                return BadRequest(new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

    }
}
