using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tree.api.Tests.ClientGenerator;
using tree_api.Configuration;
using tree_api.Database;
using tree_api.Tests.ClientGenerator.Utils;

namespace tree_api.Tests.Setup;

public class TestFixture
{
    private readonly TestServer _server;

    public readonly TreeAppClient TreeAppClient;

    public TestFixture()
    {
        _server = CreateServer();

        var client = _server.CreateClient();
        TreeAppClient = new(client.BaseAddress!.ToString(), client);

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Local");
        RunMigrations().GetAwaiter().GetResult();
        CleanDb().GetAwaiter().GetResult();
    }

    private async Task RunMigrations()
    {
        await Program.Main(["--migrate"]);
    }

    public IServiceScope CreateScope() => _server.Services.CreateScope();

    public static TestServer CreateServer(Action<IServiceCollection>? configureServices = null)
    {
#if DEBUG
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Local");
#endif
        var factory = new CustomWebApplicationFactory().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(options =>
            {
                configureServices?.Invoke(options);
            });
        });

        return factory.Server;
    }

    public static async Task CleanDb()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{environment}.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var dbCfg = configuration
            .GetSection(nameof(DatabaseConfiguration))
            .Get<DatabaseConfiguration>() ?? throw new ArgumentNullException(nameof(DatabaseConfiguration));

        var optionsBuilder = new DbContextOptionsBuilder<DbCtx>()
            .UseNpgsql(dbCfg.ConnectionString);

        await using var dbCtx = new DbCtx(optionsBuilder.Options);

        var tableNames = await dbCtx.Database
            .SqlQueryRaw<string>("""
                                SELECT tablename
                                FROM pg_tables
                                WHERE schemaname = 'public' AND tablename != '__EFMigrationsHistory';
                                """)
            .ToListAsync();

        foreach (var table in tableNames.Where(s => !s.Equals("__EFMigrationsHistory", StringComparison.OrdinalIgnoreCase)))
        {
            var sql = $"TRUNCATE TABLE \"{table}\" CASCADE;";
            await dbCtx.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
