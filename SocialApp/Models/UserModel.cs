using System.ComponentModel.DataAnnotations;

namespace SocialApp.Models;

public class UserModel
{
    // PK
    public int Id { get; set; }
    [MaxLength(25)]
    public required string Name { get; set; }
    [MaxLength(25)]
    public required string Email { get; set; }

    // Nav
    public List<PostModel> Posts { get; set; } = [];
    public List<CommentModel> Comments { get; set; } = [];
}