using Posts.Domain.Entities;

namespace Posts.Domain.Interfaces;

public interface IPostRepository
{
    Task<Post[]> GetAsync(CancellationToken cancellationToken = default);

    Task<Post?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default);

    Task<bool> CreateAsync(Post post, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Post post, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Post post, CancellationToken cancellationToken = default);
}