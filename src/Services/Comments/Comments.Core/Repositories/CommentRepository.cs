using Comments.Core.Data;
using Comments.Core.Entities;
using Comments.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Comments.Core.Repositories;

public class CommentRepository(CommentDbContext commentDbContext) : ICommentRepository
{
    public async Task<Comment?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await commentDbContext.Comments.FindAsync([guid], cancellationToken);
    }

    public async Task<Comment[]> GetByPostGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await commentDbContext.Comments.Where(x => x.PostGuid == guid).ToArrayAsync(cancellationToken);
    }

    public async Task<bool> CreateAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        commentDbContext.Comments.Add(comment);

        return await commentDbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        commentDbContext.Comments.Update(comment);

        return await commentDbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        commentDbContext.Comments.Remove(comment);

        return await commentDbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}