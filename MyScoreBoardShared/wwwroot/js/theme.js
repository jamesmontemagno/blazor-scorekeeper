// Theme management helpers for cross-platform Blazor JS interop
window.setAppTheme = function(theme) {
    const validThemes = ['system', 'light', 'dark'];
    const nextTheme = validThemes.includes(theme) ? theme : 'system';
    document.documentElement.setAttribute('data-theme', nextTheme);
    try {
        localStorage.setItem('appTheme', nextTheme);
    } catch (e) {
        console.warn('Failed to persist theme to localStorage:', e);
    }
};
