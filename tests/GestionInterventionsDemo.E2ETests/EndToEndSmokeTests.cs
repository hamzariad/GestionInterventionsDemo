using Xunit;

namespace GestionInterventionsDemo_E2ETests;

public sealed class EndToEndSmokeTests
{
    [Fact]
    public async Task DeployedApplicationAnswersWithoutServerError()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL");
        Assert.False(
            string.IsNullOrWhiteSpace(baseUrl),
            "E2E_BASE_URL doit être configurée par le pipeline.");

        using var client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl!)
        };
        using var response = await client.GetAsync("/");

        Assert.True(
            (int)response.StatusCode < 500,
            $"L'application E2E a retourné {(int)response.StatusCode}.");
    }
}
