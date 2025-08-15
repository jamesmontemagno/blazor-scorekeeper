using Microsoft.JSInterop;

namespace MyScoreBoard.Services;

public class LocalStorageService
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
}
