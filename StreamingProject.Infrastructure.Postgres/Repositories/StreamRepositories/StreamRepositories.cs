using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Domain;
using StreamingProject.Domain.Stream;

namespace StreamingProject.Repository.Repositories.StreamRepositories;

public class StreamRepositories : IStreamRepository
{
    private readonly StreamingDbContext _dbContext;

    public StreamRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Task<StreamEntity> AddStreamAsync(StreamEntity stream)
    {
        throw new NotImplementedException();
    }

    public Task<StreamEntity> UpdateStreamAsync(StreamEntity stream)
    {
        throw new NotImplementedException();
    }

    public Task<StreamEntity> GetStreamByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<StreamEntity> GetActiveStream(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<StreamEntity>> GetStreamsByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }
}