using SocialApp.Models;

namespace SocialApp.Contracts.DataLayers;

public interface ICommentDataLayer
{
    Task<List<CommentModel>> GetAllCommentsAsync();
    Task CreateCommentAsync(CommentModel comment);
    Task<CommentModel?> GetCommentByIdWithNavPropsAsync(int commentId, bool includeUser, bool includePost);
    Task UpdateCommentAsync(CommentModel comment);
    Task DeleteCommentAsync(CommentModel comment);
}