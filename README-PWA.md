# PWA Notes

PWA support added:

- `MyScoreBoard/wwwroot/manifest.webmanifest`
- `MyScoreBoard/wwwroot/service-worker.js` (development)
- `MyScoreBoard/wwwroot/service-worker.published.js` (production)
- Update prompt component: `MyScoreBoard/Components/PWAUpdater.razor` (+ `.razor.css`) with script `MyScoreBoard/wwwroot/js/pwaUpdater.js`

Follow-ups:
- Add a 512x512 icon at `MyScoreBoard/wwwroot/icon-512.png` and include it in the manifest.
- Test install on Chrome/Edge desktop and iOS/Android.
- If hosting under a subpath, update `base` in `service-worker.published.js` accordingly.
