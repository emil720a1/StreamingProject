using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain;

namespace StreamingProject.Application.Service;

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
    Task<StreamDetailsDto> JoinStreamAsync(Guid streamId, Guid userId ,CancellationToken cancellationToken);
}