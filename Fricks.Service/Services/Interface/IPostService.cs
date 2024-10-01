using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.PostModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IPostService
    {
        public Task<PostModel> AddPostAsync(CreatePostModel postModel);

        public Task<PostModel> UpdatePostAsync(UpdatePostModel updatePostModel);

        public Task<Post> DeletePostAsync(int id);

        public Task<PostModel> GetPostByIdAsync(int id);

        public Task<Pagination<PostModel>> GetPostPaginationAsync(PaginationParameter paginationParameter, PostFilter postFilter);
    }
}
