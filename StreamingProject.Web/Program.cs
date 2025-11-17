
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using StreamingProject.Repository;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(StreamMapper));

builder.Services.AddDbContext<StreamingDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(StreamingDbContext)));
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();