using FluentValidation;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;

namespace SocialApp.Validators;

public class CommentCreateDTOValidator : AbstractValidator<CommentCreateDTO>
{
    public CommentCreateDTOValidator(IUserService userService, IPostService postService)
    {
        RuleFor(comment => comment.UserId)
            .MustAsync(async (userId, _) => await userService.GetUserByIdWithNavPropsAsync(userId) != null)
            .WithMessage("UserId {PropertyValue} does not exist.");

        RuleFor(comment => comment.PostId)
            .MustAsync(async (postId, _) => await postService.GetPostByIdWithNavPropsAsync(postId) != null)
            .WithMessage("PostId {PropertyValue} does not exist.");
    }
}