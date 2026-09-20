// ============================================================
// Mindora — Sidebar Controller
// Handles: collapse (desktop), mobile drawer, active link detection
// ============================================================
(function () {
    'use strict';

    // ============================================================
    // CONSTANTS
    // ============================================================
    const COLLAPSE_KEY = 'mindora-sidebar-collapsed';
    const MOBILE_BREAKPOINT = 992;

    // ============================================================
    // INIT
    // ============================================================
    document.addEventListener('DOMContentLoaded', () => {
        // Only run if sidebar exists on page
        if (!document.querySelector('.mindora-sidebar')) return;

        restoreCollapsedState();
        highlightActiveLink();
        bindDesktopToggle();
        bindMobileToggle();
        bindEscKey();
    });

    // ============================================================
    // STATE — DESKTOP COLLAPSE
    // ============================================================
    function restoreCollapsedState() {
        if (isMobile()) return;

        try {
            const collapsed = localStorage.getItem(COLLAPSE_KEY) === '1';
            if (collapsed) {
                document.body.classList.add('mindora-sidebar-collapsed');
            }
        } catch (e) {
            /* localStorage blocked */
        }
    }

    function bindDesktopToggle() {
        const btn = document.getElementById('sidebarToggle');
        if (!btn) return;

        btn.addEventListener('click', () => {
            document.body.classList.toggle('mindora-sidebar-collapsed');

            try {
                const collapsed = document.body.classList.contains('mindora-sidebar-collapsed');
                localStorage.setItem(COLLAPSE_KEY, collapsed ? '1' : '0');
            } catch (e) {
                /* localStorage blocked */
            }
        });
    }

    // ============================================================
    // STATE — MOBILE DRAWER
    // ============================================================
    function bindMobileToggle() {
        const toggleBtn = document.getElementById('sidebarToggleMobile');
        const overlay = document.getElementById('sidebarOverlay');

        if (toggleBtn) {
            toggleBtn.addEventListener('click', openMobileSidebar);
        }

        if (overlay) {
            overlay.addEventListener('click', closeMobileSidebar);
        }

        // Auto-close when clicking a nav link on mobile
        document.querySelectorAll('.mindora-nav-item').forEach(link => {
            link.addEventListener('click', () => {
                if (isMobile()) closeMobileSidebar();
            });
        });
    }

    function openMobileSidebar() {
        document.body.classList.add('mindora-sidebar-open');
        document.getElementById('sidebarOverlay')?.classList.add('show');
        document.body.style.overflow = 'hidden'; // lock scroll
    }

    function closeMobileSidebar() {
        document.body.classList.remove('mindora-sidebar-open');
        document.getElementById('sidebarOverlay')?.classList.remove('show');
        document.body.style.overflow = '';
    }

    // ============================================================
    // ESC KEY — close mobile drawer
    // ============================================================
    function bindEscKey() {
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && document.body.classList.contains('mindora-sidebar-open')) {
                closeMobileSidebar();
            }
        });
    }

    // ============================================================
    // ACTIVE LINK DETECTION
    // ============================================================
    function highlightActiveLink() {
        const currentPath = window.location.pathname.toLowerCase();

        document.querySelectorAll('.mindora-nav-item').forEach(link => {
            const href = link.getAttribute('href');
            if (!href || href === '#') return;

            const linkPath = href.toLowerCase();

            // Exact match or starts-with (excluding root to avoid false positives)
            const isActive =
                currentPath === linkPath ||
                (linkPath.length > 1 && currentPath.startsWith(linkPath));

            if (isActive) {
                link.classList.add('active');
            }
        });
    }

    // ============================================================
    // HELPERS
    // ============================================================
    function isMobile() {
        return window.innerWidth < MOBILE_BREAKPOINT;
    }

    // Auto-close mobile drawer on resize to desktop
    window.addEventListener('resize', () => {
        if (!isMobile() && document.body.classList.contains('mindora-sidebar-open')) {
            closeMobileSidebar();
        }
    });

    // ============================================================
    // PUBLIC API
    // ============================================================
    window.MindoraSidebar = {
        open: openMobileSidebar,
        close: closeMobileSidebar,
        toggle: () => {
            if (isMobile()) {
                document.body.classList.contains('mindora-sidebar-open')
                    ? closeMobileSidebar()
                    : openMobileSidebar();
            } else {
                document.getElementById('sidebarToggle')?.click();
            }
        }
    };
})();