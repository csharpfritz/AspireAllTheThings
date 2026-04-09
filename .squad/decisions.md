# Squad Decisions

## Active Decisions

### Decision: Migrate Aspire.Hosting.NodeJs → Aspire.Hosting.JavaScript

**Date:** 2025-07-25  
**Author:** Kaylee (Backend Dev)  
**Status:** Applied

#### Context
The Aspire.Hosting.NodeJs NuGet package was deprecated and only published up to version 9.5.2. Since all other Aspire packages are at 13.1.0, this created a version mismatch.

#### Resolution
Replaced Aspire.Hosting.NodeJs with Aspire.Hosting.JavaScript (version 13.1.0) and migrated the AddNpmApp() call in 2-MultiLanguage.cs to AddJavaScriptApp() with the unScriptName: parameter.

#### Impact
- 2-MultiLanguage.cs: AddNpmApp("node-api", "../node-api", "start") → AddJavaScriptApp("node-api", "../node-api", runScriptName: "start")
- .csproj: Package reference updated
- README: Code sample updated to match
- Build verified: passes cleanly

---

### Decision: Multi-Language API Version Strategy

**Author:** Wash (Full-Stack Dev)  
**Date:** 2025-07-16  
**Status:** Applied

#### Context
Reviewed all three multi-language APIs for training session readiness. Found dependency versions that needed updating to maintain stability and security.

#### Resolutions

**1. Stay on Express 4.x (not 5.x)**  
Express 5 is now the npm default, but migrating introduces breaking changes. Since these demos are about Aspire orchestration — not Express itself — stability wins. Updated minimum from 4.18.2 → 4.21.0 within the 4.x line.

**2. Stay on Spring Boot 3.4.x (not 4.0)**  
Spring Boot 4.0 was released Nov 2025 but is too new for stable demos. Updated from 3.4.2 → 3.4.13 (latest patch) for security fixes while staying on the proven 3.4 LTS line.

**3. Keep Redis npm on 4.x**  
Redis npm v5 changed APIs. Updated minimum from 4.6.12 → 4.7.0 within the same major. The createClient({ url }) pattern in the demo works on both.

#### Impact
- Node package.json: express ^4.21.0, redis ^4.7.0
- Java pom.xml: Spring Boot 3.4.13
- Python equirements.txt: No changes needed (flask>=3.0.0, redis>=5.0.0 are current)

#### Open Items
- Conference reference "CodeStock 2026" appears in all three APIs; update across all when targeting a different event.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction.
