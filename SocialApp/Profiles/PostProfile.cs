using AutoMapper;
using SocialApp.DTOs;
using SocialApp.Models;

namespace SocialApp.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostModel, PostResponseDTO>();
    }
}