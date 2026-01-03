# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade MyScoreBoardShared/MyScoreBoardShared.csproj
4. Upgrade MyScoreBoardMaui/MyScoreBoardMaui.csproj
5. Upgrade MyScoreBoard/MyScoreBoard.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

No projects are excluded from this upgrade.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                         | Current Version | New Version | Description                      |
|:-----------------------------------------------------|:---------------:|:-----------:|:---------------------------------|
| Microsoft.AspNetCore.Components.Web                  | 9.0.6           | 10.0.1      | Recommended for .NET 10.0        |
| Microsoft.AspNetCore.Components.WebAssembly          | 9.0.6           | 10.0.1      | Recommended for .NET 10.0        |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer| 9.0.6           | 10.0.1      | Recommended for .NET 10.0        |
| Microsoft.Extensions.Logging.Debug                   | 9.0.0           | 10.0.1      | Recommended for .NET 10.0        |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### MyScoreBoardShared/MyScoreBoardShared.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.AspNetCore.Components.Web should be updated from `9.0.6` to `10.0.1` (*recommended for .NET 10.0*)

#### MyScoreBoardMaui/MyScoreBoardMaui.csproj modifications

Project properties changes:
  - Target frameworks should be changed from `net9.0-android;net9.0-ios;net9.0-maccatalyst` to `net10.0-android;net10.0-ios;net10.0-maccatalyst`

NuGet packages changes:
  - Microsoft.Extensions.Logging.Debug should be updated from `9.0.0` to `10.0.1` (*recommended for .NET 10.0*)

API compatibility issues (97 issues - mostly source incompatible, will compile with .NET 10.0):
  - Microsoft.Maui.Controls.BindingMode - 20 occurrences (source incompatible)
  - Microsoft.Maui.Storage.Preferences - 9 occurrences (source incompatible)
  - Microsoft.Maui.Hosting.MauiAppBuilder.Services - 6 occurrences (source incompatible)
  - Microsoft.Maui.Hosting.MauiApp - 5 occurrences (source incompatible)
  - Microsoft.Maui.MauiApplication - 2 occurrences (binary incompatible)
  - Other MAUI APIs with minor source compatibility changes

#### MyScoreBoard/MyScoreBoard.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.AspNetCore.Components.WebAssembly should be updated from `9.0.6` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.AspNetCore.Components.WebAssembly.DevServer should be updated from `9.0.6` to `10.0.1` (*recommended for .NET 10.0*)

Behavioral changes (low impact):
  - System.Uri - 3 occurrences (behavioral change that may require runtime testing)
