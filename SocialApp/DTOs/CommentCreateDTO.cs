
namespace SocialApp.DTOs;

public class CommentCreateDTO
{
    public required string Text { get; set; }
    public required int PostId { get; set; }
    public required int UserId { get; set; }
}