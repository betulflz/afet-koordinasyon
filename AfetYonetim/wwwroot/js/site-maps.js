// site-maps.js — Leaflet helper'ları (Faz 3)
(function (window) {
    const TILE_URL = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
    const TILE_ATTR = '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> katkıda bulunanları';

    const URGENCY_COLORS = { Yuksek: '#dc3545', Orta: '#ffc107', Dusuk: '#28a745' };
    const STATUS_LABELS  = { Bekliyor: 'Bekliyor', Onaylandi: 'Onaylandı', Atandi: 'Atandı', Yolda: 'Yolda', TeslimEdildi: 'Teslim Edildi', Reddedildi: 'Reddedildi' };
    const RISK_COLORS    = { Kritik: '#212529', Yuksek: '#dc3545', Orta: '#ffc107', Dusuk: '#28a745' };
    const RISK_RADIUS    = { Kritik: 22, Yuksek: 18, Orta: 14, Dusuk: 10 };

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text == null ? '' : String(text);
        return div.textContent.replace(/[&<>"']/g, function (ch) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[ch];
        });
    }

    function urgencyIcon(urgency) {
        const color = URGENCY_COLORS[urgency] || '#6c757d';
        return L.divIcon({
            className: 'custom-pin',
            html: '<div style="background:' + color + ';width:22px;height:22px;'
                + 'border-radius:50% 50% 50% 0;transform:rotate(-45deg);'
                + 'border:2px solid #fff;box-shadow:0 2px 4px rgba(0,0,0,.4)"></div>',
            iconSize: [22, 22],
            iconAnchor: [11, 22]
        });
    }

    function initMap(elementId, opts) {
        opts = opts || {};
        const map = L.map(elementId, { scrollWheelZoom: opts.scrollWheelZoom !== false })
                     .setView(opts.center || [37.5, 37.0], opts.zoom || 7);
        L.tileLayer(TILE_URL, { attribution: TILE_ATTR, maxZoom: 18 }).addTo(map);
        return map;
    }

    function buildPopupHtml(pin) {
        const color = URGENCY_COLORS[pin.urgency] || '#6c757d';
        const statusLabel = STATUS_LABELS[pin.status] || pin.status;
        const safeTitle = escapeHtml(pin.title);
        const safeStatus = escapeHtml(statusLabel);
        const safeUrgency = escapeHtml(pin.urgency);
        const safeSubtitle = escapeHtml(pin.subtitle);
        const safeUrl = escapeHtml(pin.detailUrl || '');
        let html = '<div style="min-width:170px">'
                 + '<strong>' + safeTitle + '</strong><br>'
                 + '<span class="badge badge-light">' + safeStatus + '</span> '
                 + '<span class="badge" style="background:' + color + ';color:#fff">' + safeUrgency + '</span><br>'
                 + '<small>' + safeSubtitle + '</small><br>';
        if (safeUrl) {
            html += '<a href="' + safeUrl + '" class="btn btn-link btn-sm p-0">Detayı Gör →</a>';
        }
        return html + '</div>';
    }

    function addPinToMap(map, pin) {
        const marker = L.marker([pin.lat, pin.lng], { icon: urgencyIcon(pin.urgency) });
        marker.bindPopup(buildPopupHtml(pin));
        marker.options.statusKey = pin.status;
        marker.addTo(map);
        return marker;
    }

    function copyCoords(text) {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(text)
                .then(function () { alert('Konum panoya kopyalandı: ' + text); });
        } else {
            alert('Konum: ' + text);
        }
    }

    window.SiteMaps = {
        initMap: initMap,
        addPinToMap: addPinToMap,
        urgencyIcon: urgencyIcon,
        copyCoords: copyCoords,
        URGENCY_COLORS: URGENCY_COLORS,
        RISK_COLORS: RISK_COLORS,
        RISK_RADIUS: RISK_RADIUS,
        STATUS_LABELS: STATUS_LABELS
    };
})(window);