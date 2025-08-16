using System.Threading.Tasks;
using MyScoreBoardShared.Services;
using Microsoft.Maui.Storage;

namespace MyScoreBoardMaui.Services;

public class LocalStorageService : ILocalStorageService
{
    public Task<bool> GetHasActiveGameAsync()
    {
        var value = Preferences.Get("hasActiveGame", "false");
        return Task.FromResult(value?.ToLower() == "true");
    }

    public Task SetHasActiveGameAsync(bool hasActive)
    {
        Preferences.Set("hasActiveGame", hasActive ? "true" : "false");
        return Task.CompletedTask;
    }

    public Task<string?> GetItemAsync(string key)
    {
        var value = Preferences.Get(key, null);
        return Task.FromResult(value);
    }

    public Task SetItemAsync(string key, string value)
    {
        Preferences.Set(key, value);
        return Task.CompletedTask;
    }

    public Task RemoveItemAsync(string key)
    {
        Preferences.Remove(key);
        return Task.CompletedTask;
    }
}
