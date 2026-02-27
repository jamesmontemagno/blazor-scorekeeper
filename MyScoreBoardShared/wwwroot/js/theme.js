// Theme management helpers for cross-platform Blazor JS interop
window.setAppTheme = function(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    try {
        localStorage.setItem('appTheme', theme);
    } catch (e) {
        console.warn('Failed to persist theme to localStorage:', e);
    }
};
