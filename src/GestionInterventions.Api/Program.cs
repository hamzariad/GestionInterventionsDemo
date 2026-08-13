using System.Collections.Concurrent;
using GestionInterventions.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<InterventionStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "GestionInterventions.Api"
}));

app.MapGet("/api/interventions", (InterventionStore store) =>
    Results.Ok(store.List()));

app.MapGet("/api/interventions/{id:guid}", (Guid id, InterventionStore store) =>
    store.Get(id) is { } intervention
        ? Results.Ok(intervention)
        : Results.NotFound(new { message = "Intervention introuvable." }));

app.MapPost("/api/interventions", (
    CreateInterventionRequest request,
    InterventionStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Titre) ||
        string.IsNullOrWhiteSpace(request.Technicien))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["intervention"] = ["Le titre et le technicien sont obligatoires."]
        });
    }

    if (!Enum.TryParse<InterventionPriority>(
            request.Priorite,
            ignoreCase: true,
            out var priorite))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["priorite"] = ["La priorité doit être Basse, Normale, Haute ou Critique."]
        });
    }

    var intervention = store.Create(
        request.Titre,
        request.Technicien,
        request.DatePlanifiee,
        priorite);
    return Results.Created($"/api/interventions/{intervention.Id}", intervention);
});

app.MapPost("/api/interventions/{id:guid}/cloture", (
    Guid id,
    InterventionStore store) =>
{
    var intervention = store.Get(id);
    if (intervention is null)
        return Results.NotFound(new { message = "Intervention introuvable." });

    intervention.Cloturer();
    return Results.Ok(intervention);
});

app.Run();

public sealed record CreateInterventionRequest(
    string Titre,
    string Technicien,
    DateOnly DatePlanifiee,
    string Priorite);

public sealed class InterventionStore
{
    private readonly ConcurrentDictionary<Guid, Intervention> _items = new();

    public IReadOnlyCollection<Intervention> List() =>
        _items.Values.OrderBy(x => x.DatePlanifiee).ToList();

    public Intervention? Get(Guid id) =>
        _items.GetValueOrDefault(id);

    public Intervention Create(
        string titre,
        string technicien,
        DateOnly datePlanifiee,
        InterventionPriority priorite)
    {
        var intervention = new Intervention(
            Guid.NewGuid(),
            titre,
            technicien,
            datePlanifiee,
            priorite);
        _items[intervention.Id] = intervention;
        return intervention;
    }
}

public partial class Program;
