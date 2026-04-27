using Microsoft.JSInterop;

namespace MyScoreBoardShared.Services;

public class ThemeService : IThemeService
{
    private const string ThemeKey = "appTheme";
    private static readonly HashSet<string> ValidThemes = new(StringComparer.OrdinalIgnoreCase) { "system", "light", "dark" };

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
        var theme = await _localStorage.GetItemAsync(ThemeKey);
        return IsValidTheme(theme) ? theme! : "system";
    }

    public async Task SetThemeAsync(string theme)
    {
        theme = IsValidTheme(theme) ? theme : "system";
        await _localStorage.SetItemAsync(ThemeKey, theme);
        await ApplyThemeAsync(theme);
        ThemeChanged?.Invoke();
    }

    public async Task ApplyThemeAsync(string theme)
    {
        try
        {
            // Use a window-level JS helper instead of dot-notation DOM calls,
            // which can fail silently in MAUI's BlazorWebView JS interop.
            await _js.InvokeVoidAsync("setAppTheme", IsValidTheme(theme) ? theme : "system");
        }
        catch
        {
            // Ignore JS interop errors (e.g., during prerendering)
        }
    }

    private static bool IsValidTheme(string? theme) => theme is not null && ValidThemes.Contains(theme);
}
