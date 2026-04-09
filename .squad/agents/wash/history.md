# Wash — History

## Project Context
- **Project:** AspireAllTheThings — Aspire training demo suite
- **Tech Stack:** Python Flask, Node.js Express, Java Spring Boot, all orchestrated by .NET Aspire
- **User:** Jeffrey T. Fritz
- **Python API:** Flask app at `python-api/app.py`, port 5000
- **Node API:** Express app at `node-api/`, port 3000
- **Java API:** Spring Boot at `java-api/`, port 8080, requires Java 21+ and Maven
- **Aspire integration:** Uses AddPythonApp, AddNpmApp, AddSpringApp helper methods

## Learnings
- All three APIs (Python, Node, Java) have `/health` endpoints and consistent response structure (`message`, `managed_by`, `conference` fields)
- Python and Node APIs both reference `ConnectionStrings__shared-cache` for Redis, matching Aspire's naming convention
- Java API uses Spring Boot Actuator for `/actuator/health` plus a custom `/health` endpoint — both work fine
- The `agents/` directory under AppHost is for the optional OTel Java agent; it exists but is empty (agent downloaded at user's discretion)
- Conference reference across all APIs is "Swetugg Stockholm 2026" — update this when targeting a different event
- Spring Boot 3.4.x is the right line for Java 21+ demos (stable, LTS); don't jump to 4.0 for training demos
- Express 4.x is intentionally kept over Express 5.x to avoid breaking changes in a demo that's about Aspire, not Express
- Python Redis connection parsing strips `tcp://` prefix then splits `host:port` — works with Aspire's injection format
- Node.js Redis client uses URL-based connection (`redis.createClient({ url })`) — compatible with Aspire's connection strings
- 2-MultiLanguage.cs correctly passes PORT env var to Node (`env: "PORT"`) but not to Python (Python defaults to 5000, which matches `targetPort: 5000`)
