namespace MyScoreBoardShared.Services;

public interface IThemeService
{
    /// <summary>Returns the persisted theme name ("system", "light", "dark", or "fun").</summary>
    Task<string> GetThemeAsync();

    /// <summary>Persists the theme and applies it to the document immediately.</summary>
    Task SetThemeAsync(string theme);

    /// <summary>Applies the given theme to the document via JS interop (sets data-theme on &lt;html&gt;).</summary>
    Task ApplyThemeAsync(string theme);

    /// <summary>Raised after the theme is changed via <see cref="SetThemeAsync"/>.</summary>
    event Action? ThemeChanged;
}
