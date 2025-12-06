using AutoMapper;
using StreamingProject.Contracts.Chat;
using StreamingProject.Domain;
using StreamingProject.Domain.Chat;

namespace Shared.Common;

public class ChatMapper : Profile
{
    public ChatMapper()
    {
        CreateMap<ChatEntity, ChatDetailsDto>()
            .ForMember(d => d.Id,
                opt => opt.MapFrom(s => s.Id))

            .ForMember(s => s.Message,
                opt => opt.MapFrom(s => s.Id))

            .ForMember(t => t.SentAt,
                opt => opt.MapFrom(s => s.SentTime))

            .ForMember(u => u.UserId,
                opt => opt.MapFrom(s => s.UserId));
        
        CreateMap<ChatDetailsDto, ChatEntity>();
    }
}