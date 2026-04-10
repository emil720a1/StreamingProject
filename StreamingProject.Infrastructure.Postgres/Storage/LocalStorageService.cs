using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared;
using StreamingProject.Application.Interfaces.Storage;

namespace StreamingProject.Infrastructure.Postgres.Storage;

public class LocalStorageService : IStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<LocalStorageService> _logger;

    public LocalStorageService(IWebHostEnvironment env, ILogger<LocalStorageService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<Result<string, Failure>> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return Failure.FromError(Error.Validation("File.Empty", "The uploaded file is empty or missing."));
        }

        try
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream, cancellationToken);
            }

            var fileUrl = $"/{folderName}/{uniqueFileName}";
            return Result.Success<string, Failure>(fileUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to {FolderName}", file.FileName, folderName);
            return Failure.FromError(Error.Failure("Storage.UploadFailed", ex.Message));
        }
    }

    public Task<Result<bool, Failure>> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return Task.FromResult(Result.Success<bool, Failure>(true));

            // remove leading slash
            var relativePath = fileUrl.TrimStart('/');
            var filePath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.FromResult(Result.Success<bool, Failure>(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileUrl}", fileUrl);
            return Task.FromResult(Result.Failure<bool, Failure>(Failure.FromError(Error.Failure("Storage.DeleteFailed", ex.Message))));
        }
    }
}
