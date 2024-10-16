using System.ComponentModel.DataAnnotations;

namespace SocialApp.Models;

public class CommentModel
{
    // PK
    public int Id { get; set; }
    [MaxLength(200)]
    public required string Text { get; set; }

    // FK
    public required int PostId { get; set; }
    public required int UserId { get; set; }

    // Nav
    public PostModel Post { get; set; } = null!;
    public UserModel User { get; set; } = null!;
}