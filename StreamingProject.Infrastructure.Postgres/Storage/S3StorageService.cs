using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared;
using StreamingProject.Application.Interfaces.Storage;

namespace StreamingProject.Infrastructure.Postgres.Storage;

public class S3StorageService : IStorageService
{
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(ILogger<S3StorageService> logger)
    {
        _logger = logger;
    }

    public Task<Result<string, Failure>> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual S3 upload. For now, just logging and returning a fake URL.
        _logger.LogInformation("Simulating S3 upload for {FileName}", file.FileName);
        
        var fakeUrl = $"https://s3.amazonaws.com/fakebucket/{folderName}/{file.FileName}";
        return Task.FromResult(Result.Success<string, Failure>(fakeUrl));
    }

    public Task<Result<bool, Failure>> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual S3 delete. For now, just logging and returning true.
        _logger.LogInformation("Simulating S3 delete for {FileUrl}", fileUrl);
        return Task.FromResult(Result.Success<bool, Failure>(true));
    }
}
