# AI Agent Quickstart for MyScoreBoard

This repo hosts a .NET 9 Blazor WebAssembly app for a board‑game scorekeeper. Agents should prioritize end‑to‑end edits, verify with a local build, and keep UI consistent with the existing glass/white theme.

## Architecture and data flow
- UI: Razor pages/components in `MyScoreBoard/Pages`, layout in `MyScoreBoard/Layout`, small UI components in `MyScoreBoard/Components`.
- State: `Services/GameService.cs` is the single source of truth for the current game (players, rounds, totals). It also orchestrates persistence.
- Persistence:
  - IndexedDB via JS interop (`wwwroot/js/indexedDb.js`) wrapped by `Services/IndexedDbService.cs`.
  - Active game cached in the `active` store (fixed key `current`), completed games in `games` (auto‑increment keys). DB versioning is in `indexedDb.js`.
  - `LocalStorageService.cs` mirrors a quick boolean `hasActiveGame` for fast UI (Home) and preferences (saved setup values).
- Celebrations/UI polish: `Components/Confetti.razor` and `wwwroot/js/confetti.js`.

```instructions
# AI Agent Quickstart for MyScoreBoard (Blazor + MAUI)

This monorepo contains a Blazor WebAssembly web app (`MyScoreBoard`), a MAUI host (`MyScoreBoardMaui`) and a shared library (`MyScoreBoardShared`) with models and service interfaces. Agents should prefer end-to-end edits, use DI interfaces from `MyScoreBoardShared`, and verify via local builds (web and MAUI Android for CI-free verification).

Architecture & big picture
- Projects: `MyScoreBoard` (Blazor), `MyScoreBoardMaui` (MAUI), `MyScoreBoardShared` (models + interfaces).
- Shared types: `MyScoreBoardShared/Models` holds `Player`, `Round`, `GameSession`, `GameStoreEntry`.
- Service interfaces: `MyScoreBoardShared/Services` includes `IGameService`, `IIndexedDbService`, `ILocalStorageService` — prefer these for edits and DI wiring.
- Data flow: `GameService` (web) is the app state orchestrator; it calls `IIndexedDbService` + `ILocalStorageService` for persistence.

Persistence backends (important)
- Web: `wwwroot/js/indexedDb.js` + `MyScoreBoard/Services/IndexedDbService.cs` (JS interop). Stores: `active` (explicit key `current`) and `games` (auto-increment).
- MAUI: sqlite-backed `IIndexedDbService` implementation (uses `sqlite-net-pcl` and `SQLiteAsyncConnection`) at `MyScoreBoardMaui/Services/IndexedDbService.cs` and `ILocalStorageService` implemented using `Preferences` at `MyScoreBoardMaui/Services/LocalStorageService.cs`.

Dependency injection & lifetimes
- Web DI registrations live in `MyScoreBoard/Program.cs` (scoped): bind `IGameService`, `IIndexedDbService`, `ILocalStorageService` to Blazor implementations.
- MAUI DI registrations live in `MyScoreBoardMaui/MauiProgram.cs` (singleton for platform services): bind shared interfaces to MAUI implementations.
- When editing services, update both registration locations and prefer constructor injection of the shared interfaces.

Build / dev workflow (practical)
- Web: cd `MyScoreBoard` → `dotnet build` / `dotnet run` (serves the Blazor app).
- MAUI: prefer Android target for local CI-free checks: `dotnet build MyScoreBoardMaui/MyScoreBoardMaui.csproj -f net9.0-android`.
- Full solution: `dotnet build MyScoreBoard.sln` (note: iOS/MacCatalyst builds may fail locally if Xcode version mismatch; see note below).

Key files to inspect for edits
- UI: `MyScoreBoard/Pages/*` (`Home.razor`, `GameSetup.razor`, `GamePlay.razor`, `GameHistory.razor`).
- State/persistence: `MyScoreBoard/Services/GameService.cs`, `MyScoreBoard/Services/IndexedDbService.cs`, `MyScoreBoard/Services/LocalStorageService.cs`.
- Shared contracts: `MyScoreBoardShared/Models/GameModels.cs`, `MyScoreBoardShared/Services/*`.
- MAUI implementations: `MyScoreBoardMaui/Services/*` and `MyScoreBoardMaui/MauiProgram.cs`.

Examples & small contracts
- Adding a field to `GameSession`:
  - Update `MyScoreBoardShared/Models/GameModels.cs`.
  - Update `GameService.EndGameAsync()` to persist the new field (both web and MAUI storage representations).
  - If web persistence shape changed, bump DB schema version in `wwwroot/js/indexedDb.js` for dev iterations.

Gotchas & project-specific conventions
- IndexedDB `games` store is auto-increment; do not pass a key when adding. Use explicit key `'current'` for the `active` store.
- `OnInitializedAsync` may run before browser APIs are ready — use `OnAfterRenderAsync(firstRender)` for IndexedDB/localStorage access.
- Shared project exposes models and interfaces; concrete implementations live in each host (web vs MAUI). Prefer editing the shared interfaces when adding capabilities, then implement per-host.

Troubleshooting / environment notes
- MAUI iOS/MacCatalyst builds often require matching Xcode (example: Xcode 16.4 required by some .NET workloads). If a solution build fails with Xcode errors, either update Xcode or build only Android and Blazor locally.
- Android build warnings may appear from native sqlite binaries (page-size warnings); these are non-blocking but worth tracking when upgrading native libs.

If anything here is unclear or you want more patterns (testing, deployments, CI config), say which area to expand and include any preferred conventions.

```

