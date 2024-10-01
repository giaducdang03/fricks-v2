using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.PostModels;
using Fricks.Service.BusinessModel.UserModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PostModel> AddPostAsync(CreatePostModel postModel)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(postModel.ProductId);
            if (product == null)
            {
                throw new Exception("Sản phẩm trong bài viết không tồn tại");
            }
            var post = _mapper.Map<Post>(postModel);
            await _unitOfWork.PostRepository.AddAsync(post);
            _unitOfWork.Save();
            return _mapper.Map<PostModel>(post);
        }

        public async Task<Post> DeletePostAsync(int id)
        {
            var deletePost = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (deletePost != null && deletePost.IsDeleted == false)
            {
                _unitOfWork.PostRepository.SoftDeleteAsync(deletePost);
                _unitOfWork.Save();
                return deletePost;
            }
            throw new Exception("Bài viết không tồn tại");
        }

        public async Task<PostModel> GetPostByIdAsync(int id)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(id);
            if (post != null && post.IsDeleted == false)
            {
                return _mapper.Map<PostModel>(post);
            }
            return null;
        }

        public async Task<Pagination<PostModel>> GetPostPaginationAsync(PaginationParameter paginationParameter, PostFilter postFilter)
        {
            var posts = await _unitOfWork.PostRepository.GetPostPaging(paginationParameter, postFilter);
            var postModels = _mapper.Map<List<PostModel>>(posts);
            return new Pagination<PostModel>(postModels,
                posts.TotalCount,
                posts.CurrentPage,
                posts.PageSize);
        }

        public async Task<PostModel> UpdatePostAsync(UpdatePostModel updatePostModel)
        {
            var updatePost = await _unitOfWork.PostRepository.GetPostByIdAsync(updatePostModel.Id);
            if (updatePost != null)
            {
                updatePost.Title = updatePostModel.Title;
                updatePost.Content = updatePostModel.Content;
                updatePost.ProductId = updatePostModel.ProductId;
                updatePost.Image = updatePostModel.Image;

                _unitOfWork.PostRepository.UpdateAsync(updatePost);
                _unitOfWork.Save();
                return _mapper.Map<PostModel>(updatePost);
            }
            throw new Exception("Bài viết không tồn tại");
        }
    }
}
