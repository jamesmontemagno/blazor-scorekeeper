using Microsoft.JSInterop;

namespace MyScoreBoardShared.Services;

public class ThemeService : IThemeService
{
    private const string ThemeKey = "appTheme";

    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _js;

    public event Action? ThemeChanged;

    public ThemeService(ILocalStorageService localStorage, IJSRuntime js)
    {
        _localStorage = localStorage;
        _js = js;
    }

    public async Task<string> GetThemeAsync()
    {
        return await _localStorage.GetItemAsync(ThemeKey) ?? "system";
    }

    public async Task SetThemeAsync(string theme)
    {
        await _localStorage.SetItemAsync(ThemeKey, theme);
        await ApplyThemeAsync(theme);
        ThemeChanged?.Invoke();
    }

    public async Task ApplyThemeAsync(string theme)
    {
        try
        {
            await _js.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
            // Also persist to browser localStorage so the inline script in index.html
            // can restore the theme on next app launch (MAUI uses native Preferences for
            // app-level storage, but the HTML inline script reads browser localStorage).
            await _js.InvokeVoidAsync("localStorage.setItem", "appTheme", theme);
        }
        catch
        {
            // Ignore JS interop errors (e.g., during prerendering)
        }
    }
}
