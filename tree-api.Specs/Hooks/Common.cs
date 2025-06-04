using BoDi;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using tree_api.Database;
using tree_api.Specs.Contexts;

namespace tree_api.Specs.Hooks;

[Binding]
public class Common
{
    private readonly IObjectContainer _container;

    public Common(IObjectContainer container)
    {
        _container = container;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var ctx = new TestContext();
        _container.RegisterInstanceAs(ctx);
        RunMigrations(ctx);
    }

    private static void RunMigrations(TestContext ctx)
    {
        ctx.Server.Services
            .GetRequiredService<MigrationsChecker>()
            .CheckAndRunMigrations(default)
            .GetAwaiter()
            .GetResult();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _container.Resolve<TestContext>().Server.Dispose();
    }
}
