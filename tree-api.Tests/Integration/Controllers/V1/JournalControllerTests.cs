using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tree_api.Domain.Services.ExceptionHandler;
using tree_api.Tests.Setup;
using Xunit;

namespace tree_api.Tests.Integration.Controllers.V1;

public class JournalControllerTests : TestBase
{
    public JournalControllerTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetRange_WhenErrorOcurred_ShouldCreate()
    {
        //Arrange
        using var scope = CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IExceptionHandler>();
        var exception = new Exception(Faker.Random.String2(10));
        await handler.HandleAsync(exception);

        //Act
        var actual = await TreeAppClient.GetRangeAsync(skip: 0, take: 1, body: null, cancellationToken: CancellationToken);

        //Assert
        actual.Items.Should().HaveCount(1);
        var actualError = actual.Items.First();
        actualError.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }
}
