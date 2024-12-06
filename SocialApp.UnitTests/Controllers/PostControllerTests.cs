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

public class PostControllerTests
{
    private IPostService _fakePostService;
    private IMapper _fakeMapper;
    private PostController _postController;

    [SetUp]
    public void SetUp()
    {
        _fakePostService = A.Fake<IPostService>();
        _fakeMapper = A.Fake<IMapper>();
        _postController = new PostController(_fakePostService, _fakeMapper);
    }

    #region GetAllPosts

    [Test]
    public async Task GetAllPosts_WhenCalled_ReturnsPosts()
    {
        // Arrange
        const int userId = 2;
        const string title = "Post Title";
        const string content = "Post Content";

        List<PostModel> posts =
        [
            new PostModel()
            {
                Id = 1,
                Title = title,
                Content = content,
                UserId = userId,
                Comments = [],
                User = new UserModel
                {
                    Id = userId,
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                }
            }
        ];

        List<PostResponseDTO> postsDTO =
        [
            new()
            {
                Id = 1,
                Title = title,
                Content = content,
                UserId = userId
            }
        ];
        A.CallTo(() => _fakePostService.GetAllPostsAsync()).Returns(Task.FromResult(posts));
        A.CallTo(() => _fakeMapper.Map<List<PostResponseDTO>>(posts)).Returns(postsDTO);
        
        // Act
        ActionResult<PostResponseDTO> result = await _postController.GetAllPosts();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(postsDTO);
    }
    

    #endregion

    #region GetPostByIdWithNavProps

    [Test]
    public async Task GetPostByIdWithNavProps_WhenIncludeUserAndComments_ReturnsOkResultWithPostModel()
    {
        // Arrange
        const int userId = 2;
        const string title = "Post Title";
        const string content = "Post Content";

        PostModel post = new PostModel()
        {
            Id = 1,
            Title = title,
            Content = content,
            UserId = userId,
            Comments = [],
            User = new UserModel
            {
                Id = userId,
                Name = "Ben",
                Email = "ben@hotmail.com"
            }
        };

        A.CallTo(() => _fakePostService.GetPostByIdWithNavPropsAsync(userId, true, true))
            .Returns(Task.FromResult<PostModel?>(post));

        A.CallTo(() => _fakeMapper.Map<PostResponseDTO>(post)).MustNotHaveHappened();
        
                
        // Act
        IActionResult result = await _postController.GetPostByIdWithNavProps(userId, true, true);

        // Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(post);
    }
    
    [Test]
    public async Task GetPostByIdWithNavProps_WhenDoesNotIncludeUserAndComments_ReturnsOkResultWithPostDTOModel()
    {
        // Arrange
        const int userId = 2;
        const string title = "Post Title";
        const string content = "Post Content";

        PostModel post = new PostModel()
        {
            Id = 1,
            Title = title,
            Content = content,
            UserId = userId,
            Comments = [],
            User = new UserModel
            {
                Id = userId,
                Name = "Ben",
                Email = "ben@hotmail.com"
            }
        };

        PostResponseDTO postResponseDTO = new()
        {
            Id = 1,
            Title = title,
            Content = content,
            UserId = userId
        };

        A.CallTo(() => _fakePostService.GetPostByIdWithNavPropsAsync(userId, false, false))
            .Returns(Task.FromResult<PostModel?>(post));
        A.CallTo(() => _fakeMapper.Map<PostResponseDTO>(post)).Returns(postResponseDTO);
        
                
        // Act
        IActionResult result = await _postController.GetPostByIdWithNavProps(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(postResponseDTO);
    }

    [Test]
    public async Task GetPostByIdWithNavProps_WhenPostDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        const int userId = 22;

        PostModel? post = null;

        A.CallTo(() => _fakePostService.GetPostByIdWithNavPropsAsync(userId, false, false))
            .Returns(Task.FromResult(post));
                
        // Act
        IActionResult result = await _postController.GetPostByIdWithNavProps(userId);

        // Assert
        result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    #endregion

    #region GetPostsByUserId

    [Test]
    public async Task GetPostsByUserId_WhenCalledWithUserId_ReturnsPostsForGivenUserId()
    {
        //Arrange
        const int userId = 4;
        List<PostModel> posts= [];
        List<PostResponseDTO> postResponseDtos = [];

        A.CallTo(() => _fakePostService.GetPostsByUserIdAsync(userId)).Returns(posts);
        A.CallTo(() => _fakeMapper.Map<List<PostResponseDTO>>(posts)).Returns(postResponseDtos);
        
        //Act
        ActionResult<List<PostResponseDTO>> result = await _postController.GetPostsByUserId(userId);

        //Assert
        result.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(postResponseDtos);

    }

    #endregion

    #region CreatePost

    [Test]
    public async Task CreatePost_WhenUserExists_ReturnsCreatedAtActionResultWithCorrectPostResponse()
    {
        // Arrange
        const int userId = 2;
        const int postId = 1;
        const string title = "Post Title";
        const string content = "Post Content";
        
        PostCreateDTO postCreateDTO = new PostCreateDTO()
        {
            Title = title,
            Content = content,
            UserId = userId,
        };
        
        PostModel post = new PostModel()
        {
            Id = postId,
            Title = title,
            Content = content,
            UserId = userId,
            Comments = [],
            User = new UserModel
            {
                Id = userId,
                Name = "Ben",
                Email = "ben@hotmail.com"
            }
        };
        
        PostResponseDTO postResponseDTO =
            new()
            {
                Id = postId,
                Title = title,
                Content = content,
                UserId = userId
            };

        A.CallTo(() => _fakePostService.CreatePostAsync(postCreateDTO)).Returns(Task.FromResult(post));
        A.CallTo(() => _fakeMapper.Map<PostResponseDTO>(post)).Returns(postResponseDTO);
        
        // Act
        CreatedAtActionResult result = await _postController.CreatePost(postCreateDTO);
        
        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(postResponseDTO);
        result.ActionName.Should().Be(nameof(_postController.GetPostByIdWithNavProps));
        result.RouteValues.Should().ContainKey(nameof(PostModel.Id)).WhoseValue.Should().Be(postId);
    }

    
    #endregion

    #region UpdatePost

    [Test]
    public async Task UpdatePost_WhenPostExists_ReturnOkResultWithUpdatedPost()
    {
        //Arrange
        const int userId = 2;
        const int postId = 1;
        const string title = "Post Title";
        const string content = "Post Content";

        PostUpdateDTO postUpdateDto = new PostUpdateDTO()
        {
            Title = title,
            Content = content,
        };
        
        PostModel post = new PostModel()
        {
            Id = postId,
            Title = title,
            Content = content,
            UserId = userId,
            Comments = [],
            User = new UserModel
            {
                Id = userId,
                Name = "Ben",
                Email = "ben@hotmail.com"
            }
        };
        
        PostResponseDTO postResponseDTO =
            new()
            {
                Id = postId,
                Title = title,
                Content = content,
                UserId = userId
            };

        A.CallTo(() => _fakePostService.UpdatePostAsync(postId, postUpdateDto)).Returns(post);
        A.CallTo(() => _fakeMapper.Map<PostResponseDTO>(post)).Returns(postResponseDTO);
        
        //Act
        ActionResult<PostResponseDTO> result = await _postController.UpdatePost(postId, postUpdateDto);
        
        //Assert
        result.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(postResponseDTO);
    }

    #endregion

    #region DeletePost

    
    [Test]
    public async Task DeletePost_WhenPostExists_ReturnsNoContent()
    {
        //Arrange
        const int postId = 1;
        A.CallTo(() => _fakePostService.DeletePostAsync(postId)).Returns(true);
        //Act
        IActionResult result = await _postController.DeletePost(postId);
        //Assert
        result.Should().BeOfType<NoContentResult>().Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
    
    [Test]
    public async Task DeletePost_WhenPostDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        const int postId = 100;
        A.CallTo(() => _fakePostService.DeletePostAsync(postId)).Returns(false);
        
        //Act
        IActionResult result = await _postController.DeletePost(postId);
        
        //Assert
        result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

}