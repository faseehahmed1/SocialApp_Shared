using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Specialized;
using SocialApp.Contracts.DataLayers;
using SocialApp.DTOs;
using SocialApp.Exceptions;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.UnitTests.Services;

public class CommentServiceTests
{
    private ICommentDataLayer _fakeCommentDataLayer;
    private IUserDataLayer _fakeUserDataLayer;
    private IPostDataLayer _fakePostDataLayer;
    private CommentService _commentService;
    
    [SetUp]
    public void SetUp()
    {
        _fakeCommentDataLayer = A.Fake<ICommentDataLayer>();
        _fakeUserDataLayer = A.Fake<IUserDataLayer>();
        _fakePostDataLayer = A.Fake<IPostDataLayer>();
        _commentService = new CommentService(_fakeCommentDataLayer, _fakeUserDataLayer, _fakePostDataLayer);
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
    public async Task CreateCommentAsync_WhenForeignKeysAreValid_CreatesAComment()
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
        
         UserModel user = new UserModel()
        {
            Id = userId,
            Name = "Ben",
            Email = "Ben@homtail.com"
        };

         PostModel post = new PostModel()
        {
            Id = postId,
            UserId = userId,
            Title = "Title here",
            Content = "Hello World"
        };


        A.CallTo(() => _fakeUserDataLayer.GetUserByIdWithNavPropsAsync(commentCreateDTO.UserId, false, false))
            .Returns(Task.FromResult<UserModel?>(user));
        
        A.CallTo(() => _fakePostDataLayer.GetPostByIdWithNavPropsAsync(commentCreateDTO.PostId, false, false))
            .Returns(Task.FromResult<PostModel?>(post));
        
        //Act
        CommentModel result = await _commentService.CreateCommentAsync(commentCreateDTO);

        //Assert
        result.Should().BeEquivalentTo(comment);
        A.CallTo(() => _fakeCommentDataLayer.CreateCommentAsync(A<CommentModel>.That.Matches(c=>c.Text == text && c.UserId == userId && c.PostId == postId))).MustHaveHappenedOnceExactly();

    }
    
    [Test]
    public async Task CreateCommentAsync_WhenForeignKeysAreInValid_ThrowsNotFoundValidationException()
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
        A.CallTo(() => _fakeUserDataLayer.GetUserByIdWithNavPropsAsync(commentCreateDTO.UserId, false, false))
            .Returns(Task.FromResult<UserModel?>(null));
        
        A.CallTo(() => _fakePostDataLayer.GetPostByIdWithNavPropsAsync(commentCreateDTO.PostId, false, false))
            .Returns(Task.FromResult<PostModel?>(null));
        
        //Act && Assert
        ExceptionAssertions<NotFoundException> exception = await FluentActions
            .Invoking(() => _commentService.CreateCommentAsync(commentCreateDTO)).Should()
            .ThrowAsync<NotFoundException>();

        //Assert
        exception.WithMessage($"User ID {commentCreateDTO.UserId} not found");    }

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