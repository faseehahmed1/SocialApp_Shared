using SocialApp.Contracts.DataLayers;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.Exceptions;
using SocialApp.Models;

namespace SocialApp.Services;

public class CommentService(ICommentDataLayer commentDataLayer, IUserDataLayer userDataLayer, IPostDataLayer postDataLayer) : ICommentService
{
    public async Task<List<CommentModel>> GetAllCommentsAsync()
    {
        return await commentDataLayer.GetAllCommentsAsync();
    }

    public async Task<CommentModel?> GetCommentByIdWithNavPropsAsync(int commentId, bool includeUser = false, bool includePost = false)
    {
        return await commentDataLayer.GetCommentByIdWithNavPropsAsync(commentId, includeUser, includePost);
    }

    public async Task<CommentModel> CreateCommentAsync(CommentCreateDTO commentCreateDTO)
    {
        await ValidateForeignKeysAsync(commentCreateDTO);

        CommentModel comment = new CommentModel()
        {
            PostId = commentCreateDTO.PostId,
            UserId = commentCreateDTO.UserId,
            Text = commentCreateDTO.Text
        };
        await commentDataLayer.CreateCommentAsync(comment);
        return comment;
    }

    public async Task<CommentModel> UpdateCommentAsync(int commentId, CommentUpdateDTO commentUpdateDTO)
    {
        CommentModel? existingComment = await GetCommentByIdWithNavPropsAsync(commentId);
        if (existingComment == null)
        {
            throw new NotFoundException($"Comment with ID {commentId} not found");
        }

        existingComment.Text = commentUpdateDTO.Text;

        await commentDataLayer.UpdateCommentAsync(existingComment);
        return existingComment;
    }

    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        CommentModel? existingComment = await GetCommentByIdWithNavPropsAsync(commentId);
        if (existingComment == null)
        {
            return false;
        }
        await commentDataLayer.DeleteCommentAsync(existingComment);
        return true;
    }

    private async Task ValidateForeignKeysAsync(CommentCreateDTO commentCreateDTO)
    {
        UserModel? user = await userDataLayer.GetUserByIdWithNavPropsAsync(commentCreateDTO.UserId);
        if (user == null)
        {
            throw new NotFoundException($"User ID {commentCreateDTO.UserId} not found");
        }

        PostModel? post = await postDataLayer.GetPostByIdWithNavPropsAsync(commentCreateDTO.PostId);
        if (post == null)
        {
            throw new NotFoundException($"Post ID {commentCreateDTO.PostId} not found");

        }
    }
}