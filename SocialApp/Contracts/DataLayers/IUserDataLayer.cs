using SocialApp.Models;

namespace SocialApp.Contracts.DataLayer;

public interface IUserDataLayer
{
    Task<List<UserModel>> GetAllUsersAsync();
    Task<UserModel?> GetUserByIdWithNavPropsAsync(int id, bool includePosts, bool includeComments);
    Task<UserModel> CreateUserAsync(UserModel user);
    Task<UserModel> UpdateUserAsync(UserModel user);
    Task DeleteUserAsync(UserModel user);
}