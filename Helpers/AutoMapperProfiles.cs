using AutoMapper;
using Graduation.Data;
using Graduation.DTOs;
using Graduation.Entities;
using Graduation.Extensions;

namespace Graduation.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
{
    CreateMap<AppUser, MemberDto>()
        .ForMember(dest => dest.PhotoUrl,
            opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
        .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
        .ForMember(dest => dest.Role,                         // ✅ السطر ده هو الجديد
        opt => opt.MapFrom(src => src.Role));
    CreateMap<Photo, PhotoDto>();
    CreateMap<MemberUpdateDto, AppUser>();
    CreateMap<RegisterDto, AppUser>()
        .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
        .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName)) // جديد
        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));  // جديد

 CreateMap<Message, MessageDto>()
    .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos
        .FirstOrDefault(x => x.IsMain).Url))
    .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos
        .FirstOrDefault(x => x.IsMain).Url))
    .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.ImageUrl))
    .ForMember(d => d.MessageType, o => o.MapFrom(s => s.MessageType));


    CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
    CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ?
        DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
}
}
}
