using SocialApp.DTOs;
using SocialApp.Models;

namespace SocialApp.Contracts.Services;

public interface IUserService
{
    Task<List<UserModel>> GetAllUsersAsync();
    Task<UserModel?> GetUserByIdWithIncludesAsync(int id, bool includePosts = false, bool includeComments = false);
    Task CreateUserAsync(UserModel userModel);
    Task<UserModel> UpdateUserAsync(int id, UserDTO userDTO);
    Task<bool> DeleteUserAsync(int id);
}