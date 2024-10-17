using FakeItEasy;
using FluentAssertions;
using SocialApp.Contracts.DataLayer;
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
}