# AI Usage Declaration

## Tools Used
- Gemini / AI Assistant (interactive coding, debugging, and architecture/documentation assistance)
- Visual Studio 2022 & .NET 8 CLI (`dotnet`)

## How They Were Used
- Helped structure the multi-project .NET 8 solution (`JobTimecardTracker.Domain`, `JobTimecardTracker.Data`, `JobTimecardTracker.Api`, `JobTimecardTracker.WinForms`, `JobTimecardTracker.Tests`).
- Assisted with diagnosing build, Swagger/OpenAPI, FluentNHibernate, and runtime/timeout issues.
- Created starter xUnit test scaffolding for summing hours worked for an employee on a given day across multiple jobs.
- Helped draft `ARCHITECTURE.md` and `AI_USAGE.md` to align with the project specification.

## What You Changed
- Updated `Swashbuckle.AspNetCore` and removed invalid `OpenApiVersion` usage on `SwaggerOptions` so Swagger UI rendered properly.
- Switched NHibernate's SQL Server driver to `MicrosoftDataSqlClientDriver` with `Microsoft.Data.SqlClient` for .NET 8 compatibility.
- Fixed `CS0718` in `NHibernateHelper` by passing a non-static mapping class (`JobMap`) to `.AddFromAssemblyOf<JobMap>()`.
- Moved `ISessionFactory` initialization to eager startup in `JobTimecardTracker.Api\Program.cs` and verified `MSSQLLocalDB` connectivity to prevent first-request timeouts.
- Updated decimal literals in the xUnit test with the `m` suffix (`4.5m`, `3.25m`, `7.75m`) for `decimal` hours and adjusted the test employee filter (`"CAT VIP"`) so the same-day multi-job total passes.

## Lessons Learned
- AI speeds up scaffolding and troubleshooting, but generated code and snippets still require careful review against your domain types, project conventions, and actual runtime behavior.
- Checking inner exceptions, verifying local services (like `MSSQLLocalDB`), and keeping `ISessionFactory` / process lifecycles clear saves significant debugging time.
- There is no penalty for using AI to assist with development, provided you understand, verify, and own the final code and design decisions.