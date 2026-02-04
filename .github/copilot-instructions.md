## Repository Overview
- Aspire All The Things is a demo suite that orchestrates multiple workloads via the Aspire AppHost.  
- The AppHost wires up official integrations (Redis, Postgres, SQL Server, Azure services), multi-language endpoints (ASP.NET Web API, Python Flask, Node.js Express), and custom containers (MailPit, Minecraft, Discord notifier).  
- The AppHost project references the WebApi and MailDemo projects, and uses the Aspiret AppHost SDK 13.1 (note the `Aspire.AppHost.Sdk/13.1.0` Sdk in AspireAllTheThings.AppHost.csproj) plus various Aspire hosting packages for each integration.

## Build, Test, and Lint
- `dotnet build AspireAllTheThings.sln` – compiles all projects.  
- `dotnet build AspireAllTheThings.AppHost` – builds only the AppHost frequently edited during demos.  
- No dedicated unit tests or linters are defined; rely on `dotnet build` for validation.  
- To execute the Python API, run `pip install -r requirements.txt` inside `python-api` followed by `python app.py`.  
- To run the Node API, `npm install` inside `node-api` then `npm start`.

## Running the Demos
- `cd AspireAllTheThings.AppHost && dotnet run` – launches the distributed AppHost demo environment. Uncomment calls like `builder.AddRedisDemo()` or `builder.AddMailPitDemo()` in `AppHost.cs` to enable different sections; refer to README sections for setup notes (e.g., Discord webhook via user secrets).  
- The AppHost logs connection info for each resource (HTTP ports, Docker endpoints), so watch console output to confirm which demos respond.

## High-Level Architecture
- `AspireAllTheThings.AppHost` is the orchestrator. `AppHost.cs` configures builder extensions defined across `1-OfficialIntegrations.cs` through `6-Fun.cs`, and the `builder.AddXxxDemo()` pattern adds Aspire-provided modules or custom containers for Phoenix integration.  
- `AspireAllTheThings.WebApi` is a minimal ASP.NET Core project with OpenAPI and a sample `/weatherforecast` controller; it is referenced by the AppHost and exposed by `builder.AddAspNetApiDemo()`.  
- `python-api` and `node-api` are external services registered via Aspire runtime helpers (`AddPythonApp`, `AddNpmApp`) and expose HTTP endpoints that the AppHost routes into the Aspire dashboard.  
- Shared defaults like resilience, service discovery, and OpenTelemetry live in `AspireAllTheThings.ServiceDefaults`; include it where new projects need the same middleware/observability stack.

## Key Conventions
- `AppHost.cs` intentionally keeps demos commented out; enable the desired scenario by uncommenting the relevant `builder.AddXxxDemo()` line before running.  
- Use Aspire extension packages (e.g., `Aspire.Hosting.Redis`) paired with `builder.AddRedisDemo()` wrappers to keep integration code aligned with naming conventions (watch 1-OfficialIntegrations.cs, 2-MultiLanguage.cs, etc.).  
- Multi-language services expect you to install dependencies per language folder and keep each demo self-contained; the AppHost assumes they listen on the ports documented in the README.  
- Secretful resources (Discord webhook, MailPit SMTP) are configured via `dotnet user-secrets` targeting `AspireAllTheThings.AppHost`.

