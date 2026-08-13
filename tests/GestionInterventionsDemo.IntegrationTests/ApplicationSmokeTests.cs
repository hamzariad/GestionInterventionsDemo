using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GestionInterventionsDemo_IntegrationTests;

public sealed class ApplicationSmokeTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApplicationSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ApplicationStartsAndAnswersWithoutServerError()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.True(
            (int)response.StatusCode < 500,
            $"L'application a retourné {(int)response.StatusCode}.");
    }
}
