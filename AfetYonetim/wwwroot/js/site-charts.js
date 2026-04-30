// site-charts.js — Chart.js helper'ları (Faz 3)
(function (window) {
    const CATEGORY_COLORS = {
        Gida: '#dc3545',
        Su: '#17a2b8',
        Ilac: '#6f42c1',
        Cadir: '#fd7e14',
        Giysi: '#ffc107',
        Diger: '#6c757d'
    };

    function categoryDoughnut(canvasId, stats) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;
        return new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: stats.map(function (s) { return s.category; }),
                datasets: [{
                    data: stats.map(function (s) { return s.count; }),
                    backgroundColor: stats.map(function (s) { return CATEGORY_COLORS[s.category] || '#888'; })
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { position: 'right' } }
            }
        });
    }

    function dailyTrendLine(canvasId, trend) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;
        return new Chart(ctx, {
            type: 'line',
            data: {
                labels: trend.map(function (t) { return t.label; }),
                datasets: [{
                    label: 'Talep Sayısı',
                    data: trend.map(function (t) { return t.count; }),
                    borderColor: '#007bff',
                    backgroundColor: 'rgba(0,123,255,.15)',
                    fill: true,
                    tension: 0.3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: { y: { beginAtZero: true, ticks: { precision: 0 } } }
            }
        });
    }

    function regionBar(canvasId, stats) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;
        return new Chart(ctx, {
            type: 'bar',
            data: {
                labels: stats.map(function (s) { return s.regionName; }),
                datasets: [{
                    label: 'Talep Sayısı',
                    data: stats.map(function (s) { return s.count; }),
                    backgroundColor: '#007bff'
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: { x: { beginAtZero: true, ticks: { precision: 0 } } }
            }
        });
    }

    window.SiteCharts = {
        categoryDoughnut: categoryDoughnut,
        dailyTrendLine: dailyTrendLine,
        regionBar: regionBar,
        CATEGORY_COLORS: CATEGORY_COLORS
    };
})(window);