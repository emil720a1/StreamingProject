using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.Stream.UserStream;

namespace StreamingProject.Repository.Repositories.StreamRepositories;

public class StreamRepositories : IStreamRepository
{
    private readonly StreamingDbContext _dbContext;

    public StreamRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<StreamEntity> AddStreamAsync(StreamEntity stream)
    {
        await _dbContext.Streams.AddAsync(stream);
        await _dbContext.SaveChangesAsync();

        return stream;
    }

    public async Task<StreamEntity> UpdateStreamAsync(StreamEntity stream)
    {
        await _dbContext.SaveChangesAsync();
        return stream;
    }

    public async Task<StreamEntity?> GetStreamByIdAsync(Guid id)
    {
        return await _dbContext.Streams
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.VideoEntity)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<StreamEntity?> GetActiveStream(Guid userId)
    {
        return await _dbContext.Streams
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.EndTime == null)
            .FirstOrDefaultAsync();
    }

    public async Task<StreamPage> GetAvailableStreamsAsync(
        GetStreamsRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Streams
            .AsNoTracking()
            .Where(stream => stream.EndTime == null);

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var category = request.Category.Trim().ToLowerInvariant();
            query = query.Where(stream => stream.Category.ToLower() == category);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{EscapeLikePattern(request.Search.Trim())}%";

            query = query.Where(stream =>
                EF.Functions.ILike(stream.Title, searchPattern, "\\")
                || (stream.User.UserName != null
                    && EF.Functions.ILike(stream.User.UserName, searchPattern, "\\")));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var sortDescending = request.SortDirection.Equals(
            "desc",
            StringComparison.OrdinalIgnoreCase);

        var orderedQuery = request.SortBy.Equals(
            "viewerCount",
            StringComparison.OrdinalIgnoreCase)
            ? sortDescending
                ? query.OrderByDescending(stream => stream.Participants.Count)
                    .ThenBy(stream => stream.Id)
                : query.OrderBy(stream => stream.Participants.Count)
                    .ThenBy(stream => stream.Id)
            : sortDescending
                ? query.OrderByDescending(stream => stream.StartTime)
                    .ThenBy(stream => stream.Id)
                : query.OrderBy(stream => stream.StartTime)
                    .ThenBy(stream => stream.Id);

        var items = await orderedQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(stream => new StreamListItemReadModel(
                stream.Id,
                stream.UserId,
                stream.User.UserName,
                stream.Title,
                stream.Description,
                stream.StartTime,
                stream.Category,
                stream.ThumbnailUrl,
                stream.Participants.Count))
            .ToListAsync(cancellationToken);

        return new StreamPage(items, totalCount);
    }

    private static string EscapeLikePattern(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("%", "\\%", StringComparison.Ordinal)
        .Replace("_", "\\_", StringComparison.Ordinal);

    public async Task<List<StreamEntity>> GetStreamsByUserId(Guid userId)
    {

        return await _dbContext.Streams
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<StreamEntity?> GetStreamByKeyAsync(string streamKey)
    {
        return await _dbContext.Streams
            .FirstOrDefaultAsync(s => s.StreamKey == streamKey);
    }

    public async Task<bool> HasJoinedStreamAsync(Guid streamId, Guid userId)
    {
        return await _dbContext.UserStreams
            .AsNoTracking()
            .AnyAsync(s => s.StreamId == streamId && s.UserId == userId);
    }

    public async Task<bool> AddParticipantAsync(UserStream userStream)
    {
        await _dbContext.UserStreams.AddAsync(userStream);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveParticipantAsync(Guid streamId, Guid userId)
    {
        if (streamId == Guid.Empty || userId == Guid.Empty) return false;

        var deletedCount =  await _dbContext.UserStreams
            .Where(s => s.StreamId == streamId && s.UserId == userId)
            .ExecuteDeleteAsync();

        return deletedCount > 0;
    }

    public async Task<bool> CheckStreamKeyExistsAsync(string streamKey)
    {
        return await _dbContext.Streams.AnyAsync(s => s.StreamKey == streamKey);
    }
}
