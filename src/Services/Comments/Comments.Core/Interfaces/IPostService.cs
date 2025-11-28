using Comments.Core.Dtos;

namespace Comments.Core.Interfaces;

public interface IPostService
{
    Task<PostDto?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default);
}