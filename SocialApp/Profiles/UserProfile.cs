using AutoMapper;
using SocialApp.DTOs;
using SocialApp.Models;

namespace SocialApp.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserModel, UserResponseDTO>();
    }
}