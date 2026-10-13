using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using StreamingProject.Presenters.Hubs;

namespace StreamingProject.UnitTests;

[TestFixture]
public sealed class ChatHubTests
{
    private const string ConnectionId = "connection-1";
    private const string StreamId = "stream-1";

    private Mock<ILogger<ChatHub>> _loggerMock = null!;
    private Mock<HubCallerContext> _contextMock = null!;
    private Mock<IGroupManager> _groupsMock = null!;
    private ChatHub _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<ChatHub>>();
        _contextMock = new Mock<HubCallerContext>();
        _contextMock.SetupGet(context => context.ConnectionId)
            .Returns(ConnectionId);
        _groupsMock = new Mock<IGroupManager>();
        _groupsMock.Setup(groups => groups.AddToGroupAsync(
                ConnectionId,
                StreamId,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _groupsMock.Setup(groups => groups.RemoveFromGroupAsync(
                ConnectionId,
                StreamId,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new ChatHub(_loggerMock.Object)
        {
            Context = _contextMock.Object,
            Groups = _groupsMock.Object,
        };
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Dispose();
    }

    [Test]
    public async Task OnConnectedAsync_ShouldLogConnection()
    {
        await _sut.OnConnectedAsync();

        VerifyLog(LogLevel.Information, "SignalR client connected");
    }

    [Test]
    public async Task OnDisconnectedAsync_ShouldLogNormalDisconnection()
    {
        await _sut.OnDisconnectedAsync(null);

        VerifyLog(LogLevel.Information, "SignalR client disconnected");
    }

    [Test]
    public async Task OnDisconnectedAsync_ShouldLogDisconnectException()
    {
        var exception = new InvalidOperationException("connection failed");

        await _sut.OnDisconnectedAsync(exception);

        VerifyLog(
            LogLevel.Warning,
            "SignalR client disconnected with error");
    }

    [Test]
    public async Task JoinStreamGroup_ShouldAddConnectionOnceAndLog()
    {
        await _sut.JoinStreamGroup(StreamId);

        _groupsMock.Verify(groups => groups.AddToGroupAsync(
                ConnectionId,
                StreamId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        VerifyLog(LogLevel.Information, "SignalR client joined stream group");
    }

    [Test]
    public async Task LeaveStreamGroup_ShouldRemoveConnectionOnceAndLog()
    {
        await _sut.LeaveStreamGroup(StreamId);

        _groupsMock.Verify(groups => groups.RemoveFromGroupAsync(
                ConnectionId,
                StreamId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        VerifyLog(LogLevel.Information, "SignalR client left stream group");
    }

    private void VerifyLog(LogLevel level, string message)
    {
        _loggerMock.Verify(logger => logger.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains(message, StringComparison.Ordinal)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
