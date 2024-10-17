using System.ComponentModel.DataAnnotations;

namespace SocialApp.Models;

public class PostModel
{
    // PK
    public int Id { get; set; }
    [MaxLength(80)]
    public required string Title { get; set; }
    [MaxLength(500)]
    public required string Content { get; set; }

    // FK
    public required int UserId { get; set; }

    // Nav
    public UserModel User { get; set; } = null!; // Use null-forgiving operator if you know User will be initialized properly
    public List<CommentModel> Comments { get; set; } = [];
}