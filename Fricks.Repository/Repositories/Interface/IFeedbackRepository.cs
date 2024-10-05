using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IFeedbackRepository: IGenericRepository<Feedback>
    {
        public Task<bool> CheckUserFeedbackProductAsync(int productId, int userId);

        public Task<Pagination<Feedback>> GetFeedBackByProductAsync(int productId, PaginationParameter paginationParameter, FeedbackFilter feedbackFilter);
    }
}
