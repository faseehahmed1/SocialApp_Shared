using SocialApp.Models;

namespace SocialApp.Contracts.DataLayers;

public interface IPostDataLayer
{
    Task<List<PostModel>> GetAllPostsAsync();
    Task<PostModel?> GetPostByIdWithNavPropsAsync(int id, bool includeUser = false, bool includeComments = false);
    Task<List<PostModel>> GetPostsByUserIdAsync(int id);
    Task UpdatePostAsync(PostModel post);
    Task DeletePostAsync(PostModel post);
    Task CreatePostAsync(PostModel post);
}