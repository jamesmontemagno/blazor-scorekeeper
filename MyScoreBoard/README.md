# MyScoreBoard (Blazor WebAssembly)

A simple score keeper inspired by the React app, using IndexedDB via JS interop for local storage.

- Pages: Home, Setup, Play, History
- Storage: IndexedDB `myscoreboard` database with `games` store

## Run

Build and run the project (Debug configuration):

- Restore/build: dotnet build
- Launch: use F5 or `dotnet run` from the MyScoreBoard project folder

## Notes

- JS module at `wwwroot/js/indexedDb.js` implements init/add/get/delete.
- C# `IndexedDbService` loads the module and provides typed methods.
