using Microsoft.EntityFrameworkCore;
using SocialApp.Contracts.DataLayer;
using SocialApp.Data;
using SocialApp.Models;

namespace SocialApp.DataLayers;

public class UserDataLayer(AppDbContext dbContext) : IUserDataLayer
{
    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        return await dbContext.Users.OrderBy(u => u.Id).ToListAsync();
    }

    public async Task<UserModel?> GetUserByIdWithNavPropsAsync(int id, bool includePosts, bool includeComments)
    {
        IQueryable<UserModel> query = dbContext.Users.AsQueryable();

        (bool includePosts, bool includeComments) loadOptions = (includePosts, includeComments);

        query = loadOptions switch
        {
            (true, true) => query.Include(u => u.Posts)
                .Include(p => p.Comments),
            (true, false) => query.Include(u => u.Posts),
            (false, true) => query.Include(u => u.Comments),
            _ => query  // No includes, just return the user
        };

        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserModel> CreateUserAsync(UserModel user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<UserModel> UpdateUserAsync(UserModel user)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUserAsync(UserModel user)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
    }
}