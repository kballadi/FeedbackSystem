using AutoMapper;
using FeedbackSystem.Api.Dtos;
using FeedbackSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedbackSystem.Api.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserFeedback, FeedbackReadDto>();
            CreateMap<FeedbackCreateDto, UserFeedback>();
            CreateMap<FeedbackUpdateDto, UserFeedback>();
            CreateMap<UserFeedback, FeedbackUpdateDto>();
        }
    }
}
