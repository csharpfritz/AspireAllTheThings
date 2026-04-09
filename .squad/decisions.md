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

### Decision: Reorder Demo Parts 6 & 7 — AI Integration / Fun Capstone

**Date:** 2026-04-09  
**Author:** Kaylee (Backend Dev)  
**Status:** Applied

#### Context
User directive (2026-04-09T13:46) requested keeping Minecraft as the final, fun capstone demo. To support narrative flow, reordered Parts 6 & 7 so AI Integration comes before the Fun demos.

#### Resolution
- Renamed `7-AI.cs` → `6-AI.cs`
- Renamed `6-Fun.cs` → `7-Fun.cs`
- Updated all inline comments and extension method headers
- Reordered AppHost.cs header and demo builder sections
- Updated README.md part table and narrative

#### Impact
- Demo order now: Part 1–5 (Integrations, Multi-Language, IT-Tools, MailPit, Discord) → **Part 6 (AI Integration)** → **Part 7 (Fun/Minecraft, final capstone)**
- All file names, comments, and cross-references updated
- Build verified: clean

---

### Decision: Part 6 AI Demo — Chat UI Implementation

**Date:** 2026-04-09  
**Author:** Kaylee (Backend Dev)  
**Status:** Implemented  
**Requested by:** Jeffrey T. Fritz

#### Context
The Part 6 AI Integration demo had a bare GET `/chat?message=...` endpoint returning JSON. For live demos at CodeStock 2026, a visually appealing chat UI was needed for audience engagement rather than just JSON responses.

#### Resolution

**1. Changed `/chat` Endpoint from GET to POST**
- Accepts JSON body `{ "message": "..." }` instead of query string
- Response format remains: `{ "prompt": "...", "response": "..." }`

**2. Created Self-Contained Chat UI**
- File: `AspireAllTheThings.WebApi/wwwroot/chat.html`
- Single HTML file with inline CSS/JavaScript (no external dependencies)
- Dark theme matching Aspire dashboard (purple/dark gradient)
- Features: Chat bubbles, loading indicator, error handling, responsive layout
- Uses vanilla JavaScript and `fetch()` to call POST `/chat` endpoint

**3. Enabled Static File Serving**
- Added `app.UseStaticFiles();` to Program.cs
- Chat UI accessible at `/chat.html`

**4. Updated Demo Script**
- `.squad/files/part6-ai-demo-script.md` updated
- Step 2: Reflects POST with ChatRequest
- Step 6: Replaced endpoint test with `/chat.html` visual demo

#### Impact
- **Files changed:** Program.cs (UseStaticFiles, POST endpoint, ChatRequest record), new wwwroot/chat.html
- **Demo script updated:** Part 6 steps 2 & 6 revised
- **Build verified:** Clean
- **UX improvement:** Polished, stage-friendly chat interface for live demo
- **Backward compatibility:** /weatherforecast unchanged; IChatClient pattern intact

#### Technical Notes
- Static file middleware must come after `app.Build()` and before endpoint mapping
- Record types in top-level programs must come after `app.Run()` to avoid CS8803
- Self-contained HTML avoids build step and bundling complexity
- Chat UI gracefully handles null IChatClient (displays friendly error)

---

### User Directives

#### 2026-04-09T13:16:04Z — Update Conference References
**By:** Jeffrey T. Fritz  
**What:** Update all conference references from "Swetugg Stockholm 2026" to "CodeStock 2026"  
**Status:** Captured for team action

#### 2026-04-09T13:31:19Z — Focus on GitHub Models, Drop Low-Priority Demos
**By:** Jeffrey T. Fritz  
**What:** Focus new demo additions on GitHub Models (easy, impactful). Drop DevTunnels demo (audience can't browse from phones in ballroom without Minecraft). ExternalService and YARP need clearer use cases before committing.  
**Status:** Captured for team action

#### 2026-04-09T13:46:00Z — Keep Minecraft as Final Demo
**By:** Jeffrey T. Fritz  
**What:** Keep Minecraft (Fun Demos) as the last demo — it's the fun capstone that puts a cap on everything.  
**Status:** ✅ Applied (Parts 6/7 reordered per this directive)

---

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction.
