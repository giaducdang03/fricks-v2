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
    public interface IPostRepository: IGenericRepository<Post>
    {
        public Task<Post> GetPostByIdAsync(int id);

        public Task<Pagination<Post>> GetPostPaging(PaginationParameter paginationParameter, PostFilter postFilter);
    }
}
