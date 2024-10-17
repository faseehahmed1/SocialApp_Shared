using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Contracts.Services;
using SocialApp.DTOs;
using SocialApp.Models;

namespace SocialApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController(ICommentService commentService, IMapper mapper) : ControllerBase
{
    [HttpGet($"{nameof(GetAllComments)}")]
    public async Task<ActionResult<CommentResponseDTO>> GetAllComments()
    {
        List<CommentModel> comments = await commentService.GetAllCommentsAsync();
        List<CommentResponseDTO> commentResponseDTO = mapper.Map<List<CommentResponseDTO>>(comments);
        return Ok(commentResponseDTO);
    }

    [HttpGet($"{nameof(GetCommentByIdWithNavProps)}/{{id}}")]
    public async Task<IActionResult> GetCommentByIdWithNavProps(int id, bool includeUser = false, bool includePost = false)
    {
        CommentModel? comment = await commentService.GetCommentByIdWithNavPropsAsync(id, includeUser, includePost);
        if (comment == null) return NotFound();
        if (includeUser == false && includePost == false)
        {
            CommentResponseDTO commentResponseDTO = mapper.Map<CommentResponseDTO>(comment);
            return Ok(commentResponseDTO);
        }
        return Ok(comment);
    }

    [HttpPost($"{nameof(CreateComment)}")]
    public async Task<CreatedAtActionResult> CreateComment([FromBody] CommentCreateDTO commentCreateDto)
    {
        CommentModel comment = await commentService.CreateCommentAsync(commentCreateDto);
        CommentResponseDTO commentResponseDTO = mapper.Map<CommentResponseDTO>(comment);
        return CreatedAtAction(nameof(GetCommentByIdWithNavProps), new { id = comment.Id }, commentResponseDTO);
    }

    [HttpPut($"{nameof(UpdateComment)}/{{id}}")]
    public async Task<ActionResult<CommentResponseDTO>> UpdateComment(int id, [FromBody] CommentUpdateDTO commentDto)
    {
        CommentModel updatedComment = await commentService.UpdateCommentAsync(id, commentDto);
        CommentResponseDTO commentResponseDTO = mapper.Map<CommentResponseDTO>(updatedComment);
        return Ok(commentResponseDTO);
    }

    [HttpDelete($"{nameof(DeleteComment)}/{{id}}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        bool commentHasBeenDelete = await commentService.DeleteCommentAsync(id);
        return commentHasBeenDelete ? NoContent() : NotFound();
    }
}