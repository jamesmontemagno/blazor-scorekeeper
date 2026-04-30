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
    } catch (e) {
        // Swallow to avoid breaking navigation
        // console.debug('myMenu.closeNav error', e);
    }
};

// Accessibility: prevent page scroll when Space is pressed on ARIA role="button" elements.
// Native <button> elements handle this automatically; non-button elements with role="button"
// must call event.preventDefault() in a synchronous JS handler (Blazor C# runs too late).
document.addEventListener('keydown', function (e) {
    if (e.key === ' ' && e.target.getAttribute('role') === 'button') {
        e.preventDefault();
    }
}, { capture: true });
