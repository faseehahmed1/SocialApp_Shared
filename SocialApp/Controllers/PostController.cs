using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.Models;

namespace SocialApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController(IPostService postService, IMapper mapper) : ControllerBase
{
    [HttpGet($"{nameof(GetAllPosts)}")]
    public async Task<IActionResult> GetAllPosts()
    {
        List<PostModel> posts = await postService.GetAllPostsAsync();
        List<PostResponseDTO> postResponseDTO = mapper.Map<List<PostResponseDTO>>(posts);
        return Ok(postResponseDTO);
    }

    [HttpGet($"{nameof(GetPostByIdWithNavProps)}/{{id}}")]
    public async Task<IActionResult> GetPostByIdWithNavProps(int id, [FromQuery] bool includeUser = false, [FromQuery] bool includeComments = false)
    {
        PostModel? post = await postService.GetPostByIdWithNavPropsAsync(id, includeUser, includeComments);
        if (includeUser == false && includeComments == false)
        {
            PostResponseDTO postResponseDTO = mapper.Map<PostResponseDTO>(post);
            return Ok(postResponseDTO);
        }
        return post == null ? NotFound() : Ok(post);
    }

    [HttpGet($"{nameof(GetPostsByUserId)}/{{id}}")]
    public async Task<IActionResult> GetPostsByUserId([FromRoute(Name = "id")] int userId)
    {
        List<PostModel> posts = await postService.GetPostsByUserIdAsync(userId);
        List<PostResponseDTO> postResponseDTO = mapper.Map<List<PostResponseDTO>>(posts);
        return Ok(postResponseDTO);
    }

    [HttpPost($"{nameof(CreatePost)}")]
    public async Task<IActionResult> CreatePost([FromBody] PostCreateDTO postCreateDto)
    {
        PostModel post = await postService.CreatePostAsync(postCreateDto);
        PostResponseDTO postResponseDTO = mapper.Map<PostResponseDTO>(post);
        return CreatedAtAction(nameof(GetPostByIdWithNavProps), new { id = post.Id }, postResponseDTO);
    }

    [HttpPut($"{nameof(UpdatePost)}/{{id}}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateDTO postCreateDto)
    {
        PostModel updatedPost = await postService.UpdatePostAsync(id, postCreateDto);
        PostResponseDTO postResponseDTO = mapper.Map<PostResponseDTO>(updatedPost);
        return Ok(postResponseDTO);
    }

    [HttpDelete($"{nameof(DeletePost)}/{{id}}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        bool result = await postService.DeletePostAsync(id);
        return result ? NoContent() :
        NotFound();
    }
}