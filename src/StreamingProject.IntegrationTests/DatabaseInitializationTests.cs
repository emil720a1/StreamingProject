using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;
using StreamingProject.Repository;
using StreamingProject.Repository.Repositories.RoleRepositories;
using StreamingProject.Repository.Repositories.UserRepositories;
using Testcontainers.PostgreSql;

namespace StreamingProject.IntegrationTests;

[TestFixture]
[NonParallelizable]
public sealed class DatabaseInitializationTests
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
    public async Task InitializeAsync_ShouldApplyAllMigrationsToCleanDatabase()
    {
        var connectionString = await CreateDatabaseAsync();
        await using var context = CreateContext(connectionString);
        var initializer = CreateInitializer(
            context,
            new RoleSeeder(),
            new UserSeeder());

        await initializer.InitializeAsync(CancellationToken.None);

        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
        var knownMigrations = context.Database.GetMigrations();

        appliedMigrations.Should().BeEquivalentTo(knownMigrations);
        context.Database.HasPendingModelChanges().Should().BeFalse();
        (await context.Users.CountAsync(user => user.UserName == "admin"))
            .Should().Be(1);
    }

    [Test]
    public async Task InitializeAsync_ShouldApplyPendingMigrationToExistingDatabase()
    {
        var connectionString = await CreateDatabaseAsync();
        await using var context = CreateContext(connectionString);
        var migrations = context.Database.GetMigrations().ToArray();
        migrations.Should().HaveCountGreaterThan(1);

        var migrator = context.Database.GetService<IMigrator>();
        await migrator.MigrateAsync(migrations[0]);

        (await context.Database.GetAppliedMigrationsAsync())
            .Should().ContainSingle()
            .Which.Should().Be(migrations[0]);

        var initializer = CreateInitializer(context);
        await initializer.InitializeAsync(CancellationToken.None);

        (await context.Database.GetAppliedMigrationsAsync())
            .Should().BeEquivalentTo(migrations);
        (await context.Roles.CountAsync()).Should().Be(2);
    }

    [Test]
    public async Task InitializeAsync_ShouldNotDuplicateSeedDataWhenRunTwice()
    {
        var connectionString = await CreateDatabaseAsync();

        await using (var firstContext = CreateContext(connectionString))
        {
            await CreateInitializer(
                    firstContext,
                    new RoleSeeder(),
                    new UserSeeder())
                .InitializeAsync(CancellationToken.None);
        }

        await using (var secondContext = CreateContext(connectionString))
        {
            await CreateInitializer(
                    secondContext,
                    new RoleSeeder(),
                    new UserSeeder())
                .InitializeAsync(CancellationToken.None);

            (await secondContext.Roles.CountAsync()).Should().Be(2);
            (await secondContext.Users.CountAsync(user => user.UserName == "admin"))
                .Should().Be(1);
            (await secondContext.UserRoles.CountAsync()).Should().Be(1);
        }
    }

    [Test]
    public async Task InitializeAsync_ShouldFailWhenDatabaseCannotBeReached()
    {
        const string unreachableDatabase =
            "Host=127.0.0.1;Port=1;Database=streaming;Username=postgres;Password=postgres;Timeout=1;Command Timeout=1";
        await using var context = CreateContext(unreachableDatabase);
        var seeder = new TrackingSeeder();
        var initializer = CreateInitializer(context, seeder);

        var initialize = () => initializer.InitializeAsync(CancellationToken.None);

        await initialize.Should().ThrowAsync<Exception>();
        seeder.WasCalled.Should().BeFalse();
    }

    [Test]
    public async Task InitializeAsync_ShouldFailAndSkipSeedersWhenMigrationFails()
    {
        var connectionString = await CreateDatabaseAsync();
        await using var context = CreateContext(connectionString);
        var migrations = context.Database.GetMigrations().ToArray();
        var migrator = context.Database.GetService<IMigrator>();
        await migrator.MigrateAsync(migrations[0]);

        await context.Database.ExecuteSqlRawAsync(
            "ALTER TABLE \"Streams\" ADD COLUMN \"Category\" text NOT NULL DEFAULT ''");

        var seeder = new TrackingSeeder();
        var initializer = CreateInitializer(context, seeder);
        var initialize = () => initializer.InitializeAsync(CancellationToken.None);

        await initialize.Should().ThrowAsync<Exception>();
        seeder.WasCalled.Should().BeFalse();
        (await context.Database.GetAppliedMigrationsAsync())
            .Should().ContainSingle()
            .Which.Should().Be(migrations[0]);
    }

    [Test]
    public async Task InitializeAsync_ShouldRunSeedersOnlyAfterMigrations()
    {
        var connectionString = await CreateDatabaseAsync();
        await using var context = CreateContext(connectionString);
        var seeder = new MigrationAwareSeeder();
        var initializer = CreateInitializer(context, seeder);

        await initializer.InitializeAsync(CancellationToken.None);

        seeder.WasCalled.Should().BeTrue();
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

    private static DbInitializer CreateInitializer(
        StreamingDbContext context,
        params ISeeder[] seeders)
    {
        return new DbInitializer(
            context,
            seeders,
            NullLogger<DbInitializer>.Instance);
    }

    private sealed class TrackingSeeder : ISeeder
    {
        public bool WasCalled { get; private set; }

        public Task SeedAsync(StreamingDbContext context)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class MigrationAwareSeeder : ISeeder
    {
        public bool WasCalled { get; private set; }

        public async Task SeedAsync(StreamingDbContext context)
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            pendingMigrations.Should().BeEmpty();

            (await context.Roles.AnyAsync()).Should().BeTrue();
            WasCalled = true;
        }
    }
}
