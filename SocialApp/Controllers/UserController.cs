using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.DTOs.Response;
using SocialApp.Exceptions;
using SocialApp.Models;

namespace SocialApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet($"{nameof(GetAllUsers)}")]
    public async Task<ActionResult<List<UserResponseDTO>>> GetAllUsers()
    {
        List<UserModel> users = await userService.GetAllUsersAsync();
        List<UserResponseDTO> userResponseDTO = mapper.Map<List<UserResponseDTO>>(users);
        return Ok(userResponseDTO);
    }

    [HttpGet($"{nameof(GetUserByIdWithNavProps)}/{{id}}")]
    public async Task<IActionResult> GetUserByIdWithNavProps(int id, [FromQuery] bool includePosts = false, [FromQuery] bool includeComments = false)
    {
        UserModel? user = await userService.GetUserByIdWithNavPropsAsync(id, includePosts, includeComments);
        if (user == null) return NotFound();

        if (includePosts == false && includeComments == false)
        {
            UserResponseDTO userResponseDTO = mapper.Map<UserResponseDTO>(user);
            return Ok(userResponseDTO);
        }

        return Ok(user);
    }

    [HttpPost($"{nameof(CreateUser)}")]
    public async Task<CreatedAtActionResult> CreateUser([FromBody] UserDTO userDTO)
    {
        UserModel user = await userService.CreateUserAsync(userDTO);
        UserResponseDTO userResponseDTO = mapper.Map<UserResponseDTO>(user);
        return CreatedAtAction(nameof(GetUserByIdWithNavProps), new { id = user.Id }, userResponseDTO);
    }

    [HttpPut($"{nameof(UpdateUser)}/{{id}}")]
    public async Task<ActionResult<UserResponseDTO>> UpdateUser(int id, [FromBody] UserDTO userDTO)
    {
        try
        {
            UserModel updatedUser = await userService.UpdateUserAsync(id, userDTO);
            UserResponseDTO userResponseDTO = mapper.Map<UserResponseDTO>(updatedUser);
            return Ok(userResponseDTO);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete($"{nameof(DeleteUser)}/{{id}}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        bool isUserDeleted = await userService.DeleteUserAsync(id);
        return isUserDeleted ? NoContent() : NotFound();
    }
}