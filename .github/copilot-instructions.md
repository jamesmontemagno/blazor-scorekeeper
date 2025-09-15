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

CSS architecture & styling (important)
- **Shared CSS**: `MyScoreBoardShared/wwwroot/css/app.css` (617 lines) contains the complete component foundation - CSS variables, glass effects, animations, all shared component styles. This is the single source of truth for styling.
- **Web CSS**: `MyScoreBoard/wwwroot/css/app.css` (321 lines) contains web-specific enhancements only - base layout (html/body background), Blazor loading UI, web-specific overrides for better contrast.
- **MAUI CSS**: `MyScoreBoardMaui/wwwroot/css/app.css` (374 lines) contains mobile-specific optimizations - touch targets (44px minimum), safe area handling, iOS/Android platform tweaks, mobile layout adjustments.
- **Static Assets**: All Bootstrap CSS/JS and Bootstrap Icons are centralized in `MyScoreBoardShared/wwwroot/` and referenced via `_content/MyScoreBoardShared/` path in both hosts.
- **CSS Loading Order**: Both apps load in this order: Bootstrap CSS → Platform CSS → Shared CSS → Generated styles. This ensures proper cascade and override behavior.
- **When adding new components**: Add base styles to shared CSS first, then platform-specific tweaks to web/MAUI CSS if needed. Always test both platforms after changes.
- **Bootstrap Integration**: Local Bootstrap Icons (no CDN), centralized in shared project. Icon usage: `bi-*` classes work across all platforms. Bootstrap components: cards, buttons, forms, modals, navbar, badges, alerts, grid all supported.

Build / dev workflow (practical)
- Web: cd `MyScoreBoard` → `dotnet build` / `dotnet run` (serves the Blazor app).
- MAUI: prefer Android target for local CI-free checks: `dotnet build MyScoreBoardMaui/MyScoreBoardMaui.csproj -f net9.0-android`.
- Full solution: `dotnet build MyScoreBoard.sln` (note: iOS/MacCatalyst builds may fail locally if Xcode version mismatch; see note below).

Key files to inspect for edits
- UI: `MyScoreBoard/Pages/*` (`Home.razor`, `GameSetup.razor`, `GamePlay.razor`, `GameHistory.razor`) - now located in `MyScoreBoardShared/Pages/*` for cross-platform sharing.
- Layout: `MyScoreBoardShared/Layout/*` (`MainLayout.razor`, `NavMenu.razor`) - shared layout components with glass card theme.
- State/persistence: `MyScoreBoard/Services/GameService.cs`, `MyScoreBoard/Services/IndexedDbService.cs`, `MyScoreBoard/Services/LocalStorageService.cs`.
- Shared contracts: `MyScoreBoardShared/Models/GameModels.cs`, `MyScoreBoardShared/Services/*`.
- MAUI implementations: `MyScoreBoardMaui/Services/*` and `MyScoreBoardMaui/MauiProgram.cs`.
- CSS files: `MyScoreBoardShared/wwwroot/css/app.css` (shared), `MyScoreBoard/wwwroot/css/app.css` (web), `MyScoreBoardMaui/wwwroot/css/app.css` (mobile).
- Static assets: `MyScoreBoardShared/wwwroot/` (Bootstrap, icons, JS) accessed via `_content/MyScoreBoardShared/` in both hosts.

Examples & small contracts
- Adding a field to `GameSession`:
  - Update `MyScoreBoardShared/Models/GameModels.cs`.
  - Update `GameService.EndGameAsync()` to persist the new field (both web and MAUI storage representations).
  - If web persistence shape changed, bump DB schema version in `wwwroot/js/indexedDb.js` for dev iterations.
- Adding a new UI component:
  - Create component in `MyScoreBoardShared/Components/` for cross-platform use.
  - Add base styles to `MyScoreBoardShared/wwwroot/css/app.css` using CSS variables and glass card theme.
  - Add platform-specific enhancements to `MyScoreBoard/wwwroot/css/app.css` (web) or `MyScoreBoardMaui/wwwroot/css/app.css` (mobile) if needed.
  - Test both web and MAUI builds after changes.
- Adding Bootstrap components:
  - Use existing Bootstrap classes (cards, buttons, forms, modals, navbar, badges, alerts, grid).
  - Icons: use `bi-*` classes from local Bootstrap Icons (no CDN).
  - All Bootstrap assets loaded from `MyScoreBoardShared/wwwroot/` via `_content/` path.

Gotchas & project-specific conventions
- IndexedDB `games` store is auto-increment; do not pass a key when adding. Use explicit key `'current'` for the `active` store.
- `OnInitializedAsync` may run before browser APIs are ready — use `OnAfterRenderAsync(firstRender)` for IndexedDB/localStorage access.
- Shared project exposes models and interfaces; concrete implementations live in each host (web vs MAUI). Prefer editing the shared interfaces when adding capabilities, then implement per-host.
- CSS inheritance: shared styles are inherited by both platforms. Only add platform-specific CSS when absolutely necessary. Use CSS variables (`--bs-primary`, `--glass-bg`, etc.) for theming.
- Bootstrap Icons: all icons are local files, no CDN dependencies. Use `bi-*` classes directly (e.g., `bi-trophy-fill`, `bi-plus-circle-fill`).
- Glass card theme: use `.glass-card` class for consistent styling. Text contrast utilities: `.text-contrast`, `.text-contrast-muted`, `.text-contrast-white`.

Troubleshooting / environment notes
- MAUI iOS/MacCatalyst builds often require matching Xcode (example: Xcode 16.4 required by some .NET workloads). If a solution build fails with Xcode errors, either update Xcode or build only Android and Blazor locally.
- Android build warnings may appear from native sqlite binaries (page-size warnings); these are non-blocking but worth tracking when upgrading native libs.

If anything here is unclear or you want more patterns (testing, deployments, CI config), say which area to expand and include any preferred conventions.

```


## Extended CSS Architecture & Contribution Guide

This section defines exactly where new styles belong, how to evolve existing ones, and how to avoid fragmentation now that most UI has been unified.

### Layering Model (Load Order & Responsibility)
1. Bootstrap base (from shared static assets)
2. Platform CSS (Web: `MyScoreBoard/wwwroot/css/app.css`, MAUI: `MyScoreBoardMaui/wwwroot/css/app.css`)
3. Shared CSS (`MyScoreBoardShared/wwwroot/css/app.css`) – PRIMARY layer for nearly all component & theme styling
4. Generated / component-scoped CSS (e.g., `Component.razor.css`) – only for true one-off visual tweaks that should not cascade

Rule of thumb: If more than one component could reuse a style, it belongs in the shared CSS. If only one component needs it temporarily, put it in a `.razor.css` file (then promote later if reused).

### Ownership by File
| File | Purpose | Add Here When |
|------|---------|---------------|
`MyScoreBoardShared/wwwroot/css/app.css` | Design system tokens (variables), reusable utility classes, component base styles (`.glass-card`, buttons, player cards, navbar, animations) | 90% of new styles, any cross-platform element | 
`MyScoreBoard/wwwroot/css/app.css` | Web-only experiential chrome: initial page background, loading UI, minor contrast tweaks | The effect is visual-only on Web OR would degrade MAUI perf/clarity | 
`MyScoreBoardMaui/wwwroot/css/app.css` | Mobile-only ergonomics: safe areas, touch target scaling, platform gesture edge cases | The rule depends on device viewport constraints or safe-area insets | 
`*.razor.css` | Truly component-local micro-adjustments not yet generalized | A tweak is experimental or not proven reusable |

### Tokens & Variables
Defined centrally in `:root` inside shared CSS. Always try a variable before inventing a hard-coded color/size. If a new semantic is needed:
1. Add variable to `:root` (choose semantic name: `--score-warning-bg`, not `--yellowish-bg`).
2. Use it in styles.
3. Avoid per-platform overrides unless absolutely needed—prefer adaptable base values.

### Adding a New Component Style
1. Create component in `MyScoreBoardShared/Components`.
2. Add its class block(s) to shared CSS (group near similar domains, use a section comment header).
3. If mobile requires spacing or tap tweaks, layer a media query inside shared CSS first (`@media (max-width: 767.98px)`), only move to MAUI CSS if it causes desktop regression.
4. If web needs large-screen layout improvements, add a desktop media query in shared CSS or put in web CSS if it’s purely decorative.

### Navbar & Global Layout
- Unified styling now lives in shared CSS.
- Do NOT reintroduce host-specific navbar styles unless there is a platform accessibility issue.
- Mobile collapse should rely on default Bootstrap structure; any future variation must remain accessible (focusable toggler, visible focus outline).

### Player Card States
- Base: `.player-card` (glass aesthetic)
- Leader: `.leader-card` (gradient champion style)
- Pending (no explicit round score yet): `.player-card.needs-score` (high contrast; only adjust here if readability regressions appear)
- Avoid creating additional ad-hoc modifier classes; reuse or extend with a BEM-like suffix (`.player-card--alert`) only if semantically distinct.

### Modifier & Utility Naming
Use short, semantic, non-presentational names:
- Good: `.needs-score`, `.score-pending`, `.text-contrast-muted`
- Avoid: `.yellow-bg`, `.big-shadow2`, `.mobile-only-box`

### Media Query Standards
- Mobile breakpoint: `max-width: 767.98px` (Bootstrap md threshold)
- Prefer feature queries (e.g., `@supports(backdrop-filter: blur(10px)) {}`) only if a degradation would look broken; otherwise allow graceful fallback.

### Animations
- Place keyframes once in shared CSS near the bottom under an `/* Animations */` comment.
- Reuse existing ones (`fade-pulse`, `trophy-shimmer`) before adding new.
- New animation? Prefix with domain if specific (e.g., `@keyframes player-card-attn`).

### Performance & Specificity Guidelines
- Keep selectors shallow: `.glass-card h2` acceptable; avoid long descendant chains.
- Do not use `!important` unless overriding 3rd-party (Bootstrap) or providing a required state override (already used sparingly in glass theme). If you need it repeatedly, restructure the cascade instead.
- Avoid inline styles except for dynamic values that are hard to express via classes (we now have very few, keep it that way).

### Promoting a Local Style to Shared
When a pattern appears in a second component:
1. Move declarations into shared CSS under an appropriate section.
2. Replace component-local declarations with the new class name.
3. Delete obsolete local rules.

### Refactoring / Cleanup Workflow
1. Identify duplication (search for a distinctive token across css files).
2. Consolidate into shared.
3. Remove host copies.
4. Build (web + MAUI) and manually smoke test: navbar toggle, player cards, modals, forms.
5. Commit with message: `css: consolidate <area> into shared`.

### Accessibility Considerations
- Maintain minimum 4.5:1 contrast for body text on non-decorative surfaces; use the `.needs-score` pattern as reference for high contrast.
- Interactive elements must show a visible focus ring (Bootstrap default + any custom outlines). If a custom style suppresses outline, reintroduce `:focus-visible` styling.

### When NOT to Touch Shared CSS
- A change is experimental or you’re unsure about cross-platform impact → put it in a component `.razor.css` temporarily.
- A style is purely for a loading screen placeholder (web only) → keep it in web CSS.
- Platform-specific safe area/resizing concerns → MAUI CSS only.

### Quick Decision Tree
Adding style for a reusable component? → Shared CSS.
Tweak only for phone ergonomics? → Try media query in shared first; if conflict, MAUI CSS.
Visual polish for web loading boot UI? → Web CSS.
One-off experiment? → Component `.razor.css` (promote later).

### Example: Adding a New Badge Variant
1. Add variable (if needed) to shared `:root` (e.g., `--badge-accent-bg`).
2. Add `.badge-accent { background: var(--gradient-secondary); color:#fff; }` to shared CSS near existing badge styles.
3. Use `<span class="badge badge-accent">…</span>` in any Razor page.
4. If mobile needs larger tap target, extend with `.badge-accent { padding: .55rem .85rem; }` inside mobile breakpoint media query.

### Example: Creating a Responsive Grid Utility
Add to shared CSS:
```
.grid-auto-fill {
  display: grid;
  gap: 1rem;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
}
```
Then apply `<div class="grid-auto-fill">…</div>` anywhere; no platform override unless a device perf issue emerges.

### Style Review Checklist (Before Commit)
1. Is this reusable? If yes → shared.
2. Did I duplicate an existing pattern? If yes → unify instead.
3. Any new `!important`? Justify or remove.
4. Built both hosts? (web + MAUI Android or iOS) Ensure no layout regressions.
5. Checked dark/light contrast on glass surfaces.

Following this guide keeps styling consistent, maintainable, and reduces future refactors.

