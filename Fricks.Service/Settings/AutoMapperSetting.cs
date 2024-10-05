using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.BusinessModel.CategoryModels;
using Fricks.Service.BusinessModel.FavoriteProductModels;
using Fricks.Service.BusinessModel.FeedbackModels;
using Fricks.Service.BusinessModel.PostModels;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.BusinessModel.ProductPriceModels;
using Fricks.Service.BusinessModel.ProductUnitModels;
using Fricks.Service.BusinessModel.StoreModels;
using Fricks.Service.BusinessModel.UserModels;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Settings
{
    public class AutoMapperSetting : Profile
    {
        public AutoMapperSetting() 
        {
            //Add Automapper
            CreateMap<UserModel, User>().ReverseMap();
            CreateMap<CreateUserModel, User>();
            
            CreateMap<BrandModel, Brand>().ReverseMap();
            CreateMap<BrandProcessModel, Brand>().ReverseMap();
            CreateMap<Pagination<Brand>, Pagination<BrandModel>>().ConvertUsing<PaginationConverter<Brand, BrandModel>>();

            CreateMap<CategoryModel, Category>().ReverseMap();
            CreateMap<CategoryProcessModel, Category>().ReverseMap();
            CreateMap<Pagination<Category>, Pagination<CategoryModel>>().ConvertUsing<PaginationConverter<Category, CategoryModel>>();

            CreateMap<FavoriteProduct, FavoriteProductModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Product.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name));
            CreateMap<FavoriteProduct, FavoriteProductProcessModel>().ReverseMap();
            CreateMap<Pagination<FavoriteProduct>, Pagination<FavoriteProductModel>>().ConvertUsing<PaginationConverter<FavoriteProduct, FavoriteProductModel>>();

            CreateMap<Store, StoreModel>().ForMember(dest => dest.ManagerEmail, otp => otp.MapFrom(src => src.Manager.Email));
            CreateMap<Store, StoreProcessModel>().ReverseMap();
            CreateMap<Store, StoreRegisterModel>().ReverseMap();
            CreateMap<Pagination<Store>, Pagination<StoreModel>>().ConvertUsing<PaginationConverter<Store, StoreModel>>();

            CreateMap<Product, ProductModel>().ReverseMap();
            CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrices.ToList()));
            CreateMap<Product, ProductProcessModel>().ReverseMap();
            CreateMap<Product, ProductRegisterModel>().ReverseMap();
            CreateMap<Pagination<Product>, Pagination<ProductModel>>().ConvertUsing<PaginationConverter<Product, ProductModel>>();

            CreateMap<ProductUnit, ProductUnitModel>().ReverseMap();
            CreateMap<ProductUnit, ProductUnitProcessModel>().ReverseMap();

            CreateMap<ProductPrice, ProductPriceModel>().ReverseMap();
            CreateMap<ProductPrice, ProductPriceProcessModel>().ReverseMap();
            CreateMap<ProductPrice, ProductPriceRegisterModel>().ReverseMap();

            CreateMap<Post, PostModel>().ForMember(dest => dest.ProductName, otp => otp.MapFrom(src => src.Product.Name));
            CreateMap<Pagination<Post>, Pagination<PostModel>>().ConvertUsing<PaginationConverter<Post, PostModel>>();
            CreateMap<CreatePostModel, Post>();

            CreateMap<ProductModel, ItemData>().ReverseMap();

            CreateMap<Feedback, FeedbackModel>()
                .ForMember(dest => dest.ProductName, otp => otp.MapFrom(src => src.Product.Name));
        }
    }

    public class PaginationConverter<TSource, TDestination> : ITypeConverter<Pagination<TSource>, Pagination<TDestination>>
    {
        public Pagination<TDestination> Convert(Pagination<TSource> source, Pagination<TDestination> destination, ResolutionContext context)
        {
            var mappedItems = context.Mapper.Map<List<TDestination>>(source);
            return new Pagination<TDestination>(mappedItems, source.TotalCount, source.CurrentPage, source.PageSize);
        }
    }
}
