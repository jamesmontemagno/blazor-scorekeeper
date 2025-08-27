// Shared navbar helpers used by both Web and MAUI hosts
// Requires Bootstrap bundle to be loaded (bootstrap.Collapse)

window.myMenu = window.myMenu || {};

window.myMenu.closeNav = function () {
    try {
        var el = document.getElementById('mainNav');
        if (!el) return;
        var collapse = bootstrap.Collapse.getOrCreateInstance(el, { toggle: false });
        // Only hide if currently shown to avoid flicker
        if (el.classList.contains('show')) {
            collapse.hide();
        }
        // Hide optional mobile backdrop if present
        var backdrop = document.getElementById('nav-backdrop');
        if (backdrop) {
            backdrop.classList.remove('show');
        }
        // Restore body scroll if it was locked
        if (document && document.body) {
            document.body.style.overflow = '';
        }
    } catch (e) {
        // Swallow to avoid breaking navigation
        // console.debug('myMenu.closeNav error', e);
    }
};
