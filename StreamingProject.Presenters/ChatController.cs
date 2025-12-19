using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Chat.CharService;
using StreamingProject.Contracts.Chat;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters;


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
    [HttpGet("get")]
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
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }
        
        var message = await _chatService.SendMessageAsync(sendMessageDto, userId, cancellationToken);
        
        return message.IsFailure ? message.Error.ToResponse() : Ok(message.Value);
    }

    // [Authorize(Policy = "Permission.Delete")] 
    [Authorize]
    [HttpPost("delete")]
    public async Task<IActionResult> DeleteChatMessages(
        [FromBody] DeleteMessageDto deleteMessageDto,
        CancellationToken cancellationToken)
    {
        
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }
        
        var messageToDelete = await _chatService.DeleteChatMessageAsync(deleteMessageDto, userId, cancellationToken);

        return messageToDelete.IsFailure ? messageToDelete.Error.ToResponse() : Ok (messageToDelete.Value);
    }
}