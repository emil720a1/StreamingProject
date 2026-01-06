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
    Task<Result<StreamDetailsDto, Failure>> CreateStreamAsync(
        CreateStreamDto streamDto,
        CancellationToken cancellationToken);
    
    
    
    /// <summary>
    /// Приєднатися до стріму
    /// </summary>
    Task<Result<StreamDetailsDto, Failure>> JoinStreamAsync(JoinStreamDto streamDto, CancellationToken cancellationToken);
    
    
    
    Task<Result<StreamDetailsDto, Failure>> GetStreamByIdAsync(GetStreamByIdDto streamDto, CancellationToken cancellationToken);
    
    
    
    /// <summary>
    /// Перевірка ключа трансляціх перед початком стріму
    /// </summary>
    Task<Result<bool, Failure>> ValidateStreamKeyAsync(string streamKey, CancellationToken cancellationToken);
    
    
}