using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.Models;

namespace SocialApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet($"{nameof(GetAllUsers)}")]
    public async Task<IActionResult> GetAllUsers()
    {
        List<UserModel> users = await userService.GetAllUsersAsync();
        List<UserResponseDTO> userResponseDTO = mapper.Map<List<UserResponseDTO>>(users);
        return Ok(userResponseDTO);
    }

    [HttpGet($"{nameof(GetUserByIdWithNavProps)}/{{id}}")]
    public async Task<IActionResult> GetUserByIdWithNavProps(int id, [FromQuery] bool includePosts = false, [FromQuery] bool includeComments = false)
    {
        UserModel? user = await userService.GetUserByIdWithIncludesAsync(id, includePosts, includeComments);
        if (includePosts == false && includeComments == false)
        {
            UserResponseDTO userResponseDTO = mapper.Map<UserResponseDTO>(user);
            return Ok(userResponseDTO);
        }
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost($"{nameof(CreateUser)}")]
    public async Task<IActionResult> CreateUser([FromBody] UserModel user)
    {
        await userService.CreateUserAsync(user);
        UserResponseDTO userResponseDTO = mapper.Map<UserResponseDTO>(user);
        return CreatedAtAction(nameof(GetUserByIdWithNavProps), new { id = user.Id }, userResponseDTO);
    }

    [HttpPut($"{nameof(UpdateUser)}/{{id}}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO user)
    {
        UserModel updatedUser = await userService.UpdateUserAsync(id, user);
        UserResponseDTO userResponseDTO = mapper.Map<UserResponseDTO>(updatedUser);
        return Ok(userResponseDTO);
    }

    [HttpDelete($"{nameof(DeleteUser)}/{{id}}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        bool isUserDeleted = await userService.DeleteUserAsync(id);
        return isUserDeleted ? NoContent() : NotFound();
    }
}