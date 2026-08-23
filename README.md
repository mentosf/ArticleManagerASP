# ArticleManagerASP

A web application for managing articles, built with **ASP.NET Core MVC** (.NET 10). The project has a public-facing part for viewing/working with articles and a separate admin panel, authentication via OpenID Connect (Keycloak), and database access through Entity Framework Core (PostgreSQL).

## Tech Stack

- **ASP.NET Core MVC** — `net10.0`
- **Entity Framework Core** + **Npgsql** — PostgreSQL data access
- **OpenID Connect (Keycloak)** — user authentication and authorization
- **Areas** — separate admin area (`Areas/Admin`)
- **Custom Middleware** — custom exception handling (`ExceptionHandlingMiddleware`)
<img width="2948" height="1896" alt="Знімок екрана 2026-08-23 200010" src="https://github.com/user-attachments/assets/fc1ca671-eba6-4ce6-9af0-dc9d025609a1" />


## Project Structure

```
ArticleManagerASP/
├── Areas/Admin/        # Admin panel (separate MVC area)
├── Controllers/        # MVC controllers
├── DTOs/               # Data transfer objects
├── Data/               # DbContext and database configuration
├── Extensions/         # Extension methods
├── Middlewares/         # Custom middleware (exception handling, etc.)
├── Migrations/         # Entity Framework Core migrations
├── Models/             # Data models (entities)
├── Properties/         # Launch settings (launchSettings.json)
├── Services/           # Business logic (e.g. ArticleService)
├── Views/              # Razor views (UI)
├── wwwroot/            # Static files (CSS, JS, images)
├── Program.cs          # Entry point, application configuration
├── appsettings.json    # Main configuration
└── FinalTask.csproj    # .NET project file
```

## Authentication

Authentication is implemented via **OpenID Connect** with **Keycloak** as the provider:

- Realm: `ArticleManager`
- Client ID: `mvc-client`
- Default scheme: Cookie authentication
- Provider endpoint: `http://localhost:8080/realms/ArticleManager`

> ⚠️ Before running the app, make sure Keycloak is running locally (or update `Authority` to point to your own server), and move the `ClientSecret` to a secure location (user secrets / environment variables) rather than leaving it in plain code.

## Routing

- **Admin area**: `Admin/{controller=Dashboard}/{action=Index}/{id?}`
- **Default route**: `{controller=Article}/{action=Index}/{id?}`

## Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL (locally or in a container)
- Keycloak (for authentication)

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/mentosf/ArticleManagerASP.git
   cd ArticleManagerASP
   ```

2. Configure the database connection string in `appsettings.json` (or `appsettings.Development.json`):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=articlemanager;Username=postgres;Password=yourpassword"
     }
   }
   ```

3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

5. Make sure Keycloak is reachable at the address specified in `Program.cs` (`Authority`), with the `ArticleManager` realm and `mvc-client` client configured.

## License

Specify a license if you plan to make the repository public (e.g. MIT).
