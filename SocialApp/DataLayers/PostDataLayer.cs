using Microsoft.EntityFrameworkCore;
using SocialApp.Contracts.DataLayers;
using SocialApp.Data;
using SocialApp.Models;

namespace SocialApp.DataLayers;

public class PostDataLayer(AppDbContext dbContext) : IPostDataLayer
{
    public async Task<List<PostModel>> GetAllPostsAsync()
    {
        return await dbContext.Posts.ToListAsync();
    }

    public async Task<PostModel?> GetPostByIdWithNavPropsAsync(int id, bool includeUser, bool includeComments)
    {
        IQueryable<PostModel> query = dbContext.Posts.AsQueryable();

        var loadOptions = (includeUser, includeComments);

        query = loadOptions switch
        {
            (true, true) => query.Include(u => u.User)
                .Include(p => p.Comments),
            (true, false) => query.Include(u => u.User),
            (false, true) => query.Include(u => u.Comments),
            _ => query  // No includes, just return the user
        };

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<PostModel>> GetPostsByUserIdAsync(int id)
    {
        return await dbContext.Posts
            .Where(p => p.UserId == id)
            .ToListAsync();
    }

    public async Task CreatePostAsync(PostModel post)
    {
        await dbContext.Posts.AddAsync(post);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(PostModel post)
    {
        dbContext.Posts.Update(post);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeletePostAsync(PostModel post)
    {
        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync();
    }
}