using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using tree_api.Configuration;
using tree_api.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptionsWithValidateOnStart<DatabaseConfiguration>()
                        .BindConfiguration(nameof(DatabaseConfiguration))
                        .ValidateDataAnnotations();

builder.Services.AddDbContext<DbCtx>((sp, options) =>
{
    var config = sp.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
    options.UseNpgsql(config.ConnectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
