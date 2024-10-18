using SocialApp.Models;

namespace SocialApp.Contracts.DataLayers;

public interface IUserDataLayer
{
    Task<List<UserModel>> GetAllUsersAsync();
    Task<UserModel?> GetUserByIdWithNavPropsAsync(int id, bool includePosts = false, bool includeComments = false);
    Task<UserModel> CreateUserAsync(UserModel user);
    Task<UserModel> UpdateUserAsync(UserModel user);
    Task DeleteUserAsync(UserModel user);
}