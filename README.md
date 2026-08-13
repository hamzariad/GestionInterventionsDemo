# GestionInterventionsDemo

API de démonstration utilisée pour valider le comportement de QA Platform face à un nouveau projet proche d'un environnement de production.

## Métier couvert

- planifier une intervention avec un technicien et une priorité ;
- consulter les interventions ;
- retrouver une intervention ;
- clôturer une intervention.

## Démarrage local

```powershell
dotnet run --project src/GestionInterventions.Api --urls http://localhost:5095
```

Le dépôt initial ne contient volontairement ni tests ni `Jenkinsfile`. QA Platform doit détecter le projet, analyser sa technologie et proposer le socle qualité dans une pull request contrôlée.
