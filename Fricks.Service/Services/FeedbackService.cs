using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Service.BusinessModel.FeedbackModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        public Task<FeedbackModel> CreateFeedbackAsync(CreateFeedbackModel createFeedbackModel)
        {
            throw new NotImplementedException();
        }

        public Task<FeedbackModel> DeleteFeedbackAsync(int feedbackId, string email)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<FeedbackModel>> GetFeedbackProductPaginationAsync(int productId, PaginationParameter paginationParameter, FeedbackFilter feedbackFilter)
        {
            throw new NotImplementedException();
        }

        public Task<FeedbackModel> UpdateFeedbackAsync(UpdateFeedbackModel updateFeedbackModel)
        {
            throw new NotImplementedException();
        }
    }
}
