// ============================================================
// Mindora — Chart.js Shared Helpers
// Reusable chart creation functions for all modules
// ============================================================
(function () {
    'use strict';

    // Brand colors
    const COLORS = {
        primary: '#06B6D4',
        primaryDark: '#0891B2',
        purple: '#8B5CF6',
        orange: '#F59E0B',
        green: '#10B981',
        pink: '#EC4899',
        red: '#EF4444',
        blue: '#3B82F6',
        gray: '#94A3B8'
    };

    const DEFAULT_OPTIONS = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                display: false
            },
            tooltip: {
                backgroundColor: 'rgba(15, 23, 42, 0.95)',
                titleColor: '#F1F5F9',
                bodyColor: '#CBD5E1',
                padding: 12,
                cornerRadius: 8,
                displayColors: false
            }
        },
        scales: {
            x: {
                grid: {
                    display: false
                },
                ticks: {
                    color: '#94A3B8',
                    font: { size: 11 }
                }
            },
            y: {
                beginAtZero: true,
                grid: {
                    color: 'rgba(148, 163, 184, 0.1)'
                },
                ticks: {
                    color: '#94A3B8',
                    font: { size: 11 }
                }
            }
        }
    };

    // ============================================================
    // LINE CHART
    // ============================================================
    function createLineChart(canvasId, labels, datasets, options = {}) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.warn(`[Charts] Canvas #${canvasId} not found`);
            return null;
        }

        const ctx = canvas.getContext('2d');

        const enhancedDatasets = datasets.map((ds, i) => {
            const color = ds.color || COLORS.primary;
            return {
                label: ds.label || `Series ${i + 1}`,
                data: ds.data || [],
                borderColor: color,
                backgroundColor: ds.fill
                    ? createGradient(ctx, color)
                    : 'transparent',
                borderWidth: 2.5,
                fill: ds.fill || false,
                tension: 0.4,
                pointRadius: 3,
                pointHoverRadius: 6,
                pointBackgroundColor: color,
                pointBorderColor: '#FFFFFF',
                pointBorderWidth: 2
            };
        });

        return new Chart(ctx, {
            type: 'line',
            data: { labels, datasets: enhancedDatasets },
            options: mergeOptions(DEFAULT_OPTIONS, options)
        });
    }

    // ============================================================
    // BAR CHART
    // ============================================================
    function createBarChart(canvasId, labels, datasets, options = {}) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.warn(`[Charts] Canvas #${canvasId} not found`);
            return null;
        }

        const ctx = canvas.getContext('2d');

        const enhancedDatasets = datasets.map((ds, i) => {
            const color = ds.color || COLORS.primary;
            return {
                label: ds.label || `Series ${i + 1}`,
                data: ds.data || [],
                backgroundColor: ds.colors || color,
                borderRadius: 6,
                borderSkipped: false,
                maxBarThickness: 40
            };
        });

        return new Chart(ctx, {
            type: 'bar',
            data: { labels, datasets: enhancedDatasets },
            options: mergeOptions(DEFAULT_OPTIONS, options)
        });
    }

    // ============================================================
    // DOUGHNUT CHART
    // ============================================================
    function createDoughnutChart(canvasId, labels, data, colors, options = {}) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.warn(`[Charts] Canvas #${canvasId} not found`);
            return null;
        }

        const ctx = canvas.getContext('2d');

        return new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels,
                datasets: [{
                    data,
                    backgroundColor: colors || [
                        COLORS.primary,
                        COLORS.purple,
                        COLORS.orange,
                        COLORS.green,
                        COLORS.pink,
                        COLORS.blue,
                        COLORS.red
                    ],
                    borderWidth: 0,
                    hoverOffset: 8
                }]
            },
            options: mergeOptions({
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                plugins: {
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            padding: 16,
                            usePointStyle: true,
                            color: '#64748B',
                            font: { size: 12 }
                        }
                    },
                    tooltip: DEFAULT_OPTIONS.plugins.tooltip
                }
            }, options)
        });
    }

    // ============================================================
    // RADAR CHART
    // ============================================================
    function createRadarChart(canvasId, labels, data, options = {}) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.warn(`[Charts] Canvas #${canvasId} not found`);
            return null;
        }

        const ctx = canvas.getContext('2d');

        return new Chart(ctx, {
            type: 'radar',
            data: {
                labels,
                datasets: [{
                    label: 'Score',
                    data,
                    backgroundColor: 'rgba(6, 182, 212, 0.2)',
                    borderColor: COLORS.primary,
                    borderWidth: 2,
                    pointBackgroundColor: COLORS.primary,
                    pointBorderColor: '#FFFFFF',
                    pointBorderWidth: 2,
                    pointRadius: 4
                }]
            },
            options: mergeOptions({
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: DEFAULT_OPTIONS.plugins.tooltip
                },
                scales: {
                    r: {
                        beginAtZero: true,
                        grid: { color: 'rgba(148, 163, 184, 0.15)' },
                        angleLines: { color: 'rgba(148, 163, 184, 0.15)' },
                        pointLabels: {
                            color: '#64748B',
                            font: { size: 11 }
                        },
                        ticks: {
                            display: false
                        }
                    }
                }
            }, options)
        });
    }

    // ============================================================
    // HELPERS
    // ============================================================
    function createGradient(ctx, color) {
        const gradient = ctx.createLinearGradient(0, 0, 0, 300);
        gradient.addColorStop(0, hexToRgba(color, 0.25));
        gradient.addColorStop(1, hexToRgba(color, 0));
        return gradient;
    }

    function hexToRgba(hex, alpha) {
        const r = parseInt(hex.slice(1, 3), 16);
        const g = parseInt(hex.slice(3, 5), 16);
        const b = parseInt(hex.slice(5, 7), 16);
        return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    }

    function mergeOptions(base, override) {
        const result = { ...base };
        for (const key in override) {
            if (typeof override[key] === 'object' && !Array.isArray(override[key]) && override[key] !== null) {
                result[key] = mergeOptions(base[key] || {}, override[key]);
            } else {
                result[key] = override[key];
            }
        }
        return result;
    }

    // ============================================================
    // PUBLIC API
    // ============================================================
    window.MindoraCharts = {
        COLORS,
        line: createLineChart,
        bar: createBarChart,
        doughnut: createDoughnutChart,
        radar: createRadarChart,
        hexToRgba
    };
})();