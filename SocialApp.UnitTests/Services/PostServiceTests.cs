using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Specialized;
using SocialApp.Contracts.DataLayers;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.Middleware.Exceptions;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.UnitTests.Services;

public class PostServiceTests
{
    private IPostDataLayer _fakePostDataLayer;
    private IUserService _fakeUserService;
    private PostService _postService;

    [SetUp]
    public void SetUp()
    {
        _fakeUserService = A.Fake<IUserService>();
        _fakePostDataLayer = A.Fake<IPostDataLayer>();
        _postService = new PostService(_fakePostDataLayer, _fakeUserService);
    }

    #region GetAllPostsAsync

    [Test]
    public async Task GetAllPostsAsync_WhenCalled_ReturnsAllPosts()
    {
        //Arrange
        List<PostModel> posts =
        [
            new()
            {
                Id = 1,
                Title = "Some Title",
                Content = "Content Here",
                UserId = 4,
                User = new UserModel
                {
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                },
                Comments = new List<CommentModel>()
            }
        ];

        A.CallTo(() => _fakePostDataLayer.GetAllPostsAsync()).Returns(posts);
        
        //Act
        List<PostModel> result = await _postService.GetAllPostsAsync();

        //Assert
        result.Should().BeEquivalentTo(posts);

    }

    #endregion

    #region GetPostByIdWithNavPropsAsync

    [Test]
    public async Task GetPostByIdWithNavPropsAsync_WhenCalledWithValidIdAndNavProps_ReturnsPostWithNavProps()
    {
        //Arrange
        const int postId = 2;
        const bool includeUser = true;
        const bool includeComments = true;
        
        PostModel post =
            new()
            {
                Id = postId,
                Title = "Some Title",
                Content = "Content Here",
                UserId = 4,
                User = new UserModel
                {
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                },
                Comments = []
            };

        A.CallTo(() => _fakePostDataLayer.GetPostByIdWithNavPropsAsync(postId, includeUser, includeComments)).Returns(Task.FromResult<PostModel?>(post));
        
        //Act
        PostModel? result = await _postService.GetPostByIdWithNavPropsAsync(postId, includeUser, includeComments);

        //Assert
        result.Should().BeEquivalentTo(post);

    }
    
    [Test]
    public async Task GetPostByIdWithNavPropsAsync_WhenCalledWithValidIdAndNoNavProps_ReturnsPostWithoutNavProps()
    {
        //Arrange
        const int postId = 2;
        const bool includeUser = false;
        const bool includeComments = false;
        
        PostModel post =
            new()
            {
                Id = postId,
                Title = "Some Title",
                Content = "Content Here",
                UserId = 4,
            };

        A.CallTo(() => _fakePostDataLayer.GetPostByIdWithNavPropsAsync(postId, includeUser, includeComments)).Returns(Task.FromResult<PostModel?>(post));
        
        //Act
        PostModel? result = await _postService.GetPostByIdWithNavPropsAsync(postId, includeUser, includeComments);

        //Assert
        result.Should().BeEquivalentTo(post);

    }

    #endregion

    #region GetPostsByUserIdAsync

    [Test]
    public async Task GetPostsByUserIdAsync_WhenCalledWithValidUserId_ReturnsPosts()
    {
        //Arrange
        const int userId = 2;
        
        List<PostModel> posts =
        [
            new()
            {
                Id = 1,
                Title = "Some Title",
                Content = "Content Here",
                UserId = userId,
                User = new UserModel
                {
                    Name = "Ben",
                    Email = "ben@hotmail.com"
                },
                Comments = new List<CommentModel>()
            }
        ];

        A.CallTo(() => _fakePostDataLayer.GetPostsByUserIdAsync(userId)).Returns(Task.FromResult(posts));
        
        //Act
        List<PostModel> result = await _postService.GetPostsByUserIdAsync(userId);

        //Assert
        result.Should().BeEquivalentTo(posts);

    }

    #endregion

    #region CreatePostAsync

    [Test]
    public async Task CreatePostAsync_WhenNoUserDoesNotExist_ThrowsNotFoundException()
    {
        //Arrange
        const int userId = 5;
        PostCreateDTO postCreateDTO = new PostCreateDTO()
        {
            UserId = userId,
            Content = "Post content",
            Title = "Post title"
        };

        A.CallTo(() => _fakeUserService.GetUserByIdWithNavPropsAsync(userId, false, false)).Returns(Task.FromResult<UserModel?>(null));
        
        //Act & Assert
        ExceptionAssertions<NotFoundException> exception = await FluentActions.Invoking(() => _postService.CreatePostAsync(postCreateDTO)).Should()
            .ThrowAsync<NotFoundException>();
        
        // Assert the exception message is correct
        exception.WithMessage($"UserId {userId} FK does not exist");
    }

    [Test]
    public async Task CreatePostAsync_WhenUserExist_ReturnCreatedPost()
    {
        //Arrange
        const int userId = 5;
        const string content = "Post Content";
        const string title = "Post title";
        PostCreateDTO postCreateDTO = new PostCreateDTO()
        {
            UserId = userId,
            Content = content,
            Title = title
        };
        UserModel user = new()
        {
            Name = "Ben",
            Email = "ben@hotmail.com"
        };
        
        PostModel createdPost = new PostModel()
        {
            UserId = userId,
            Content = content,
            Title = title
        };

        A.CallTo(() => _fakeUserService.GetUserByIdWithNavPropsAsync(userId, false, false)).Returns(Task.FromResult<UserModel?>(user));
        A.CallTo(() =>
            _fakePostDataLayer.CreatePostAsync(
                A<PostModel>.That.Matches(p => p.UserId == userId && p.Content == content && p.Title == title))).Returns(Task.FromResult(createdPost));
        
        //Act & Assert
        PostModel result = await _postService.CreatePostAsync(postCreateDTO);

        // Assert the exception message is correct
        result.Should().BeEquivalentTo(createdPost);
    }
    #endregion

    #region UpdatePostAsync

    [Test]
    public async Task UpdatePostAsync_WhenPostDoesNotExist_ThrowsNotFoundException()
    {
        //Arrange
        const int postId = 5;
        PostUpdateDTO postUpdateDTO = new PostUpdateDTO()
        {
            Content = "Post content",
            Title = "Post title"
        };
        A.CallTo(() => _fakePostDataLayer.GetPostByIdWithNavPropsAsync(postId, false, false)).Returns(Task.FromResult<PostModel?>(null));
        
        //Act & Assert
        ExceptionAssertions<NotFoundException> exception = await FluentActions
            .Invoking(() => _postService.UpdatePostAsync(postId, postUpdateDTO)).Should()
            .ThrowAsync<NotFoundException>();

        // Assert the exception message is correct
        exception.WithMessage($"Post with ID {postId} not found");
    }
    
    
    [Test]
    public async Task UpdatePostAsync_WhenPostDoesExist_ReturnsUpdatedPost()
    {
        //Arrange
        const int postId = 5;
        const int userId = 7;
        const string content = "Post Content";
        const string title = "Post title";
        
        PostModel existingPost = new()
        {
            Id = postId,
            UserId = userId,
            Content = content,
            Title = title
        };
        
        PostUpdateDTO postUpdateDTO = new PostUpdateDTO()
        {
            Content = "Post content Updated!",
            Title = "Post title Updated!"
        };
        
        PostModel updatedPost = new()
        {
            Id = postId,
            UserId = userId,
            Content = "Post content Updated!",
            Title = "Post title Updated!"
        };
        
        A.CallTo(() => _fakePostDataLayer.GetPostByIdWithNavPropsAsync(postId, false, false)).Returns(Task.FromResult<PostModel?>(existingPost));
        
        //Act 
        PostModel result = await _postService.UpdatePostAsync(postId, postUpdateDTO);


        // Assert the exception message is correct
        result.Should().BeEquivalentTo(updatedPost);

    }

    #endregion

    #region DeletePostAsync

    [Test]
    public async Task DeletePostAsync_WhenPostDoesNotExists_ReturnsFalse()
    {
        //Arrange
        const int postId = 5;

        A.CallTo(() => _fakePostDataLayer.GetPostByIdWithNavPropsAsync(postId, false, false)).Returns(Task.FromResult<PostModel?>(null));
    
        //Act
        bool result = await _postService.DeletePostAsync(postId);

        //Assert
        result.Should().BeFalse();

    }
    
    [Test]
    public async Task DeletePostAsync_WhenPostExists_ReturnsTrue()
    {
        //Arrange
        const int postId = 5;

        PostModel post = new PostModel()
        {
            Id = postId,
            UserId = 3,
            Content = "Some Content Here",
            Title = "some Title"
        };

        A.CallTo(() => _fakePostDataLayer.GetPostByIdWithNavPropsAsync(postId, false, false)).Returns(Task.FromResult<PostModel?>(post));
    
        //Act
        bool result = await _postService.DeletePostAsync(postId);

        //Assert
        result.Should().BeTrue();

    }

    #endregion
}