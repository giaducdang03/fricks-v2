using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Service.BusinessModel.FeedbackModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fricks.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IClaimsService _claimsService;

        public FeedbacksController(IFeedbackService feedbackService, IClaimsService claimsService) 
        {
            _feedbackService = feedbackService;
            _claimsService = claimsService;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetFeedbacksProductAsync(int productId, 
            [FromQuery] PaginationParameter paginationParameter, [FromQuery] FeedbackFilter feedbackFilter)
        {
            try
            {
                var result = await _feedbackService.GetFeedbackProductPaginationAsync(productId, paginationParameter, feedbackFilter);
                if (result == null)
                {
                    return NotFound(new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status404NotFound,
                        Message = "Không có đánh giá"
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
                return BadRequest(
                    new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = ex.Message.ToString()
                    }
               );
            }
        }

        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddFeedbackProductAsync(CreateFeedbackModel createFeedbackModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentEmail = _claimsService.GetCurrentUserEmail;
                    var result = await _feedbackService.CreateFeedbackAsync(createFeedbackModel, currentEmail);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    return BadRequest(new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = "Có lỗi trong quá trình gửi đánh giá. Thử lại sau."
                    });
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var resp = new ResponseModel()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPut]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateFeedbackProductAsync(UpdateFeedbackModel updateFeedbackModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentEmail = _claimsService.GetCurrentUserEmail;
                    var result = await _feedbackService.UpdateFeedbackAsync(updateFeedbackModel, currentEmail);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    return BadRequest(new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = "Có lỗi trong quá trình gửi đánh giá. Thử lại sau."
                    });
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var resp = new ResponseModel()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFeedbackProductAsync(int id)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _feedbackService.DeleteFeedbackAsync(id, currentEmail);
                if (result != null)
                {
                    return Ok(new ResponseModel
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = $"Xóa đánh giá thành công."
                    });
                }
                return BadRequest(new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Có lỗi trong quá trình xóa đánh giá."
                });
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }
    }
}
