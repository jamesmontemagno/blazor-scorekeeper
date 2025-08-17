# MyScoreBoard

A modern, mobile-friendly board game scorekeeper. Built with .NET 9 Blazor WebAssembly and a touch of flair. Tracks players, rounds, totals, history, and lets you pause/resume games with persistent storage in the browser.

## ✨ Features

- Quick game setup with saved defaults (game name + players)
- Round-based scoring with fast +/- adjustments and negative scores
- Automatic totals and leader/tie indicators
- Resume active game from Home (auto-saved in background)
- End-game winner modal with confetti celebration
- Game history with players, scores, and winners
- Clean, high-contrast, glassy UI tuned for mobile

## 🧱 Tech Stack

- Blazor WebAssembly (.NET 9)
- Bootstrap 5 styling (with custom glass theme)
- IndexedDB for durable storage (custom JS interop)
- localStorage for quick flags and preferences
 - sqlite-net-pcl for local persistence on MAUI
 - Shared models and service interfaces in `MyScoreBoardShared` to enable code sharing between Blazor and MAUI

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK

### Run the app
```pwsh
# from repo root
cd MyScoreBoard
dotnet build
dotnet run
# the app will serve at http://localhost:5261
```

### Project structure
```
MyScoreBoard.sln
MyScoreBoard/
  App.razor
  Program.cs
  Pages/
    Home.razor        # Landing page with Resume button when a game is active
    GameSetup.razor   # Create a new game and add players
    GamePlay.razor    # Round entry, totals, leader/tie markers, end game
    GameHistory.razor # History cards showing players, scores, winners
  Layout/
    MainLayout.razor  # Top navbar (mobile friendly)
  Components/
    Confetti.razor    # Celebration overlay
  Models/
    GameModels.cs     # Player, Round, GameSession, GameStoreEntry
  Services/
    GameService.cs        # Game state + persistence orchestration
    IndexedDbService.cs   # Blazor <-> JS interop
    LocalStorageService.cs# Simple localStorage helper
  wwwroot/
    index.html
    css/app.css
    js/indexedDb.js   # IndexedDB store helpers (games, active)
    js/confetti.js    # Confetti animation (canvas-based)
    lib/bootstrap/
```

## 💾 Data Persistence

Web (Blazor) persistence
- Active games are saved to the `active` store in IndexedDB (via `wwwroot/js/indexedDb.js`).
- Completed games are appended to the `games` store.
- A quick `hasActiveGame` boolean is mirrored in `localStorage` for snappy UI.
- The web `IndexedDbService` wraps JS interop and exposes async store helpers.

MAUI persistence
- MAUI platforms use `sqlite-net-pcl` and a sqlite-backed `IndexedDbService` implementation that persists `GameStoreEntry` records in a local database file (`myscoreboard.db3`).
- `ILocalStorageService` on MAUI is implemented with `Preferences` for quick flags.

Shared architecture
- Models (Player, Round, GameSession, GameStoreEntry) live in `MyScoreBoardShared/Models` so both apps share types.
- Service interfaces (`IGameService`, `IIndexedDbService`, `ILocalStorageService`) live in `MyScoreBoardShared/Services`.
- The Blazor app registers JS-backed services; the MAUI app registers platform-backed implementations; both consume the shared interfaces via DI.

If you ever hit an IndexedDB schema issue in dev:
- Clear site data or delete the `myscoreboard` DB (Application tab → IndexedDB), then refresh.

If you ever hit an IndexedDB schema issue in dev:
- Clear site data or delete the `myscoreboard` DB (Application tab → IndexedDB), then refresh.

## 🧭 Usage Tips

- Tap a player card to add a score (+/- quick buttons included)
- The Next Round button enables once all players have scored this round
- End Game to see the winner popup and confetti, then return Home
- History cards show top players and final scores; delete any entry as needed

## 🛠️ Troubleshooting

- "Resume" button not showing:
  - Home checks `localStorage` first, then verifies IndexedDB in the background
  - Start a game in Setup to create an active session
- IndexedDB errors after refactor:
  - We version and recreate the `active` object store when needed
  - If something looks stale, clear the site data and refresh
- Hamburger not opening on mobile:
  - We include `bootstrap.bundle.min.js` and use `navbar-dark` with white icon

## 🤝 Contributing

Issues and PRs are welcome! Keep changes focused and include a short description of the UX impact.

## 📄 License

MIT
