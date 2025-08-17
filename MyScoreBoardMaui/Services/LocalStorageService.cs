using System.Threading.Tasks;
using MyScoreBoardShared.Services;
using Microsoft.Maui.Storage;

namespace MyScoreBoardMaui.Services;

public class LocalStorageService : ILocalStorageService
{
    public Task<bool> GetHasActiveGameAsync()
    {
        var value = Preferences.Get("hasActiveGame", false);
        return Task.FromResult(value);
    }

    public Task SetHasActiveGameAsync(bool hasActive)
    {
        Preferences.Set("hasActiveGame", hasActive);
        return Task.CompletedTask;
    }

    public Task<string?> GetItemAsync(string key)
    {
        var value = Preferences.Get(key, string.Empty);
        return Task.FromResult(string.IsNullOrWhiteSpace(value) ? null : value);
    }

    public Task SetItemAsync(string key, string value)
    {
        Preferences.Set(key, value ?? string.Empty);
        return Task.CompletedTask;
    }

    public Task RemoveItemAsync(string key)
    {
        Preferences.Remove(key);
        return Task.CompletedTask;
    }

    public Task<string?> GetLastGameNameAsync()
    {
        var value = Preferences.Get("lastGameName", string.Empty);
        return Task.FromResult(string.IsNullOrWhiteSpace(value) ? null : value);
    }

    public Task SetLastGameNameAsync(string gameName)
    {
        Preferences.Set("lastGameName", gameName ?? string.Empty);
        return Task.CompletedTask;
    }

    public Task<List<string>?> GetLastPlayersAsync()
    {
        try
        {
            var playersJson = Preferences.Get("lastPlayers", string.Empty);
            if (string.IsNullOrWhiteSpace(playersJson))
                return Task.FromResult<List<string>?>(null);
            
            var players = System.Text.Json.JsonSerializer.Deserialize<List<string>>(playersJson);
            return Task.FromResult<List<string>?>(players);
        }
        catch
        {
            return Task.FromResult<List<string>?>(null);
        }
    }

    public Task SetLastPlayersAsync(List<string> playerNames)
    {
        try
        {
            var playersJson = System.Text.Json.JsonSerializer.Serialize(playerNames);
            Preferences.Set("lastPlayers", playersJson);
        }
        catch
        {
            // Ignore serialization errors
        }
        return Task.CompletedTask;
    }
}
