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

[TestFixture]
public class UserControllerTests
{
    private IUserService _fakeUserService = null!;
    private IMapper _fakeMapper = null!;
    private UserController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _fakeUserService = A.Fake<IUserService>();
        _fakeMapper = A.Fake<IMapper>();
        _controller = new UserController(_fakeUserService, _fakeMapper);
    }

    #region GetAllUsers

    [Test]
    public async Task GetAllUsers_WhenCalled_ReturnsOkResultWithUsers()
    {
        // Arrange
        List<UserModel> users =
        [
            new UserModel()
            {
                Id = 1,
                Name = "Lionel Messi",
                Email = "messi@hotmail.com",
                Comments = [],
                Posts = []
            }
        ];
        
        List<UserResponseDTO> usersResponseDTO =
        [
            new UserResponseDTO()
            {
                Id = 1,
                Name = "Lionel Messi",
                Email = "messi@hotmail.com"
            }
        ];

        A.CallTo(() => _fakeUserService.GetAllUsersAsync()).Returns(Task.FromResult(users));
        A.CallTo(() => _fakeMapper.Map<List<UserResponseDTO>>(users)).Returns(usersResponseDTO);
        
        // Act
        ActionResult<List<UserResponseDTO>> result = await _controller.GetAllUsers();
        // Assert
        OkObjectResult okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(usersResponseDTO);
    }

    #endregion

    #region GetUserByIdWithNavProps

    [Test]
    public async Task GetUserByIdWithNavProps_WhenDoesNotIncludePostsOrComments_ReturnsOkResultWithUsersResponseDTOModel()
    {
        // Arrange
        bool includeComments = false;
        bool includePosts = false;
        int userId = 5;

        UserModel user = new UserModel()
        {
            Id = userId,
            Name = "Lionel Messi",
            Email = "messi@hotmail.com",
            Posts = [],
            Comments = []
        };
        UserResponseDTO userResponseDTO = new UserResponseDTO()
        {
            Id = userId,
            Name = "Lionel Messi",
            Email = "messi@hotmail.com",
        };

        A.CallTo(() => _fakeUserService.GetUserByIdWithNavPropsAsync(userId, includePosts,includeComments )).Returns(Task.FromResult<UserModel?>(user));
        A.CallTo(() => _fakeMapper.Map<UserResponseDTO>(user)).Returns(userResponseDTO);
        
        // Act
        IActionResult result = await _controller.GetUserByIdWithNavProps(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(userResponseDTO);
    }
    
    [Test]
    public async Task GetUserByIdWithNavProps_WhenIncludesPostsAndComments_ReturnsOkResultWithUsersModel()
    {
        // Arrange
        bool includeComments = true;
        bool includePosts = true;
        int userId = 5;

        UserModel user = new UserModel()
        {
            Id = userId,
            Name = "Lionel Messi",
            Email = "messi@hotmail.com",
            Posts = [],
            Comments = []
        };

        A.CallTo(() => _fakeUserService.GetUserByIdWithNavPropsAsync(userId, includePosts,includeComments )).Returns(Task.FromResult<UserModel?>(user));
        A.CallTo(() => _fakeMapper.Map<UserResponseDTO>(user)).MustNotHaveHappened();
        
        // Act
        IActionResult result = await _controller.GetUserByIdWithNavProps(userId, includePosts, includeComments);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(user);
    }
    
    [Test]
    public async Task GetUserByIdWithNavProps_WhenUserDoesNotExistForId_ReturnsNotFound()
    {
        // Arrange
        int userId = 5;

        UserModel? user = null;

        A.CallTo(() => _fakeUserService.GetUserByIdWithNavPropsAsync(userId, false,false )).Returns(Task.FromResult(user));
        A.CallTo(() => _fakeMapper.Map<UserResponseDTO>(user)).MustNotHaveHappened();
        
        // Act
        IActionResult result = await _controller.GetUserByIdWithNavProps(userId);

        // Assert
        result.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    #region CreateUser

    [Test]
    public async Task CreateUser_WithValidInput_ReturnsCreatedAtActionResultWithCorrectUserResponse()
    {
        // Arrange
        const string name = "Messi";
        const string email = "messi@hotmail.com";
        const int userId = 1;

        UserDTO userDTO = new UserDTO()
        {
            Name = name,
            Email = email
        };
        
        UserModel user = new UserModel()
        {
            Id = userId,
            Name = name,
            Email = email,
            Posts = [],
            Comments = []
        };
        
        UserResponseDTO userResponseDTO = new UserResponseDTO()
        {
            Id = userId,
            Name = name,
            Email = email
        };

        A.CallTo(() => _fakeUserService.CreateUserAsync(userDTO)).Returns(Task.FromResult(user));
        A.CallTo(() => _fakeMapper.Map<UserResponseDTO>(user)).Returns(userResponseDTO);
        
        // Act
        CreatedAtActionResult result = await  _controller.CreateUser(userDTO);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(userResponseDTO);
        result.ActionName.Should().Be(nameof(_controller.GetUserByIdWithNavProps));
        result.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(userId);
    }

    #endregion

    #region UpdateUser

    [Test]
    public async Task UpdateUser_WithValidInput_ReturnsOkResultWithUpdatedUser()
    {
        // Arrange
        const string name = "Messi";
        const string email = "messi@hotmail.com";
        const int userId = 1;

        UserDTO userDTO = new UserDTO()
        {
            Name = "Ben",
            Email = "ben@hotmail.com",
        };
        UserModel updatedUser = new UserModel()
        {
            Id = userId,
            Name = name,
            Email = email,
            Posts = [],
            Comments = []
        };
        
        UserResponseDTO userResponseDTO = new UserResponseDTO()
        {
            Id = userId,
            Name = name,
            Email = email
        };

        A.CallTo(() => _fakeUserService.UpdateUserAsync(userId, userDTO)).Returns(updatedUser);
        A.CallTo(() => _fakeMapper.Map<UserResponseDTO>(updatedUser)).Returns(userResponseDTO);
        
        // Act
        ActionResult<UserResponseDTO> result = await _controller.UpdateUser(userId, userDTO);
        
        // Assert
        result.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(userResponseDTO);
    }
    #endregion

    #region DeleteUser

    [Test]
    public async Task DeleteUser_WhenUserExists_ReturnsNoContent()
    {
        // Arrange
        const int userId = 5;
        A.CallTo(() => _fakeUserService.DeleteUserAsync(userId)).Returns(Task.FromResult(true));

        // Act
        IActionResult result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<NoContentResult>().Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
    
    [Test]
    public async Task DeleteUser_WhenUserDoesNotExists_ReturnsNoFound()
    {
        // Arrange
        const int userId = 6;
        A.CallTo(() => _fakeUserService.DeleteUserAsync(userId)).Returns(Task.FromResult(false));

        // Act
        IActionResult result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion
}