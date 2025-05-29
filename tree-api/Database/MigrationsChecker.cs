using Microsoft.EntityFrameworkCore;

namespace tree_api.Database;

internal sealed class MigrationsChecker
{
    private readonly DbCtx _ctx;
    private readonly ILogger<MigrationsChecker> _logger;

    public MigrationsChecker(DbCtx ctx, ILogger<MigrationsChecker> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task CheckAndRunMigrations(CancellationToken token)
    {
        await _ctx.Database.EnsureCreatedAsync();

        if (!(await _ctx.Database.GetPendingMigrationsAsync(token)).Any())
        {
            _logger.LogInformation("No pending migrations.");
            return;
        }

        _logger.LogInformation("Applying migrations...");
        await _ctx.Database.MigrateAsync(token);
        _logger.LogInformation("Migrations applied.");
    }
}
