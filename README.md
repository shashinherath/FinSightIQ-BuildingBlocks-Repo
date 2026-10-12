# FinSightIQ.BuildingBlocks

Shared, dependency-free .NET primitives for FinSightIQ services. Distributed as the private NuGet package `FinSightIQ.BuildingBlocks` on GitHub Packages.

- **Target framework:** `net8.0`
- **Package dependencies:** none
- **Current version:** `0.1.0` (pre-1.0; the API may change between minor versions)

The library contains no service-specific business logic, database, caching, messaging, authentication or ASP.NET Core code. It must stay that way.

## Components

| Type | Namespace | Purpose |
|---|---|---|
| `Result`, `Result<T>`, `Error` | `FinSightIQ.BuildingBlocks.Results` | Expected success/failure outcomes without exceptions. `Error.Code` is a stable `UPPER_SNAKE_CASE` code (e.g. `SYMBOL_NOT_FOUND`). |
| `PagedResult<T>` | `FinSightIQ.BuildingBlocks.Results` | One page of a cursor-paginated collection (`Items`, `NextCursor`, `HasMore`, `Limit`). Default limit 50, maximum 500. The cursor is opaque. |
| `BaseEntity` | `FinSightIQ.BuildingBlocks.Entities` | Optional `Guid` identity base class. Rejects `Guid.Empty`. No audit or soft-delete fields. |
| `DomainException` | `FinSightIQ.BuildingBlocks.Exceptions` | Domain-invariant violation with a stable `Code`; supports an inner exception. HTTP-agnostic. |
| `CorrelationId` | `FinSightIQ.BuildingBlocks.Middleware` | Validated `X-Correlation-Id` value (`HeaderName`, `NewId()`, `Parse`, `TryParse`). |

### Examples

```csharp
Result<Quote> result = quote is null
    ? Result<Quote>.Failure("SYMBOL_NOT_FOUND", "Ticker is not in the supported universe.")
    : Result<Quote>.Success(quote);

PagedResult<Price> page = hasMore
    ? PagedResult<Price>.WithMore(prices, limit: 100, nextCursor: cursor)
    : PagedResult<Price>.Last(prices, limit: 100);

if (!CorrelationId.TryParse(headerValue, out CorrelationId? id))
{
    id = CorrelationId.NewId();
}
```

### Design notes

- **No HTTP middleware.** `CorrelationId` lives in the `Middleware` namespace as the implementation plan specifies, but it is only the value type. Request middleware belongs in the gateway or a web-specific project.
- **No logging implementation.** The implementation plan mentions Serilog; adding it here would force a logging stack on every consumer, so it is deliberately excluded.
- **Untrusted correlation IDs** are limited to 64 characters from `A-Z a-z 0-9 - _ .` to prevent log/header injection.

## Installation

Packages come from GitHub Packages, which requires authentication even for public packages. Add to the consumer's `nuget.config` (the token is read from the environment and never stored):

```xml
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="finsightiq" value="https://nuget.pkg.github.com/<owner>/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <finsightiq>
      <add key="Username" value="<owner>" />
      <add key="ClearTextPassword" value="%NUGET_TOKEN%" />
    </finsightiq>
  </packageSourceCredentials>
</configuration>
```

`NUGET_TOKEN` is a personal access token with `read:packages` locally, or `GITHUB_TOKEN` with `packages: read` in Actions (the consuming repository must be granted access in the package's settings). Then:

```powershell
dotnet add package FinSightIQ.BuildingBlocks --version 0.1.0
```

## Build, test, pack

```powershell
dotnet restore FinSightIQ.BuildingBlocks.sln
dotnet build FinSightIQ.BuildingBlocks.sln --configuration Release
dotnet test FinSightIQ.BuildingBlocks.sln --configuration Release --collect:"XPlat Code Coverage"
dotnet pack src/FinSightIQ.BuildingBlocks/FinSightIQ.BuildingBlocks.csproj --configuration Release --output ./artifacts
```

Packages are created only by `dotnet pack`, never by an ordinary build.

## CI and publishing

- `.github/workflows/ci.yml` restores, builds (Release), tests and packs (validation only) on pushes and pull requests to `main`.
- `.github/workflows/publish.yml` runs **only** when a tag such as `v0.1.0` is pushed. It checks that the tag equals `<Version>` in the csproj, builds, tests, packs and pushes to `https://nuget.pkg.github.com/<owner>/index.json` using `GITHUB_TOKEN` (`packages: write`). `--skip-duplicate` prevents overwriting an existing version.

Repository requirement: **Settings → Actions → General → Workflow permissions** must allow the workflow to request `packages: write` (the workflow declares it explicitly).

Release steps:

```powershell
# 1. Bump <Version> in src/FinSightIQ.BuildingBlocks/FinSightIQ.BuildingBlocks.csproj and merge to main
# 2. Tag and push
git tag v0.1.0
git push origin v0.1.0
```

## Versioning policy

Semantic versioning. Stay on `0.x` until the first consuming services (Sprint 3) have validated the types; breaking changes then bump the minor version. From `1.0.0`, breaking changes require a major version. Published versions are immutable — never re-tag or overwrite; publish a new version instead.

## Contributing and compatibility

- Follow the FinSightIQ C# coding standards (STD-CSHARP-001); `.editorconfig` encodes the formatting rules.
- Only add types that are shared by multiple services. Do not add infrastructure dependencies (Dapper, Npgsql, EF Core, Redis, RabbitMQ, JWT, ASP.NET Core).
- Every public API needs XML documentation and unit tests; target at least 80% line coverage.
- Do not change `PackageId` or the target framework without a documented reason. Error codes are contract values — do not rename them in a minor release.
