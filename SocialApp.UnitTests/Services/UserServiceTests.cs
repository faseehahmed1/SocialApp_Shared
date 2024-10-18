using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Specialized;
using SocialApp.Contracts.DataLayers;
using SocialApp.DTOs;
using SocialApp.Middleware.Exceptions;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.UnitTests.Services;

public class UserServiceTests
{
    private IUserDataLayer _fakeUserDataLayer;
    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        _fakeUserDataLayer = A.Fake<IUserDataLayer>();
        _userService = new UserService(_fakeUserDataLayer);
    }

    #region GetAllUsersAsync

    [Test]
    public async Task GetAllUsersAsync_WhenCalled_ReturnsListOfUsers()
    {
        //Arrange
        List<UserModel> expectedUsers =
        [
            new UserModel()
            {
                Id = 2,
                Name = "Ben",
                Email = "ben@hotmail.com",
                Comments = [],
                Posts = []
            }
        ];

        A.CallTo(() => _fakeUserDataLayer.GetAllUsersAsync()).Returns(Task.FromResult(expectedUsers));
        
        //Act
        List<UserModel> result = await _userService.GetAllUsersAsync();
        //Assert
        result.Should().HaveCount(expectedUsers.Count);
        result.Should().BeEquivalentTo(expectedUsers);
    }

    #endregion

    #region GetUserByIdWithNavPropsAsync

    [Test]
    public async Task GetUserByIdWithNavPropsAsync_WithNavPropsPostAndComments_ReturnsUserWithNavProps()
    {
        //Arrange
        const int userId = 2;
        const bool includePosts = true;
        const bool includeComments = true;

        UserModel expectedUser = new UserModel()
            {
                Id = 2,
                Name = "Ben",
                Email = "ben@hotmail.com",
                Comments = [],
                Posts = []
            };

        A.CallTo(() => _fakeUserDataLayer.GetUserByIdWithNavPropsAsync(userId, includePosts, includeComments)).Returns(Task.FromResult<UserModel?>(expectedUser));
        //Act
        UserModel? result = await _userService.GetUserByIdWithNavPropsAsync(userId, includePosts, includeComments);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);

    }
    
    [Test]
    public async Task GetUserByIdWithNavPropsAsync_WithoutNavPropsPostAndComments_ReturnsUserWithoutNavProps()
    {
        //Arrange
        const int userId = 2;
        const bool includePosts = false;
        const bool includeComments = false;

        UserModel expectedUser = new UserModel()
        {
            Id = 2,
            Name = "Ben",
            Email = "ben@hotmail.com",
        };

        A.CallTo(() => _fakeUserDataLayer.GetUserByIdWithNavPropsAsync(userId, includePosts, includeComments)).Returns(Task.FromResult<UserModel?>(expectedUser));
        //Act
        UserModel? result = await _userService.GetUserByIdWithNavPropsAsync(userId, includePosts, includeComments);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);

    }

    #endregion

    #region CreateUserAsync

    [Test]
    public async Task CreateUserAsync_WithValidInput_ReturnsCreatedUser()
    {
        //Arrange
        const string name = "Ben";
        const string email = "ben@hotmail.com";
        UserDTO userDTO = new UserDTO()
        {
            Name = name,
            Email = email,
        };
        
        UserModel user = new UserModel()
        {
            Name = name,
            Email = email,
            Comments = [],
            Posts = []
        };
        
        UserModel createdUser = new UserModel()
        {
            Id = 2,
            Name = name,
            Email = email,
            Comments = [],
            Posts = []
        };
        
        A.CallTo(() => _fakeUserDataLayer.CreateUserAsync(A<UserModel>.That.Matches(u => 
            u.Name == name && u.Email == email))).Returns(createdUser);
        
        //Act
        UserModel result = await _userService.CreateUserAsync(userDTO);

        //Assert
        result.Should().BeEquivalentTo(createdUser);
    }

    #endregion

    #region UpdateUserAsync

    [Test]
    public async Task UpdateUserAsync_WithValidUserIdAndInput_ReturnsUpdatedUser()
    {
        //Arrange
        const int userId = 5;
        const string name = "Ben";
        const string email = "ben@hotmail.com";
        UserDTO userDTO = new UserDTO()
        {
            Name = name,
            Email = email,
        };
        
        UserModel existingUser = new UserModel()
        {
            Id = userId,
            Name = "Sam",
            Email = "sam@hotmail.com",
            Comments = [],
            Posts = []
        };
        
        UserModel updatedUser = new UserModel()
        {
            Id = userId,
            Name = name,
            Email = email,
            Comments = [],
            Posts = []
        };
        
        A.CallTo(() => _fakeUserDataLayer.GetUserByIdWithNavPropsAsync(userId, false, false)).Returns(existingUser);
        A.CallTo(() => _fakeUserDataLayer.UpdateUserAsync(A<UserModel>.That.Matches(u => 
                                                          u.Id == userId && u.Name == name && u.Email == email))).Returns(updatedUser);
        //Act
        UserModel result = await _userService.UpdateUserAsync(userId, userDTO);

        //Assert
        result.Should().BeEquivalentTo(updatedUser);

    }
    
    [Test]
    public async Task UpdateUserAsync_WhenUserDoesNotExist_ThrowsNotFoundException()
    {
        //Arrange
        const int userId = 5;
        const string name = "Ben";
        const string email = "ben@hotmail.com";
        UserDTO userDTO = new UserDTO()
        {
            Name = name,
            Email = email,
        };

        UserModel? existingUser = null;
        
        A.CallTo(() => _fakeUserDataLayer.GetUserByIdWithNavPropsAsync(userId, false, false)).Returns(existingUser);
        
        // Act & Assert
        ExceptionAssertions<NotFoundException> exception = await FluentActions.Invoking(() => _userService.UpdateUserAsync(userId, userDTO))
            .Should().ThrowAsync<NotFoundException>();

        // Assert the exception message is correct
        exception.WithMessage($"User with id: {userId} does not exist");

    }

    #endregion

    #region DeleteUserAsync

    [Test]
    public async Task DeleteUserAsync_WhenUserExists_ReturnsTrue()
    {
        //Arrange
        const int userId = 5;
        UserModel existingUser = new UserModel()
        {
            Id = userId,
            Name = "Sam",
            Email = "sam@hotmail.com",
            Comments = [],
            Posts = []
        };

        
        A.CallTo(() => _fakeUserDataLayer.GetUserByIdWithNavPropsAsync(userId, false, false)).Returns(existingUser);
        
        //Act
        bool result = await _userService.DeleteUserAsync(userId);

        //Assert
        A.CallTo(() => _fakeUserDataLayer.DeleteUserAsync(existingUser)).MustHaveHappened();
        result.Should().BeTrue();

    }
    
    [Test]
    public async Task DeleteUserAsync_WhenUserDoesNotExist_ReturnsTrue()
    {
        //Arrange
        const int userId = 5;
        UserModel? existingUser = null;

        
        A.CallTo(() => _fakeUserDataLayer.GetUserByIdWithNavPropsAsync(userId, false, false)).Returns(existingUser);
        
        //Act
        bool result = await _userService.DeleteUserAsync(userId);

        //Assert
        A.CallTo(() => _fakeUserDataLayer.DeleteUserAsync(null!)).MustNotHaveHappened();
        result.Should().BeFalse();

    }

    #endregion
    
    
}