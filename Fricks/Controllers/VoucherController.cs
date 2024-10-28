using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.VoucherModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fricks.Controllers
{
    [Route("api/vouchers")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private IVoucherService _voucherService;
        private IClaimsService _claimsService;
        public VoucherController(IVoucherService voucherService, IClaimsService claimsService)
        {
            _voucherService = voucherService;
            _claimsService = claimsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int storeId, [FromQuery] PaginationParameter paginationParameter) 
        {
            try
            { 
                var result = await _voucherService.GetAllVoucherByStoreId(storeId, paginationParameter);
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _voucherService.GetById(id);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPost]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> Add(VoucherProcessModel voucherProcessModel)
        {
            try
            {
                var email = _claimsService.GetCurrentUserEmail;
                var result = await _voucherService.AddVoucher(voucherProcessModel, email);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPut]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> Update(int id, VoucherProcessModel voucherProcessModel) 
        {
            try
            {
                var email = _claimsService.GetCurrentUserEmail;
                var result = await _voucherService.UpdateVoucher(id, voucherProcessModel, email);
                return Ok(result);
            } catch { throw; }
        }

        [HttpDelete]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var email = _claimsService.GetCurrentUserEmail;
                var result = await _voucherService.DeleteVoucher(id, email);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPatch]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> Disable(int id)
        {
            try
            {
                var email = _claimsService.GetCurrentUserEmail;
                var result = await _voucherService.DisableVoucher(id, email);
                return Ok(result);
            } catch { throw; }
        }
    }
}
