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
}
