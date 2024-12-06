using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Contracts.Services;
using SocialApp.Controllers;
using SocialApp.DTOs;
using SocialApp.DTOs.Response;
using SocialApp.Models;

namespace SocialApp.UnitTests.Controllers;

public class CommentControllerTests
{
    private ICommentService _fakeCommentService;
    private IMapper _fakeMapper;
    private CommentController _commentController;

    [SetUp]
    public void SetUp()
    {
        _fakeCommentService = A.Fake<ICommentService>();
        _fakeMapper = A.Fake<IMapper>();
        _commentController = new CommentController(_fakeCommentService, _fakeMapper);
    }

    #region GetAllComments

    [Test]
    public async Task GetAllComments_WhenCalled_ReturnsComments()
    {
        //Arrange
        const int commentId = 3;
        const int userId = 5;
        const int postId = 10;
        const string title = "Post Title";
        const string content = "Post Content";

        List<CommentModel> comments =
        [
            new CommentModel()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
                Post = new PostModel
                {
                    Title = title,
                    Content = content,
                    UserId = userId
                },
                User = new UserModel
                {
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                }
            }
        ];

        List<CommentResponseDTO> commentResponseDtOs =
        [
            new CommentResponseDTO()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
            }
        ];

        A.CallTo(() => _fakeCommentService.GetAllCommentsAsync()).Returns(Task.FromResult(comments));
        A.CallTo(() => _fakeMapper.Map<List<CommentResponseDTO>>(comments)).Returns(commentResponseDtOs);
        
        //Act
        ActionResult<CommentResponseDTO> result = await _commentController.GetAllComments();

        //Assert
        result.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(commentResponseDtOs);
    }

    #endregion

    #region GetCommentByIdWithNavProps

    [Test]
    public async Task GetCommentByIdWithNavProps_WithIncludeUserAndPost_ReturnsOkResult()
    {
        //Arrange
        const int commentId = 3;
        const int userId = 5;
        const int postId = 10;
        const string title = "Post Title";
        const string content = "Post Content";
        const bool includeUser = true;
        const bool includePost = true;

        CommentModel comment =
            new CommentModel()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
                Post = new PostModel
                {
                    Title = title,
                    Content = content,
                    UserId = userId
                },
                User = new UserModel
                {
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                }
            };

        A.CallTo(() => _fakeCommentService.GetCommentByIdWithNavPropsAsync(commentId, includeUser, includePost)).Returns(Task.FromResult<CommentModel?>(comment));

        //Act
        IActionResult result = await _commentController.GetCommentByIdWithNavProps(commentId, includeUser, includePost);

        //Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(comment);

    }
    
    [Test]
    public async Task GetCommentByIdWithNavProps_WithDoesNotIncludeUserAndPost_ReturnsOkResult()
    {
        //Arrange
        const int commentId = 3;
        const int userId = 5;
        const int postId = 10;
        const string title = "Post Title";
        const string content = "Post Content";
        const bool includeUser = false;
        const bool includePost = false;

        CommentModel comment =
            new CommentModel()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
                Post = new PostModel
                {
                    Title = title,
                    Content = content,
                    UserId = userId
                },
                User = new UserModel
                {
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                }
            };
        
        CommentResponseDTO commentResponseDTO =
            new CommentResponseDTO()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
            };

        A.CallTo(() => _fakeCommentService.GetCommentByIdWithNavPropsAsync(commentId, includeUser, includePost)).Returns(Task.FromResult<CommentModel?>(comment));
        A.CallTo(() => _fakeMapper.Map<CommentResponseDTO>(comment)).Returns(commentResponseDTO);
        
        //Act
        IActionResult result = await _commentController.GetCommentByIdWithNavProps(commentId, includeUser, includePost);

        //Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(commentResponseDTO);

    }
    
    [Test]
    public async Task GetCommentByIdWithNavProps_WithCommentDoesNotExist_ReturnsNotFoundResult()
    {
        //Arrange
        const int commentId = 3;
        const bool includeUser = false;
        const bool includePost = false;

        CommentModel? comment = null;
        
        A.CallTo(() => _fakeCommentService.GetCommentByIdWithNavPropsAsync(commentId, includeUser, includePost)).Returns(Task.FromResult<CommentModel?>(comment));
        
        //Act
        IActionResult result = await _commentController.GetCommentByIdWithNavProps(commentId, includeUser, includePost);

        //Assert
        result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);

    }

    #endregion

    #region CreateComment

    [Test]
    public async Task CreateComment_WhenCalledWithValidInputs_ReturnsCreatedAtActionResultWithComment()
    {
        //Arrange
        const int commentId = 3;
        const int userId = 5;
        const int postId = 10;
        const string title = "Post Title";
        const string content = "Post Content";
        
        CommentCreateDTO commentCreateDTO =
            new CommentCreateDTO()
            {
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
            };

        CommentModel comment =
            new CommentModel()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
                Post = new PostModel
                {
                    Title = title,
                    Content = content,
                    UserId = userId
                },
                User = new UserModel
                {
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                }
            };
        
        CommentResponseDTO commentResponseDTO =
            new CommentResponseDTO()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
            };

        A.CallTo(() => _fakeCommentService.CreateCommentAsync(commentCreateDTO)).Returns(Task.FromResult(comment));
        A.CallTo(() => _fakeMapper.Map<CommentResponseDTO>(comment)).Returns(commentResponseDTO);
        //Act
        CreatedAtActionResult result = await _commentController.CreateComment(commentCreateDTO);

        //Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(commentResponseDTO);
        result.ActionName.Should().Be(nameof(_commentController.GetCommentByIdWithNavProps));
        result.RouteValues.Should().ContainKey(nameof(CommentModel.Id)).WhoseValue.Should().Be(commentId);

    }

    #endregion

    #region UpdateComment

    [Test]
    public async Task UpdateComment_WhenCalledWithValidData_ReturnsOkResultWithUpdatedComment()
    {
        //Arrange
        const int commentId = 3;
        const int userId = 5;
        const int postId = 10;
        const string title = "Post Title";
        const string content = "Post Content";
        
        CommentUpdateDTO commentUpdateDTO =
            new CommentUpdateDTO()
            {
                Text = "Comment here",
            };

        CommentModel comment =
            new CommentModel()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
                Post = new PostModel
                {
                    Title = title,
                    Content = content,
                    UserId = userId
                },
                User = new UserModel
                {
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                }
            };
        
        CommentResponseDTO commentResponseDTO =
            new CommentResponseDTO()
            {
                Id = commentId,
                PostId = postId,
                UserId = userId,
                Text = "Comment here",
            };
        
        A.CallTo(() => _fakeCommentService.UpdateCommentAsync(commentId, commentUpdateDTO)).Returns(comment);
        A.CallTo(() => _fakeMapper.Map<CommentResponseDTO>(comment)).Returns(commentResponseDTO);
        //Act
        ActionResult<CommentResponseDTO> result = await _commentController.UpdateComment(commentId, commentUpdateDTO);

        //Assert
        result.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(commentResponseDTO);
    }
    

    #endregion

    #region DeleteComment

    [Test]
    public async Task DeleteComment_WhenCommentDoesNotExist_ReturnsNotFoundResult()
    {
        //Arrange
        const int commentId = 5;
        A.CallTo(() => _fakeCommentService.DeleteCommentAsync(commentId)).Returns(Task.FromResult(false));
        //Act
        IActionResult result = await _commentController.DeleteComment(commentId);

        //Assert
        result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);

    }
    
    [Test]
    public async Task DeleteComment_WhenCommentDoesExist_ReturnsNoContentResult()
    {
        //Arrange
        const int commentId = 5;
        A.CallTo(() => _fakeCommentService.DeleteCommentAsync(commentId)).Returns(Task.FromResult(true));
        //Act
        IActionResult result = await _commentController.DeleteComment(commentId);

        //Assert
        result.Should().BeOfType<NoContentResult>().Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);

    }

    #endregion
}