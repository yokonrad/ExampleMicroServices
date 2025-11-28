using Comments.Core.Entities;

namespace Comments.Core.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default);

    Task<Comment[]> GetByPostGuidAsync(Guid guid, CancellationToken cancellationToken = default);

    Task<bool> CreateAsync(Comment comment, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Comment comment, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Comment comment, CancellationToken cancellationToken = default);
}