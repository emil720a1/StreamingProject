using StreamingProject.Application;
using StreamingProject.Domain;

namespace StreamingProject.Repository.Repositories;

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