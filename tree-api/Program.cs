using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using tree_api.API.Middlewares;
using tree_api.Configuration;
using tree_api.Database;
using tree_api.Domain.Services.ExceptionHandler;
using tree_api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Logging */
Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .ReadFrom.Configuration(builder.Configuration)
           .CreateLogger();
builder.Services.AddSerilog();

builder.Services.AddControllers();

/* Configs */
builder.Services.AddOptionsWithValidateOnStart<DatabaseConfiguration>()
                        .BindConfiguration(nameof(DatabaseConfiguration))
                        .ValidateDataAnnotations();

/* Database */
builder.Services.AddDbContext<DbCtx>((sp, options) =>
{
    var config = sp.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
    options.UseNpgsql(config.ConnectionString);
});

/* Services */
builder.Services.AddScoped<IExceptionHandler, ExceptionHandler>();
builder.Services.AddScoped<MigrationsChecker>();

var app = builder.Build();

/* Migrations */
if (app.Environment.ApplicationName.IsMigrationOption() ||
    args.Any(s => s.IsMigrationOption()))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<MigrationsChecker>().CheckAndRunMigrations(CancellationToken.None);

    return;
}

/* Middlewares */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionLoggingMiddleware>();

app.MapControllers();

app.Run();
