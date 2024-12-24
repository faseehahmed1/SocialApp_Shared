using SocialApp.Contracts.DataLayers;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.Exceptions;
using SocialApp.Models;

namespace SocialApp.Services;
public class UserService(IUserDataLayer userDataLayer) : IUserService
{
    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        return await userDataLayer.GetAllUsersAsync();
    }

    public async Task<UserModel?> GetUserByIdWithNavPropsAsync(int id, bool includePosts = false, bool includeComments = false)
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
        UserModel? existingUser = await GetUserByIdWithNavPropsAsync(id);
        if (existingUser == null)
        {
            throw new NotFoundException($"User with id: {id} does not exist");
        }

        await userDataLayer.UpdateUserAsync(existingUser);
        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        UserModel? user = await GetUserByIdWithNavPropsAsync(id);
        if (user == null) return false;
        await userDataLayer.DeleteUserAsync(user);
        return true;
    }
}