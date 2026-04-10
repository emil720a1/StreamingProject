using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Chat.CharService;
using StreamingProject.Contracts.Chat.CrudDtos;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatService chatService, ICurrentUser currentUser) : ApiControllerBase
{
    [Authorize(Policy = "Permission.Read")]
    [HttpGet("messages")]
    public async Task<IActionResult> GetChatMessages(
        [FromQuery] GetChatMessagesDto getChatMessagesDto,
        CancellationToken cancellationToken)
    {
        var messages = await chatService.GetChatMessagesAsync(getChatMessagesDto, cancellationToken);
        return HandleResult(messages);
    }

    [Authorize(Policy = "Permission.Create")]
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageDto sendMessageDto,
        CancellationToken cancellationToken)
    {
        var message = await chatService.SendMessageAsync(sendMessageDto, currentUser.Id, cancellationToken);
        return HandleResult(message);
    }

    [Authorize(Policy = "Permission.Delete")] 
    [HttpDelete("message")]
    public async Task<IActionResult> DeleteChatMessages(
        [FromBody] DeleteMessageDto deleteMessageDto,
        CancellationToken cancellationToken)
    {
        var result = await chatService.DeleteChatMessageAsync(
            deleteMessageDto.Id,
            currentUser.Id, 
            cancellationToken);

        return HandleResult(result);
    }
}