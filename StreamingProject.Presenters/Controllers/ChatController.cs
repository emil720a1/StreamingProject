using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Chat.CharService;
using StreamingProject.Contracts.Chat.CrudDtos;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("messages")]
    public async Task<IActionResult> GetChatMessages(
        [FromQuery] GetChatMessagesDto getChatMessagesDto,
        CancellationToken cancellationToken)
    {
       var messages =  await _chatService.GetChatMessagesAsync(getChatMessagesDto, cancellationToken);
       return messages.IsFailure ? messages.Error.ToResponse() : Ok(messages.Value);
    }

    [Authorize(Policy = "Permission.Create")]
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageDto sendMessageDto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();

        if (userId == Guid.Empty) return Unauthorized(); 
        
        var message = await _chatService.SendMessageAsync(sendMessageDto, userId, cancellationToken);
        
        return message.IsFailure ? message.Error.ToResponse() : Ok(message.Value);
    }

    [Authorize(Policy = "Permission.Delete")] 
    [HttpDelete("message")]
    public async Task<IActionResult> DeleteChatMessages(
        [FromBody] DeleteMessageDto deleteMessageDto,
        CancellationToken cancellationToken)
    {

        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty) return Unauthorized();
        
        var result = await _chatService.DeleteChatMessageAsync(
            deleteMessageDto.Id,
            userId, 
            cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok (result.Value);
    }

    private Guid GetUserIdFromClaims()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        return (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            ? userId
            : Guid.Empty;
    }
}