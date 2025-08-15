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

## Key patterns and conventions
- Game lifecycle
  - Create via `GameSetup.razor` → `GameService.NewGame()` + `GameService.SaveActiveAsync()`.
  - Play in `GamePlay.razor` using `Game.AddRound`, `Game.NextRound`, totals from `Game.GetTotals()`.
  - End with `Game.EndGameAsync()` which writes a `GameStoreEntry` to `games` and clears `active`.
- Resume flow
  - Home (`Home.razor`) checks `LocalStorageService.GetHasActiveGameAsync()` first, then verifies with `Game.HasActiveGameAsync()` (IndexedDB) and shows “Resume Game”.
- IndexedDB specifics
  - DB name `myscoreboard`, version defined at top of `indexedDb.js`. On schema changes, bump the version.
  - Stores:
    - `games`: `{ autoIncrement: true }`. Use `addItem/putItem` without passing a key.
    - `active`: no keyPath; always use explicit key `'current'` for `putItem/getFirst/clearStore`.
  - `getAll()` attaches a `Key` property to returned items for delete operations in History.
- Styling
  - Bootstrap 5 + custom glass look. Navbar uses `navbar-dark`; hamburger icon overridden to white in `Layout/MainLayout.razor.css`.
  - Text contrast is white across glass cards; prefer `text-contrast` and `text-contrast-muted` helpers.

## Build/run/debug
- Preferred run location: `MyScoreBoard/`
- Commands:
  - Build: `dotnet build`
  - Run: `dotnet run` (from `MyScoreBoard` folder)
- Browser assets are under `wwwroot/`. Ensure `bootstrap.bundle.min.js` is referenced in `wwwroot/index.html` for navbar collapse.
- If IndexedDB issues occur during dev: delete the `myscoreboard` DB via DevTools → Application → IndexedDB, or bump `DB_VERSION` in `indexedDb.js`.

## Examples for common edits
- Add a field to `GameSession`:
  1) Update `Models/GameModels.cs`.
  2) Reflect in UI (e.g., `GameSetup.razor`/`GamePlay.razor`).
  3) If persisted, ensure `GameService.EndGameAsync()` includes it in `GameStoreEntry`.
  4) Bump `DB_VERSION` if schema for stored objects changes materially.
- Add a history card detail:
  - Use `GetGameTotals()` logic in `Pages/GameHistory.razor` to compute totals; avoid re‑computing in services.
- Confetti on events:
  - Drop `<Confetti />` where needed; JS functions are `startConfetti()` / `stopConfetti()`.

## Gotchas
- Do not pass a key to `put()` on the `games` store (auto‑increment). Do pass `'current'` when writing to `active`.
- `OnInitializedAsync` can run before browser APIs are ready; use `OnAfterRenderAsync(firstRender)` for checks that hit IndexedDB/localStorage.
- Keep UI copy short, accessible, and high contrast.

## Directory pointers
- `MyScoreBoard/Pages/Home.razor` – Resume logic.
- `MyScoreBoard/Pages/GameSetup.razor` – New game + save defaults.
- `MyScoreBoard/Pages/GamePlay.razor` – Round entry, winner modal, confetti.
- `MyScoreBoard/Pages/GameHistory.razor` – History list, delete items (uses `Key`).
- `MyScoreBoard/Services/*` – Game/IndexedDB/localStorage services.
- `MyScoreBoard/wwwroot/js/indexedDb.js` – DB versioning and store helpers.
- `MyScoreBoard/Layout/MainLayout.razor(.css)` – Navbar + hamburger styles.

---
If anything here feels incomplete or you want additional patterns (e.g., testing, deployment), tell me and I’ll extend this doc.
