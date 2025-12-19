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
            .ConstructUsing(src => new ChatDetailsDto(
                src.Id,
                src.Message,
                src.SentTime,
                src.UserId
                ));
    }
}