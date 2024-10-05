using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Service.BusinessModel.FeedbackModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IFeedbackService
    {
        public Task<Pagination<FeedbackModel>> GetFeedbackProductPaginationAsync(int productId, PaginationParameter paginationParameter, 
            FeedbackFilter feedbackFilter);
        
        public Task<FeedbackModel> CreateFeedbackAsync(CreateFeedbackModel createFeedbackModel);

        public Task<FeedbackModel> UpdateFeedbackAsync(UpdateFeedbackModel updateFeedbackModel);

        public Task<FeedbackModel> DeleteFeedbackAsync(int feedbackId, string email);
    }
}
