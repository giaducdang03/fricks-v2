using AutoMapper;
using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.UserModels;
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
        }
    }
}
