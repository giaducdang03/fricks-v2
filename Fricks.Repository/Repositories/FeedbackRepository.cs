using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories
{
    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        private readonly FricksContext _context;

        public FeedbackRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckUserFeedbackProductAsync(int productId, int userId)
        {
            var userFeedback = await _context.Feedbacks.FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId && x.IsDeleted == false);
            if (userFeedback == null)
            {
                return true;
            }
            return false;
        }

        public async Task<Pagination<Feedback>> GetFeedBackByProductAsync(int productId, PaginationParameter paginationParameter, FeedbackFilter feedbackFilter)
        {
            var query = _context.Feedbacks.Include(x => x.Product)
                                    .Where(x => x.ProductId == productId && x.IsDeleted == false).AsQueryable();

            // apply filter
            query = ApplyFeedbackFiltering(query, feedbackFilter);

            var itemCount = await query.CountAsync();
            var items = await query.Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Feedback>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        private IQueryable<Feedback> ApplyFeedbackFiltering(IQueryable<Feedback> query, FeedbackFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "rate":
                        query = filter.Dir?.ToLower() == "desc" ? query.OrderByDescending(s => s.Rate) : query.OrderBy(s => s.Rate);
                        break;
                    default:
                        query = query.OrderBy(s => s.Id);
                        break;
                }
            }

            return query;
        }
    }
}
