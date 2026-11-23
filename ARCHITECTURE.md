# Job & Timecard Tracker — Architecture

## Project Structure
- **`JobTimecardTracker.Domain`:** Core domain entities and enum (`Job`, `Timecard`, `JobType`) representing jobs, client details, and employee time entries.
- **`JobTimecardTracker.Data`:** FluentNHibernate mappings (`ClassMap<T>`), `NHibernateHelper`, and SQL Server / LocalDB persistence wiring (`Microsoft.Data.SqlClient`).
- **`JobTimecardTracker.Api`:** ASP.NET Core Web API exposing REST endpoints (`/api/Jobs`, `/api/Timecards`), request/response DTOs, dependency injection setup, and Swagger/OpenAPI documentation.
- **`JobTimecardTracker.WinForms`:** Windows Forms desktop client providing workflows to manage jobs, log timecards, and view daily/total hours across multiple jobs per day.
- **`JobTimecardTracker.Tests`:** xUnit test project validating core domain rules such as summing hours worked for an employee on a given date across multiple jobs.

## Layering Strategy
- **Presentation (`JobTimecardTracker.WinForms`):** Handles user input, grids/tabs, totals display, and user-friendly error messages; communicates with the backend over HTTP rather than querying SQL Server directly.
- **API / Application Boundary (`JobTimecardTracker.Api`):** Validates incoming requests, maps between DTOs and domain entities, opens/manages NHibernate sessions, and returns appropriate HTTP status codes and payloads.
- **Domain (`JobTimecardTracker.Domain`):** Encapsulates core business concepts and rules independently of UI controls, HTTP transport, or database drivers.
- **Data Access (`JobTimecardTracker.Data`):** Encapsulates FluentNHibernate mappings and `ISessionFactory` initialization against SQL Server (`MSSQLLocalDB` / SQL Server).

## Design Decisions
- **REST API Between WinForms and SQL Server:** Decouples the desktop client from database access and credentials so business rules and persistence stay centralized and reusable across clients.
- **FluentNHibernate + `MicrosoftDataSqlClientDriver` on .NET 8:** Uses `Microsoft.Data.SqlClient` with `MicrosoftDataSqlClientDriver` for parameterized SQL Server access in .NET 8.
- **Eager `ISessionFactory` Initialization at Startup:** Builds `ISessionFactory` during API startup in `Program.cs` so mapping or database connectivity issues surface immediately instead of timing out the first live client request.
- **Exact `decimal` Hours Aggregation:** Uses `decimal` for hours worked so summing multiple jobs for an employee on the same day avoids floating-point rounding drift.

## Trade-Offs Made
- **HTTP API Layer vs. Direct WinForms-to-SQL:** Adds DTO mapping and HTTP client plumbing compared to direct DB access, in exchange for cleaner separation of concerns and multi-client readiness.
- **Eager Startup `ISessionFactory` vs. Lazy First-Request Init:** Fails fast at startup if SQL Server is unreachable and adds a brief startup cost, preventing unexpected first-request timeouts in WinForms.
- **Prioritizing Core Assessment + Documentation Over Building Both Bonus Clients:** Ensures the required SQL Server, FluentNHibernate, WebAPI, WinForms, unit tests, and documentation are rock-solid rather than spreading effort thinly across unfinished clients.

## Bonus Clients Approach (Web & Mobile)
Because `JobTimecardTracker.Api` already exposes REST endpoints and DTOs over HTTP, secondary clients can be added on top of the existing backend without changing the database layer:
- **Web Client (e.g., Blazor WebAssembly / React / ASP.NET Core):** Consumes `/api/Jobs` and `/api/Timecards` over HTTPS using typed DTOs, with CORS configured in `JobTimecardTracker.Api`, client/server validation, and responsive forms/tables for jobs, timecards, and daily hour totals.
- **Mobile Client (e.g., .NET MAUI / Flutter / React Native):** Tailored for quick field timecard entry (selecting an active job, defaulting `DateWorked` to today, entering `Employee` and `HoursWorked`, and checking daily totals), configured with a reachable API base URL/tunnel, timeout/error handling, and an optional local offline queue/sync mechanism for spotty connectivity.

## Future Improvements
- **Broader Automated Tests:** Expand unit tests for validation edge cases and add integration tests for FluentNHibernate mappings and API endpoints.
- **Enhanced Filtering, Editing & Reporting:** Add search/filtering by employee, date, or job, plus summary views/exports.
- **Operational Hardening:** Standardize API error responses, add structured logging/health checks, and externalize configuration cleanly across environments.