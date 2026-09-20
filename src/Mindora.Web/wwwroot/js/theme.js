// ============================================================
// Mindora — Theme Controller (Dark / Light Mode)
// ============================================================
(function () {
    'use strict';

    const THEME_KEY = 'mindora-theme';
    const HTML = document.documentElement;

    // ============================================================
    // INIT
    // ============================================================
    document.addEventListener('DOMContentLoaded', () => {
        syncMeta();
        bindToggles();
    });

    // ============================================================
    // BIND ALL THEME TOGGLES (there may be several per page)
    // ============================================================
    function bindToggles() {
        document.querySelectorAll('#themeToggle').forEach(btn => {
            btn.addEventListener('click', toggleTheme);
        });
    }

    // ============================================================
    // TOGGLE
    // ============================================================
    function toggleTheme() {
        const current = get();
        const next = current === 'dark' ? 'light' : 'dark';
        set(next);
    }

    // ============================================================
    // GET / SET
    // ============================================================
    function get() {
        return HTML.getAttribute('data-theme') || 'light';
    }

    function set(theme) {
        HTML.setAttribute('data-theme', theme);

        try {
            localStorage.setItem(THEME_KEY, theme);
        } catch (e) {
            // localStorage blocked — ignore
        }

        syncMeta();
        emitChange(theme);
    }

    // ============================================================
    // META THEME-COLOR (mobile browser address bar)
    // ============================================================
    function syncMeta() {
        const meta = document.querySelector('meta[name="theme-color"]');
        if (meta) {
            meta.setAttribute('content', get() === 'dark' ? '#0F172A' : '#06B6D4');
        }
    }

    // ============================================================
    // CUSTOM EVENT — other components can listen
    // ============================================================
    function emitChange(theme) {
        window.dispatchEvent(new CustomEvent('mindora:themechange', {
            detail: { theme }
        }));
    }

    // ============================================================
    // SYSTEM PREFERENCE LISTENER
    // If user hasn't set explicit choice → follow OS
    // ============================================================
    try {
        const media = window.matchMedia('(prefers-color-scheme: dark)');
        media.addEventListener('change', (e) => {
            const stored = localStorage.getItem(THEME_KEY);
            if (!stored) {
                set(e.matches ? 'dark' : 'light');
            }
        });
    } catch (e) { /* ignore */ }

    // ============================================================
    // KEYBOARD SHORTCUT — Ctrl/Cmd + Shift + L
    // ============================================================
    document.addEventListener('keydown', (e) => {
        if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key.toLowerCase() === 'l') {
            e.preventDefault();
            toggleTheme();
        }
    });

    // ============================================================
    // PUBLIC API
    // ============================================================
    window.MindoraTheme = {
        get: get,
        set: set,
        toggle: toggleTheme
    };
})();