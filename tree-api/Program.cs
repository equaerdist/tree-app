using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Reflection;
using tree_api.API.Middlewares;
using tree_api.Configuration;
using tree_api.Database;
using tree_api.Domain.Services.ExceptionHandler;
using tree_api.Domain.Services.JournalService;
using tree_api.Domain.Services.NodeService;
using tree_api.Domain.Services.TreeService;
using tree_api.Extensions;

namespace tree_api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var app = builder.Build();

        // Migrations
        if (app.Environment.ApplicationName.IsMigrationOption() ||
            args.Any(s => s.IsMigrationOption()))
        {
            using var scope = app.Services.CreateScope();
            await scope.ServiceProvider
                       .GetRequiredService<MigrationsChecker>()
                       .CheckAndRunMigrations(CancellationToken.None);
            return;
        }

        // Middleware pipeline
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseMiddleware<ExceptionLoggingMiddleware>();

        app.MapControllers();

        await app.RunAsync();
    }

    public static WebApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Logging
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Host.UseSerilog();

        // Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddOptionsWithValidateOnStart<DatabaseConfiguration>()
                        .BindConfiguration(nameof(DatabaseConfiguration))
                        .ValidateDataAnnotations();

        builder.Services.AddDbContext<DbCtx>((sp, options) =>
        {
            var config = sp.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            options.UseNpgsql(config.ConnectionString);
        });

        builder.Services.AddScoped<ITreeService, TreeService>();
        builder.Services.AddScoped<IJournalService, JournalService>();
        builder.Services.AddScoped<INodeService, NodeService>();
        builder.Services.AddScoped<IExceptionHandler, ExceptionHandler>();
        builder.Services.AddScoped<MigrationsChecker>();

        return builder;
    }
}
