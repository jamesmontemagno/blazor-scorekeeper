using Microsoft.JSInterop;

namespace MyScoreBoard.Services;

public class LocalStorageService : MyScoreBoardShared.Services.ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> GetHasActiveGameAsync()
    {
        try
        {
            var value = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "hasActiveGame");
            return !string.IsNullOrEmpty(value) && value.ToLower() == "true";
        }
        catch
        {
            return false;
        }
    }

    public async Task SetHasActiveGameAsync(bool hasActive)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "hasActiveGame", hasActive ? "true" : "false");
        }
        catch
        {
            // Ignore localStorage errors
        }
    }

    public async Task<string?> GetItemAsync(string key)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetItemAsync(string key, string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch
        {
            // Ignore localStorage errors
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch
        {
            // Ignore localStorage errors
        }
    }

    public async Task<string?> GetLastGameNameAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "lastGameName");
        }
        catch
        {
            return null;
        }
    }

    public async Task SetLastGameNameAsync(string gameName)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "lastGameName", gameName);
        }
        catch
        {
            // Ignore localStorage errors
        }
    }

    public async Task<List<string>?> GetLastPlayersAsync()
    {
        try
        {
            var playersJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "lastPlayers");
            if (string.IsNullOrEmpty(playersJson))
                return null;
            
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(playersJson);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetLastPlayersAsync(List<string> playerNames)
    {
        try
        {
            var playersJson = System.Text.Json.JsonSerializer.Serialize(playerNames);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "lastPlayers", playersJson);
        }
        catch
        {
            // Ignore localStorage errors
        }
    }

    public async Task<List<string>> GetFavoriteGamesAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "favoriteGames");
            if (string.IsNullOrEmpty(json)) return new List<string>();
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task SetFavoriteGamesAsync(List<string> games)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(games);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "favoriteGames", json);
        }
        catch { }
    }

    public async Task<List<string>> GetFavoritePlayersAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "favoritePlayers");
            if (string.IsNullOrEmpty(json)) return new List<string>();
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task SetFavoritePlayersAsync(List<string> players)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(players);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "favoritePlayers", json);
        }
        catch { }
    }
}
