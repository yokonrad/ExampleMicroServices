using AutoMapper;
using Comments.Core.Commands;
using Comments.Core.Dtos;
using Comments.Core.Entities;

namespace Comments.Core.Profiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CommentDto, Comment>().ReverseMap();
        CreateMap<CreateCommentCommand, Comment>().ForMember(x => x.Guid, y => y.Ignore());
    }
}