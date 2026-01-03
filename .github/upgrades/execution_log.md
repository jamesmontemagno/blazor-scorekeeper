# .NET 10 Upgrade Report

## Project target framework modifications

| Project name                                      | Old Target Framework                             | New Target Framework                                  | Commits                      |
|:--------------------------------------------------|:------------------------------------------------:|:-----------------------------------------------------:|:-----------------------------|
| MyScoreBoardShared/MyScoreBoardShared.csproj      | net9.0                                           | net10.0                                               | af8d155, 20afcff             |
| MyScoreBoardMaui/MyScoreBoardMaui.csproj          | net9.0-android;net9.0-ios;net9.0-maccatalyst     | net10.0-android;net10.0-ios;net10.0-maccatalyst       | 0c7048d, 5fe3920, 430cdc5    |
| MyScoreBoard/MyScoreBoard.csproj                  | net9.0                                           | net10.0                                               | 8229441, 1609a63             |

## NuGet Packages

| Package Name                                          | Old Version | New Version | Commit Id        |
|:------------------------------------------------------|:-----------:|:-----------:|:-----------------|
| Microsoft.AspNetCore.Components.Web                   | 9.0.6       | 10.0.1      | 20afcff          |
| Microsoft.AspNetCore.Components.WebAssembly           | 9.0.6       | 10.0.1      | 1609a63          |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 9.0.6       | 10.0.1      | 1609a63          |
| Microsoft.AspNetCore.Components.WebView.Maui          | 9.0.120     | 10.0.20     | 5fe3920          |
| Microsoft.Extensions.Logging.Debug                    | 9.0.0       | 10.0.1      | 5fe3920          |
| Microsoft.Maui.Controls                               | 9.0.120     | 10.0.20     | 5fe3920          |

## All commits

| Commit ID              | Description                                                          |
|:-----------------------|:---------------------------------------------------------------------|
| 62c6550                | Commit changes before fixing global.json file(s)                     |
| af8d155                | Update to .NET 10 and fix project type GUID                          |
| 20afcff                | Update Microsoft.AspNetCore.Components.Web to 10.0.1                 |
| 0c7048d                | Upgrade MyScoreBoardMaui.csproj properties and items to net10.0      |
| 5fe3920                | Upgrade MyScoreBoardMaui.csproj dependencies                         |
| 430cdc5                | Update target frameworks in MyScoreBoardMaui.csproj                  |
| 8229441                | Upgrade MyScoreBoard.csproj properties and items to net10.0          |
| 1609a63                | Update MyScoreBoard.csproj to Blazor 10.0.1 packages                 |
| 76ea380                | Update CI workflow for .NET 10                                       |

## Project feature upgrades

### MyScoreBoardShared

- Target framework changed from net9.0 to net10.0
- Microsoft.AspNetCore.Components.Web upgraded from 9.0.6 to 10.0.1

### MyScoreBoardMaui

- Target frameworks changed from net9.0-android;net9.0-ios;net9.0-maccatalyst to net10.0-android;net10.0-ios;net10.0-maccatalyst
- Fixed Windows-only targeting issue by making Windows target conditional on Windows OS
- Microsoft.Maui.Controls upgraded from 9.0.120 to 10.0.20
- Microsoft.AspNetCore.Components.WebView.Maui upgraded from 9.0.120 to 10.0.20
- Microsoft.Extensions.Logging.Debug upgraded from 9.0.0 to 10.0.1
- Android SDK API level 36 installed for .NET 10 Android target

### MyScoreBoard

- Target framework changed from net9.0 to net10.0
- Microsoft.AspNetCore.Components.WebAssembly upgraded from 9.0.6 to 10.0.1
- Microsoft.AspNetCore.Components.WebAssembly.DevServer upgraded from 9.0.6 to 10.0.1

## CI/CD Updates

- Updated .github/workflows/maui-ios.yml for .NET 10 (dotnet-version: 10.0.x, macos-26 runner)

## Next steps

- Run the application locally to validate all functionality works as expected
- Test on iOS simulator, Android emulator, and Mac Catalyst
- Push the upgrade-to-NET10 branch and create a pull request for review
