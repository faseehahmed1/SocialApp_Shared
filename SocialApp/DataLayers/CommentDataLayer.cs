using Microsoft.EntityFrameworkCore;
using SocialApp.Contracts.DataLayers;
using SocialApp.Data;
using SocialApp.Models;

namespace SocialApp.DataLayers;

public class CommentDataLayer(AppDbContext dbContext) : ICommentDataLayer
{
    public async Task<List<CommentModel>> GetAllCommentsAsync()
    {
        return await dbContext.Comments.ToListAsync();
    }

    public async Task<CommentModel?> GetCommentByIdWithNavPropsAsync(int commentId, bool includeUser, bool includePost)
    {
        IQueryable<CommentModel> query = dbContext.Comments.AsQueryable();

        var loadOptions = (includeUser, includePost);

        query = loadOptions switch
        {
            (true, true) => query.Include(u => u.Post)
                .Include(p => p.User),
            (true, false) => query.Include(u => u.User),
            (false, true) => query.Include(u => u.Post),
            _ => query  // No includes, just return the user
        };

        return await query.FirstOrDefaultAsync(c => c.Id == commentId);
    }

    public async Task CreateCommentAsync(CommentModel comment)
    {
        await dbContext.Comments.AddAsync(comment);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(CommentModel comment)
    {
        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(CommentModel comment)
    {
        dbContext.Comments.Remove(comment);
        await dbContext.SaveChangesAsync();
    }
}