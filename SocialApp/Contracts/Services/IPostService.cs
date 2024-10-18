using SocialApp.DTOs;
using SocialApp.Models;

namespace SocialApp.Contracts.Services;

public interface IPostService
{
    Task<List<PostModel>> GetAllPostsAsync();
    Task<PostModel?> GetPostByIdWithNavPropsAsync(int id, bool includeUser = false, bool includeComments = false);
    Task<PostModel> CreatePostAsync(PostCreateDTO postCreateDTO);
    Task<PostModel> UpdatePostAsync(int id, PostUpdateDTO postUpdateDTO);
    Task<bool> DeletePostAsync(int id);
    Task<List<PostModel>> GetPostsByUserIdAsync(int userId);
}