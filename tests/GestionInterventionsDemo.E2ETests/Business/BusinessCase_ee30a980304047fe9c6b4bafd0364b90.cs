using System.Net;
using System.Text;
using Xunit;

namespace GestionInterventionsDemo.E2ETests.Business;

/// <summary>
/// QA Platform : Creer une intervention critique valide
/// Identifiant stable : QA_CASE_EE30A980304047FE9C6B4BAFD0364B90
/// </summary>
public sealed class BusinessCase_ee30a980304047fe9c6b4bafd0364b90
{
    [Fact(DisplayName = "QA_CASE_EE30A980304047FE9C6B4BAFD0364B90")]
    [Trait("QAPlatformCaseId", "ee30a980-3040-47fe-9c6b-4bafd0364b90")]
    [Trait("Category", "QAPlatformBusiness")]
    public async Task Execute()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL");
        Assert.False(
            string.IsNullOrWhiteSpace(baseUrl),
            "E2E_BASE_URL doit être configurée par Jenkins.");

        using var client = new HttpClient { BaseAddress = new Uri(baseUrl!) };
        using var request = new HttpRequestMessage(
            new HttpMethod("POST"),
            "/api/interventions");
        var requestBody = "{\"titre\":\"Maintenance urgente\",\"technicien\":\"Nadia\",\"datePlanifiee\":\"2026-08-20\",\"priorite\":\"Critique\"}";
        if (!string.IsNullOrWhiteSpace(requestBody))
        {
            request.Content = new StringContent(
                requestBody,
                Encoding.UTF8,
                "application/json");
        }

        using var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(
            (HttpStatusCode)201,
            response.StatusCode);
        var expectedContent = "Maintenance urgente";
        if (!string.IsNullOrWhiteSpace(expectedContent))
        {
            Assert.Contains(expectedContent, responseBody, StringComparison.OrdinalIgnoreCase);
        }
    }
}
