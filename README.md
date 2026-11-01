# TennisStatistiques.Api

API REST en **ASP.NET Core (.NET 10)** qui expose les informations et les statistiques de joueurs de tennis à partir du fichier `headtohead.json`.

## Fonctionnalités

- Liste des joueurs triée du meilleur au moins bon classement
- Détail d'un joueur par son identifiant
- Statistiques globales :
  - pays ayant le meilleur ratio de parties gagnées
  - IMC moyen de l'ensemble des joueurs
  - médiane de la taille des joueurs
- Inscription d'un nouveau joueur (avec validation des données)

## Prérequis

- [SDK .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2026, Rider ou VS Code (facultatif)

## Lancer l'API

Depuis le dossier de la solution :

```bash
dotnet run --project TennisStatistiques.Api --launch-profile https
```

L'API écoute alors sur `https://localhost:7299` (et `http://localhost:5202`).

Avec Visual Studio : ouvrir `TennisStatistiques.Api.slnx`, choisir le profil **https** puis lancer avec F5.

### Documentation interactive

En environnement de développement, la documentation est disponible :

- **Swagger UI** : https://localhost:7299/swagger
- **Document OpenAPI** : https://localhost:7299/openapi/v1.json

Le fichier `TennisStatistiques.Api/TennisStatistiques.Api.http` contient aussi des requêtes prêtes à l'emploi (Visual Studio, Rider ou l'extension REST Client de VS Code).

## Endpoints

| Méthode | Route | Description | Réponses |
|---|---|---|---|
| `GET` | `/api/TennisStatistiques` | Liste des joueurs triée par classement | 200 |
| `GET` | `/api/TennisStatistiques/{id}` | Détail d'un joueur | 200, 404 |
| `GET` | `/api/TennisStatistiques/statistics` | Statistiques globales | 200, 404 |
| `POST` | `/api/TennisStatistiques` | Inscription d'un joueur | 201, 400 |

### Exemple : statistiques

```http
GET /api/TennisStatistiques/statistics
```

```json
{
  "bestCountry": { "code": "SRB", "winRatio": 1 },
  "averageBmi": 23.36,
  "medianHeight": 185
}
```

### Exemple : inscription d'un joueur

```http
POST /api/TennisStatistiques
Content-Type: application/json

{
  "firstname": "Arthur",
  "lastname": "Fils",
  "shortname": "A.FIL",
  "sex": "M",
  "countryCode": "FRA",
  "rank": 20,
  "points": 1500,
  "weight": 79000,
  "height": 185,
  "age": 22
}
```

Réponse : `201 Created`, avec le joueur créé dans le corps et son URL dans l'en-tête `Location`.

Règles de validation :

| Champ | Règle |
|---|---|
| `firstname`, `lastname` | obligatoires, 50 caractères maximum |
| `shortname` | obligatoire, 10 caractères maximum |
| `sex` | `M` ou `F` |
| `countryCode` | code pays sur 3 lettres (mis en majuscules) |
| `rank` | ≥ 1 |
| `points` | ≥ 0 |
| `weight` | en **grammes**, entre 30 000 et 200 000 |
| `height` | en **centimètres**, entre 100 et 250 |
| `age` | entre 10 et 70 |

Une requête invalide renvoie `400 Bad Request` avec le détail des erreurs par champ.

## Architecture

Projet unique organisé en couches, avec injection de dépendances :

```
TennisStatistiques.Api/
├── Controllers/    Couche HTTP : routes et codes de réponse, aucune logique métier
├── Services/       Logique métier : tri, recherche, calcul des statistiques, création
├── Repositories/   Accès aux données, derrière l'interface IPlayerRepository
├── Entities/       Modèle de données (Player, Country, PlayerData)
├── Dtos/           Objets d'entrée et de sortie de l'API
├── Exceptions/     Exception métier et gestionnaire global des erreurs
└── Data/           headtohead.json
```

Le flux d'une requête : **Controller → Service → Repository**. Chaque couche ne dépend que de l'interface de la suivante, ce qui permet de la tester de façon isolée.

## Choix techniques

### Calcul des statistiques

- **Ratio de victoires par pays** : les victoires et les matchs de tous les joueurs d'un même pays sont **cumulés** avant de calculer le ratio (total des victoires / total des matchs). Un joueur ayant disputé plus de matchs pèse donc davantage, au lieu de faire la moyenne des ratios individuels.
- **IMC** : `poids (kg) / taille (m)²`. Le fichier source exprime le poids en grammes et la taille en centimètres ; les valeurs sont converties avant le calcul.
- **Médiane des tailles** : les tailles sont triées ; si le nombre de joueurs est pair, la médiane est la moyenne des deux valeurs centrales.

### Données

Le fichier `headtohead.json` est chargé **une seule fois** au démarrage, dans un repository enregistré en Singleton. Les lectures et écritures sont protégées par un verrou, car l'instance est partagée entre les requêtes concurrentes.

Limite connue : les données sont **en mémoire**. Un joueur ajouté par `POST` est perdu au redémarrage. Grâce à l'interface `IPlayerRepository`, on peut passer à une base de données (EF Core avec SQLite ou SQL Server, par exemple) en ajoutant une nouvelle implémentation, sans modifier les services ni le contrôleur.

### Gestion des erreurs

Les erreurs sont gérées de façon **centralisée** par un `IExceptionHandler` (`GlobalExceptionHandler`), sans try/catch dans les contrôleurs :

- les exceptions métier sont traduites en codes HTTP (`NoPlayersException` donne `404 Not Found`) ;
- toute autre exception renvoie `500` avec un message générique, sans exposer de détails techniques, et elle est journalisée ;
- toutes les réponses d'erreur suivent le format standard **ProblemDetails** (RFC 9457).

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Aucun joueur disponible pour calculer les statistiques."
}
```

## Tests

Le projet `TennisStatistiques.Api.Tests` utilise **xUnit**, **NSubstitute** (simulacres) et **Microsoft.AspNetCore.Mvc.Testing** (tests d'intégration).

```bash
dotnet test
```

| Niveau | Fichier | Ce qui est testé |
|---|---|---|
| Unitaire | `Services/PlayerServiceTests.cs` | Tri par classement, recherche par id, création d'un joueur |
| Unitaire | `Services/StatisticsServiceTests.cs` | Meilleur pays (dont l'agrégation par pays), IMC avec conversion des unités, médiane pour un nombre pair, impair ou d'un seul joueur, liste vide |
| Unitaire | `Controllers/TennisStatistiquesControllerTests.cs` | Réponse 200 et propagation des exceptions |
| Intégration | `Integration/PlayersEndpointsTests.cs` | Codes HTTP réels de bout en bout : 200, 201, 400, 404 et format ProblemDetails |

Les tests d'intégration démarrent l'API en mémoire avec `WebApplicationFactory`. Le cas « aucun joueur » remplace le repository par un simulacre vide via `ConfigureTestServices`.

## Pistes d'amélioration

- Persistance des données en base (EF Core)
- Endpoints de modification et de suppression (`PUT`, `DELETE`)
- Pagination et filtres sur la liste des joueurs (par pays, par sexe)
- Gestion de l'unicité d'un joueur à l'inscription (`409 Conflict`)
- Conteneurisation (Dockerfile) et pipeline d'intégration continue
