using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;
using Posts.Domain.Interfaces;
using Posts.Infrastructure.Data;

namespace Posts.Infrastructure.Repositories;

public class PostRepository(PostDbContext postDbContext) : IPostRepository
{
    public async Task<Post[]> GetAsync(CancellationToken cancellationToken = default)
    {
        return await postDbContext.Posts.AsNoTracking().ToArrayAsync(cancellationToken);
    }

    public async Task<Post?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await postDbContext.Posts.FindAsync([guid], cancellationToken);
    }

    public async Task<bool> CreateAsync(Post post, CancellationToken cancellationToken = default)
    {
        postDbContext.Posts.Add(post);

        return await postDbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(Post post, CancellationToken cancellationToken = default)
    {
        postDbContext.Posts.Update(post);

        return await postDbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Post post, CancellationToken cancellationToken = default)
    {
        postDbContext.Posts.Remove(post);

        return await postDbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}