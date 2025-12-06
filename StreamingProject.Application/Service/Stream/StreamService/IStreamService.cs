using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.Application.Service.Stream.StreamService;

public interface IStreamService
{

    /// <summary>
    /// Створення  стріму
    /// </summary>
    /// <returns></returns>
    Task<Result<StreamDetailsDto, Failure>> CreateStreamAsync(CreateStreamDto streamDto, CancellationToken cancellationToken);
    
    
    
    /// <summary>
    /// Приєднатися до стріму
    /// </summary>
    /// <param name="streamId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Result<StreamDetailsDto, Failure>> JoinStreamAsync(JoinStreamDto streamDto, CancellationToken cancellationToken);
    
    
    
    Task<Result<StreamDetailsDto, Failure>> GetStreamByIdAsync(GetStreamByIdDto streamDto, CancellationToken cancellationToken);
    
    
}