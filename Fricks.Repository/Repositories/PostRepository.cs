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
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private FricksContext _context;

        public PostRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _context.Posts.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Pagination<Post>> GetPostPaging(PaginationParameter paginationParameter, PostFilter postFilter)
        {
            var query = _context.Posts.Where(x => x.IsDeleted == false).Include(x => x.Product).AsQueryable();

            // apply filter
            query = ApplyFiltering(query, postFilter);

            var itemCount = await query.CountAsync();
            var items = await query.Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Post>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);

            return result;
        }

        public IQueryable<Post> ApplyFiltering(IQueryable<Post> query, PostFilter filter)
        {
            if (filter.StoreId != null && filter.StoreId > 0)
            {
                query = query.Where(s => s.Product.StoreId == filter.StoreId);
            }

            if (filter.ProductId != null && filter.ProductId > 0)
            {
                query = query.Where(s => s.ProductId == filter.ProductId);
            }

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => s.Title.Contains(filter.Title));
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "date":
                        query = filter.Dir?.ToLower() == "desc" ? query.OrderByDescending(s => s.CreateDate) : query.OrderBy(s => s.CreateDate);
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
