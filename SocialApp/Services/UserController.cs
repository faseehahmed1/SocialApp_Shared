using SocialApp.Contracts.DataLayer;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.Middleware.Exceptions;
using SocialApp.Models;

namespace SocialApp.Services;
public class UserController(IUserDataLayer userDataLayer) : IUserService
{
    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        return await userDataLayer.GetAllUsersAsync();
    }

    public async Task<UserModel?> GetUserByIdWithIncludesAsync(int id, bool includePosts = false, bool includeComments = false)
    {
        return await userDataLayer.GetUserByIdWithNavPropsAsync(id, includePosts, includeComments);
    }

    public async Task<UserModel> CreateUserAsync(UserDTO userDTO)
    {
        UserModel user = new UserModel()
        {
            Name = userDTO.Name,
            Email = userDTO.Email
        };
        return await userDataLayer.CreateUserAsync(user);
    }

    public async Task<UserModel> UpdateUserAsync(int id, UserDTO userDTO)
    {
        UserModel? existingUser = await GetUserByIdWithIncludesAsync(id);
        if (existingUser == null)
        {
            throw new NotFoundException($"User with id: {id} does not exist");
        }

        existingUser.Name = userDTO.Name;
        existingUser.Email = userDTO.Email;
        return await userDataLayer.UpdateUserAsync(existingUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        UserModel? user = await GetUserByIdWithIncludesAsync(id);
        if (user == null) return false;
        await userDataLayer.DeleteUserAsync(user);
        return true;
    }
}