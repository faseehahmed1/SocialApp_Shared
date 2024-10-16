namespace SocialApp.DTOs;

public class UserResponseDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}