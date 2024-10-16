using SocialApp.DTOs;
using SocialApp.Models;

namespace SocialApp.Contracts.Services;

public interface ICommentService
{
    Task<List<CommentModel>> GetAllCommentsAsync();
    Task<CommentModel> CreateCommentAsync(CommentCreateDTO commentCreateDTO);
    Task<CommentModel?> GetCommentByIdWithNavPropsAsync(int commentId, bool includeUser = false, bool includePost = false);
    Task<CommentModel> UpdateCommentAsync(int commentId, CommentUpdateDTO commentCreateDTO);
    Task<bool> DeleteCommentAsync(int commentId);
}