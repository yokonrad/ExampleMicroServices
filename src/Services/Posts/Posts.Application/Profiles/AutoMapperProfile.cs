using AutoMapper;
using Posts.Application.Commands;
using Posts.Application.Dtos;
using Posts.Domain.Entities;

namespace Posts.Application.Profiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<PostDto, Post>().ReverseMap();
        CreateMap<CreatePostCommand, Post>().ForMember(x => x.Guid, y => y.Ignore());
    }
}