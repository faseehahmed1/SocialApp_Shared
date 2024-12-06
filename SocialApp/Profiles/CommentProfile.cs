using AutoMapper;
using SocialApp.DTOs.Response;
using SocialApp.Models;

namespace SocialApp.Profiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CommentModel, CommentResponseDTO>();
    }
}
