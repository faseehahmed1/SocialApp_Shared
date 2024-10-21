using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Specialized;
using FluentValidation;
using SocialApp.Contracts.DataLayers;
using SocialApp.DTOs;
using SocialApp.Middleware.Exceptions;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.UnitTests.Services;

public class CommentServiceTests
{
    private ICommentDataLayer _fakeCommentDataLayer;
    private IValidator<CommentCreateDTO> _fakeCommentValidator;
    private CommentService _commentService;

    [SetUp]
    public void SetUp()
    {
        _fakeCommentDataLayer = A.Fake<ICommentDataLayer>();
        _fakeCommentValidator = A.Fake<IValidator<CommentCreateDTO>>();
        _commentService = new CommentService(_fakeCommentDataLayer, _fakeCommentValidator);
    }

    #region GetAllCommentsAsync

    [Test]
    public async Task GetAllCommentsAsync_WhenCalled_ReturnsAllComments()
    {
        //Arrange
        List<CommentModel> comments =
        [
            new()
            {
                Id = 4,
                PostId = 6,
                UserId = 8,
                Text = "Some comment"
            }
        ];

        A.CallTo(() => _fakeCommentDataLayer.GetAllCommentsAsync()).Returns(Task.FromResult(comments));
        
        //Act
        List<CommentModel> result = await _commentService.GetAllCommentsAsync();

        //Assert
        result.Should().BeEquivalentTo(comments);
    }
    

    #endregion

    #region GetCommentByIdWithNavPropsAsync

    [Test]
    public async Task GetCommentByIdWithNavPropsAsync_WithNavProps_ReturnsCommentWithNavProps()
    {
        //Arrange
        const bool includeUser = true;
        const bool includePost = true;
        const int commentId = 4;
        const int userId = 8;
        const int postId = 2;
        
        CommentModel comment =
            new()
            {
                Id = 4,
                PostId = postId,
                UserId = userId,
                Text = "Some comment",
                User = new UserModel
                {
                    Id = userId,
                    Name = "ben",
                    Email = "ben@hotmail.com"
                },
                Post = new PostModel
                {
                    Id = postId,
                    Title = "Post title",
                    Content = "Post content",
                    UserId = userId
                }
            };

        A.CallTo(() => _fakeCommentDataLayer.GetCommentByIdWithNavPropsAsync(commentId, includeUser, includePost)).Returns(Task.FromResult<CommentModel?>(comment));
       
        //Act
        CommentModel? result =
            await _commentService.GetCommentByIdWithNavPropsAsync(commentId, includeUser, includePost);
        
        //Assert
        result.Should().BeEquivalentTo(comment);

    }

    #endregion

    #region CreateCommentAsync

    [Test]
    public async Task CreateCommentAsync_WhenCommentExists_ReturnsUpdatedComment()
    {
        //Arrange
        const int userId = 3;
        const int postId = 5;
        const string text = "comment text";

        CommentCreateDTO commentCreateDTO = new CommentCreateDTO()
        {
            UserId = userId,
            PostId = postId,
            Text = text
        };
        
        CommentModel comment = new CommentModel()
        {
            UserId = userId,
            PostId = postId,
            Text = text
        };
        
        FluentValidation.Results.ValidationResult validationResult = new();
        A.CallTo(() => _fakeCommentValidator.ValidateAsync(commentCreateDTO, CancellationToken.None))
            .Returns(Task.FromResult(validationResult));
        
        //Act
        CommentModel result = await _commentService.CreateCommentAsync(commentCreateDTO);

        //Assert
        result.Should().BeEquivalentTo(comment);
        A.CallTo(() => _fakeCommentValidator.ValidateAsync(commentCreateDTO, CancellationToken.None)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeCommentDataLayer.CreateCommentAsync(A<CommentModel>.That.Matches(c=>c.Text == text && c.UserId == userId && c.PostId == postId))).MustHaveHappenedOnceExactly();

    }
    
    [Test]
    public async Task CreateCommentAsync_WhenCommentDoesNotExist_ThrowsValidationException()
    {
        //Arrange
        const int userId = 3;
        const int postId = 5;
        const string text = "comment text";

        CommentCreateDTO commentCreateDTO = new CommentCreateDTO()
        {
            UserId = userId,
            PostId = postId,
            Text = text
        };
        
        CommentModel comment = new CommentModel()
        {
            UserId = userId,
            PostId = postId,
            Text = text
        };
        
        FluentValidation.Results.ValidationResult validationResult = new(
            new List<FluentValidation.Results.ValidationFailure>
            {
                new("UserId", "UserId does not exist.")
            });
        A.CallTo(() => _fakeCommentValidator.ValidateAsync(commentCreateDTO, CancellationToken.None))
            .Returns(Task.FromResult(validationResult));
        
        //Act && Assert
        ExceptionAssertions<ValidationException> exception = await FluentActions
            .Invoking(() => _commentService.CreateCommentAsync(commentCreateDTO)).Should()
            .ThrowAsync<ValidationException>();

        //Assert
        exception.Which.Errors.Should().Contain(failure => failure.ErrorMessage == "UserId does not exist.");
    }

    #endregion

    #region UpdateCommentAsync

    [Test]
    public async Task UpdateCommentAsync_WhenCommentDoesNotExist_ThrowsNotFoundException()
    {
        //Arrange
        const int commentId = 6;

        CommentUpdateDTO commentUpdateDTO = new()
        {
            Text = "Comment Text"
        };

        A.CallTo(() => _fakeCommentDataLayer.GetCommentByIdWithNavPropsAsync(commentId, false, false)).Returns(Task.FromResult<CommentModel?>(null));
        
        //Act & Assert
        ExceptionAssertions<NotFoundException> exception = await FluentActions
            .Invoking(() => _commentService.UpdateCommentAsync(commentId, commentUpdateDTO)).Should()
            .ThrowAsync<NotFoundException>();

        //Assert
        exception.WithMessage($"Comment with ID {commentId} not found");
    }
    
    [Test]
    public async Task UpdateCommentAsync_WhenCommentExists_ReturnsUpdatedComment()
    {
        //Arrange
        const int commentId = 4;
        const int userId = 8;
        const int postId = 2;
        
        CommentModel comment =
            new()
            {
                Id = 4,
                PostId = postId,
                UserId = userId,
                Text = "Some comment",
                User = new UserModel
                {
                    Id = userId,
                    Name = "ben",
                    Email = "ben@hotmail.com"
                },
                Post = new PostModel
                {
                    Id = postId,
                    Title = "Post title",
                    Content = "Post content",
                    UserId = userId
                }
            };

        CommentUpdateDTO commentUpdateDTO = new()
        {
            Text = "Comment Text"
        };

        A.CallTo(() => _fakeCommentDataLayer.GetCommentByIdWithNavPropsAsync(commentId, false, false)).Returns(Task.FromResult<CommentModel?>(comment));
        
        //Act & Assert
        CommentModel result = await _commentService.UpdateCommentAsync(commentId, commentUpdateDTO);

        //Assert
        result.Should().BeEquivalentTo(comment);
    }

    #endregion

    #region DeleteCommentAsync

    [Test]
    public async Task DeleteCommentAsync_WhenCommentDoesNotExist_ReturnsFalse()
    {
        //Arrange
        const int commentId = 6;

        A.CallTo(() => _fakeCommentDataLayer.GetCommentByIdWithNavPropsAsync(commentId, false, false)).Returns(Task.FromResult<CommentModel?>(null));
        
        //Act
        bool result = await _commentService.DeleteCommentAsync(commentId);
        //Assert
        result.Should().BeFalse();

    }
    
    [Test]
    public async Task DeleteCommentAsync_WhenCommentExists_ReturnsTrue()
    {
        //Arrange
        const int commentId = 4;
        const int userId = 8;
        const int postId = 2;
        
        CommentModel comment =
            new()
            {
                Id = 4,
                PostId = postId,
                UserId = userId,
                Text = "Some comment",
                User = new UserModel
                {
                    Id = userId,
                    Name = "ben",
                    Email = "ben@hotmail.com"
                },
                Post = new PostModel
                {
                    Id = postId,
                    Title = "Post title",
                    Content = "Post content",
                    UserId = userId
                }
            };

        A.CallTo(() => _fakeCommentDataLayer.GetCommentByIdWithNavPropsAsync(commentId, false, false)).Returns(Task.FromResult<CommentModel?>(comment));
        
        //Act
        bool result = await _commentService.DeleteCommentAsync(commentId);
        //Assert
        result.Should().BeTrue();

    }

    #endregion
}