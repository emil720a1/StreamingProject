using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Shared;

namespace StreamingProject.Application.Interfaces.Storage;

public interface IStorageService
{
    Task<Result<string, Failure>> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
    Task<Result<bool, Failure>> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}
