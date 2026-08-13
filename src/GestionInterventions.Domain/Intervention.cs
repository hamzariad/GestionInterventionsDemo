namespace GestionInterventions.Domain;

public sealed class Intervention
{
    public Intervention(
        Guid id,
        string titre,
        string technicien,
        DateOnly datePlanifiee,
        InterventionPriority priorite)
    {
        if (string.IsNullOrWhiteSpace(titre))
            throw new ArgumentException("Le titre est obligatoire.", nameof(titre));
        if (string.IsNullOrWhiteSpace(technicien))
            throw new ArgumentException("Le technicien est obligatoire.", nameof(technicien));

        Id = id;
        Titre = titre.Trim();
        Technicien = technicien.Trim();
        DatePlanifiee = datePlanifiee;
        Priorite = priorite;
    }

    public Guid Id { get; }
    public string Titre { get; }
    public string Technicien { get; }
    public DateOnly DatePlanifiee { get; }
    public InterventionPriority Priorite { get; }
    public InterventionStatus Statut { get; private set; } = InterventionStatus.Planifiee;
    public DateTimeOffset CreeeLe { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClotureeLe { get; private set; }

    public void Cloturer()
    {
        if (Statut == InterventionStatus.Cloturee)
            return;

        Statut = InterventionStatus.Cloturee;
        ClotureeLe = DateTimeOffset.UtcNow;
    }
}

public enum InterventionPriority
{
    Basse,
    Normale,
    Haute,
    Critique
}

public enum InterventionStatus
{
    Planifiee,
    Cloturee
}
