// ============================================================
// Mindora — Global App Utilities
// Handles loading bar, toast, smooth scroll, alerts
// ============================================================
(function () {
    'use strict';

    // ============================================================
    // LOADING BAR
    // ============================================================
    let loadingBar = null;
    let loadingTimeout = null;

    function ensureLoadingBar() {
        if (loadingBar) return loadingBar;
        loadingBar = document.createElement('div');
        loadingBar.className = 'mindora-loading-bar';
        document.body.appendChild(loadingBar);
        return loadingBar;
    }

    function startLoading() {
        const bar = ensureLoadingBar();
        bar.classList.add('active');
        bar.style.width = '30%';
        clearTimeout(loadingTimeout);
        loadingTimeout = setTimeout(() => { bar.style.width = '70%'; }, 200);
    }

    function stopLoading() {
        const bar = ensureLoadingBar();
        bar.style.width = '100%';
        clearTimeout(loadingTimeout);
        setTimeout(() => {
            bar.classList.remove('active');
            setTimeout(() => { bar.style.width = '0%'; }, 300);
        }, 200);
    }

    const originalFetch = window.fetch;
    window.fetch = function (...args) {
        const url = typeof args[0] === 'string' ? args[0] : args[0]?.url || '';
        const silent = url.includes('/_dev/') || url.includes('/GetUnreadCount');
        if (!silent) startLoading();
        return originalFetch.apply(this, args).finally(() => {
            if (!silent) stopLoading();
        });
    };

    // ============================================================
    // QUICK TOAST
    // ============================================================
    let quickToast = null;
    let toastTimeout = null;

    function ensureToast() {
        if (quickToast) return quickToast;
        quickToast = document.createElement('div');
        quickToast.className = 'mindora-quick-toast';
        document.body.appendChild(quickToast);
        return quickToast;
    }

    function showToast(message, duration = 2500) {
        const toast = ensureToast();
        toast.textContent = message;
        toast.classList.add('show');
        clearTimeout(toastTimeout);
        toastTimeout = setTimeout(() => { toast.classList.remove('show'); }, duration);
    }

    // ============================================================
    // SMOOTH SCROLL — handles both "#xxx" and "/#xxx"
    // ============================================================
    const NAVBAR_OFFSET = 90;

    function initSmoothScroll() {
        document.querySelectorAll('a[href*="#"]').forEach(link => {
            link.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                if (!href || href === '#') return;

                const hashIndex = href.indexOf('#');
                if (hashIndex === -1) return;

                const hash = href.substring(hashIndex + 1);
                const pathPart = href.substring(0, hashIndex);

                if (!hash) return;

                // Check if link target is on current page
                const currentPath = window.location.pathname || '/';
                const isSamePage =
                    pathPart === '' ||
                    pathPart === '/' ||
                    pathPart === currentPath ||
                    (currentPath === '/' && (pathPart === '/' || pathPart === ''));

                if (!isSamePage) return; // Different page → let browser navigate

                const target = document.getElementById(hash);
                if (!target) return;

                e.preventDefault();

                const targetPosition = target.getBoundingClientRect().top
                    + window.pageYOffset
                    - NAVBAR_OFFSET;

                window.scrollTo({
                    top: Math.max(0, targetPosition),
                    behavior: 'smooth'
                });

                if (history.pushState) {
                    history.pushState(null, null, '#' + hash);
                }
            });
        });
    }

    function handleInitialHash() {
        if (!window.location.hash) return;

        const target = document.getElementById(window.location.hash.substring(1));
        if (!target) return;

        setTimeout(() => {
            const targetPosition = target.getBoundingClientRect().top
                + window.pageYOffset
                - NAVBAR_OFFSET;

            window.scrollTo({
                top: Math.max(0, targetPosition),
                behavior: 'smooth'
            });
        }, 100);
    }

    // ============================================================
    // AUTO DISMISS ALERTS
    // ============================================================
    function initAutoDismiss() {
        document.querySelectorAll('.profile-alert-success, .alert-success').forEach(alert => {
            setTimeout(() => {
                alert.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
                alert.style.opacity = '0';
                alert.style.transform = 'translateY(-10px)';
                setTimeout(() => alert.remove(), 500);
            }, 4000);
        });
    }

    // ============================================================
    // CONFIRM DELETE LINKS
    // ============================================================
    function initConfirmDialogs() {
        document.querySelectorAll('[data-confirm]').forEach(el => {
            el.addEventListener('click', (e) => {
                const message = el.getAttribute('data-confirm') || 'Are you sure?';
                if (!confirm(message)) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
        });
    }

    // ============================================================
    // INIT
    // ============================================================
    document.addEventListener('DOMContentLoaded', () => {
        initSmoothScroll();
        handleInitialHash();
        initAutoDismiss();
        initConfirmDialogs();
    });

    // ============================================================
    // PUBLIC API
    // ============================================================
    window.Mindora = {
        toast: showToast,
        startLoading: startLoading,
        stopLoading: stopLoading
    };
})();