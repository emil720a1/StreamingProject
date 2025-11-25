
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using StreamingProject.Application;
using StreamingProject.Repository;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddProgramDependencies();

services.AddAutoMapper(typeof(StreamMapper));

services.AddDbContext<StreamingDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(StreamingDbContext)));
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "StreamingProject.Web"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();