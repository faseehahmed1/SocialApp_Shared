namespace SocialApp.DTOs.Response;

public class PostResponseDTO
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required int UserId { get; set; }
}