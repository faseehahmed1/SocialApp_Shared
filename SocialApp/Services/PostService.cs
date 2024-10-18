using SocialApp.Contracts.DataLayers;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.Middleware.Exceptions;
using SocialApp.Models;

namespace SocialApp.Services;


public class PostService(IPostDataLayer postDataLayer, IUserService userService) : IPostService
{
    public async Task<List<PostModel>> GetAllPostsAsync()
    {
        return await postDataLayer.GetAllPostsAsync();
    }

    public async Task<PostModel?> GetPostByIdWithNavPropsAsync(int id, bool includeUser = false, bool includeComments = false)
    {
        return await postDataLayer.GetPostByIdWithNavPropsAsync(id, includeUser, includeComments);
    }

    public async Task<List<PostModel>> GetPostsByUserIdAsync(int userId)
    {
        return await postDataLayer.GetPostsByUserIdAsync(userId);
    }

    public async Task<PostModel> CreatePostAsync(PostCreateDTO postCreateDTO)
    {
        UserModel? userExists = await userService.GetUserByIdWithNavPropsAsync(postCreateDTO.UserId);
        if (userExists == null)
        {
            throw new NotFoundException($"UserId {postCreateDTO.UserId} FK does not exist");
        }

        PostModel post = new PostModel
        {
            Title = postCreateDTO.Title,
            Content = postCreateDTO.Content,
            UserId = postCreateDTO.UserId
        };
        await postDataLayer.CreatePostAsync(post);
        return post;
    }

    public async Task<PostModel> UpdatePostAsync(int postId, PostUpdateDTO postCreateDTO)
    {
        PostModel? existingPost = await GetPostByIdWithNavPropsAsync(postId);
        if (existingPost == null)
        {
            throw new NotFoundException($"Post with ID {postId} not found");
        }

        existingPost.Title = postCreateDTO.Title;
        existingPost.Content = postCreateDTO.Content;
        await postDataLayer.UpdatePostAsync(existingPost);
        return existingPost;
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        PostModel? existingPost = await postDataLayer.GetPostByIdWithNavPropsAsync(id);
        if (existingPost == null)
        {
            return false;
        }
        await postDataLayer.DeletePostAsync(existingPost);
        return true;
    }
}