namespace SocialApp.DTOs;

public class CommentResponseDTO()
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public required int PostId { get; set; }
    public required int UserId { get; set; }
}