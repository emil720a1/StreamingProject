using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.Stream.UserStream;
using StreamingProject.Domain.User;
using StreamingProject.Repository;
using StreamingProject.Repository.Repositories.StreamRepositories;
using Testcontainers.PostgreSql;

namespace StreamingProject.IntegrationTests;

[TestFixture]
[NonParallelizable]
public sealed class StreamRepositoryPaginationTests
{
    private PostgreSqlContainer _postgres = null!;

    [OneTimeSetUp]
    public async Task StartPostgreSql()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();
    }

    [OneTimeTearDown]
    public async Task StopPostgreSql()
    {
        await _postgres.DisposeAsync();
    }

    [Test]
    public async Task GetAvailableStreamsAsync_ShouldFilterSortAndPaginateInPostgreSql()
    {
        var connectionString = await CreateDatabaseAsync();
        await using var context = CreateContext(connectionString);
        await context.Database.MigrateAsync();

        var userRole = await context.Roles.SingleAsync(role => role.Name == "User");
        var alex = UserEntity.Create(
            "alex",
            "password-hash",
            "alex@example.com",
            userRole,
            "Alex",
            "Streamer");
        var maria = UserEntity.Create(
            "maria",
            "password-hash",
            "maria@example.com",
            userRole,
            "Maria",
            "Viewer");

        context.Users.AddRange(alex, maria);

        var smallerGamingStream = CreateLiveStream(
            alex.Id,
            "Morning gameplay",
            "Gaming");
        var largerGamingStream = CreateLiveStream(
            alex.Id,
            "Evening gameplay",
            "Gaming");
        var musicStream = CreateLiveStream(
            maria.Id,
            "Special music session",
            "Music");
        var endedStream = CreateLiveStream(
            alex.Id,
            "Old gameplay",
            "Gaming");
        endedStream.EndStream();

        context.Streams.AddRange(
            smallerGamingStream,
            largerGamingStream,
            musicStream,
            endedStream);
        context.UserStreams.AddRange(
            UserStream.Create(alex.Id, smallerGamingStream.Id),
            UserStream.Create(alex.Id, largerGamingStream.Id),
            UserStream.Create(maria.Id, largerGamingStream.Id));
        await context.SaveChangesAsync();

        var repository = new StreamRepositories(context);
        var firstPage = await repository.GetAvailableStreamsAsync(
            new GetStreamsRequestDto
            {
                Page = 1,
                PageSize = 1,
                Category = "gaming",
                Search = "ALEX",
                SortBy = "viewerCount",
                SortDirection = "asc",
            },
            CancellationToken.None);

        firstPage.TotalCount.Should().Be(2);
        firstPage.Items.Should().ContainSingle();
        firstPage.Items[0].Id.Should().Be(smallerGamingStream.Id);
        firstPage.Items[0].ViewerCount.Should().Be(1);

        var secondPage = await repository.GetAvailableStreamsAsync(
            new GetStreamsRequestDto
            {
                Page = 2,
                PageSize = 1,
                Category = "Gaming",
                Search = "alex",
                SortBy = "viewerCount",
                SortDirection = "asc",
            },
            CancellationToken.None);

        secondPage.TotalCount.Should().Be(2);
        secondPage.Items.Should().ContainSingle();
        secondPage.Items[0].Id.Should().Be(largerGamingStream.Id);
        secondPage.Items[0].ViewerCount.Should().Be(2);

        var titleSearch = await repository.GetAvailableStreamsAsync(
            new GetStreamsRequestDto { Search = "special MUSIC" },
            CancellationToken.None);

        titleSearch.TotalCount.Should().Be(1);
        titleSearch.Items[0].Id.Should().Be(musicStream.Id);
    }

    private async Task<string> CreateDatabaseAsync()
    {
        var databaseName = $"streaming_{Guid.NewGuid():N}";
        var connectionStringBuilder =
            new NpgsqlConnectionStringBuilder(_postgres.GetConnectionString())
            {
                Database = "postgres",
            };

        await using var connection =
            new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE \"{databaseName}\"";
        await command.ExecuteNonQueryAsync();

        connectionStringBuilder.Database = databaseName;
        return connectionStringBuilder.ConnectionString;
    }

    private static StreamingDbContext CreateContext(string connectionString)
    {
        var dbContextOptions = new DbContextOptionsBuilder<StreamingDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        var authorizationOptions = Options.Create(new AuthorizationOptions
        {
            RolePermissions =
            [
                new RolePermissions
                {
                    Role = "Admin",
                    Permissions = ["Create", "Read", "Update", "Delete"],
                },
                new RolePermissions
                {
                    Role = "User",
                    Permissions = ["Read", "Create"],
                },
            ],
        });

        return new StreamingDbContext(dbContextOptions, authorizationOptions);
    }

    private static StreamEntity CreateLiveStream(
        Guid userId,
        string title,
        string category)
    {
        var stream = StreamEntity.Create(
            userId,
            title,
            $"{title} description",
            category,
            null);
        stream.StartStream();
        return stream;
    }
}
