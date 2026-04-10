using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Shared;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T, Failure> result)
    {
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}
