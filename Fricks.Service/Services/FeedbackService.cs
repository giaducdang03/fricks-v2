using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.FeedbackModels;
using Fricks.Service.BusinessModel.ProductModels;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<FeedbackModel> CreateFeedbackAsync(CreateFeedbackModel createFeedbackModel, string currentEmail)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(currentEmail);
            if (currentUser == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(createFeedbackModel.ProductId);
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại");
            }

            var checkUserFeedback = await _unitOfWork.FeedbackRepository.CheckUserFeedbackProductAsync(createFeedbackModel.ProductId, currentUser.Id);
            if (!checkUserFeedback)
            {
                var newFeedback = _mapper.Map<Feedback>(createFeedbackModel);
                newFeedback.UserId = currentUser.Id;

                await _unitOfWork.FeedbackRepository.AddAsync(newFeedback);
                _unitOfWork.Save();

                return _mapper.Map<FeedbackModel>(newFeedback);
            }

            return null;
        }

        public async Task<FeedbackModel> DeleteFeedbackAsync(int feedbackId, string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            var deleteFeedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(feedbackId);
            if (deleteFeedback == null)
            {
                throw new Exception("Đánh giá không tồn tại");
            }

            if (currentUser.Role.ToUpper() == RoleEnums.CUSTOMER.ToString().ToUpper())
            {
                if (deleteFeedback.UserId == currentUser.Id)
                {
                    _unitOfWork.FeedbackRepository.SoftDeleteAsync(deleteFeedback);
                    _unitOfWork.Save();

                    return _mapper.Map<FeedbackModel>(deleteFeedback);
                }
                throw new Exception("Bạn chỉ có thể xóa đánh giá của bạn");
            }
            else
            {
                _unitOfWork.FeedbackRepository.SoftDeleteAsync(deleteFeedback);
                _unitOfWork.Save();

                return _mapper.Map<FeedbackModel>(deleteFeedback);
            }
        }

        public async Task<Pagination<FeedbackModel>> GetFeedbackProductPaginationAsync(int productId, PaginationParameter paginationParameter, FeedbackFilter feedbackFilter)
        {
            var feedbacks = await _unitOfWork.FeedbackRepository.GetFeedBackByProductAsync(productId, paginationParameter, feedbackFilter);
            return _mapper.Map<Pagination<FeedbackModel>>(feedbacks);
        }

        public async Task<FeedbackModel> UpdateFeedbackAsync(UpdateFeedbackModel updateFeedbackModel, string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(updateFeedbackModel.Id);
            if (feedback == null)
            {
                throw new Exception("Đánh giá không tồn tại");
            }

            if (currentUser.Role.ToUpper() == RoleEnums.CUSTOMER.ToString().ToUpper())
            {
                if (feedback.UserId == currentUser.Id)
                {
                    var updateFeedback = _mapper.Map(updateFeedbackModel, feedback);
                    _unitOfWork.FeedbackRepository.UpdateAsync(updateFeedback);
                    _unitOfWork.Save();

                    return _mapper.Map<FeedbackModel>(updateFeedback);
                }
                throw new Exception("Bạn chỉ có thể chỉnh sửa đánh giá của bạn");
            }
            else
            {
                var updateFeedback = _mapper.Map(updateFeedbackModel, feedback);
                _unitOfWork.FeedbackRepository.UpdateAsync(updateFeedback);
                _unitOfWork.Save();

                return _mapper.Map<FeedbackModel>(updateFeedback);
            }
        }
    }
}
