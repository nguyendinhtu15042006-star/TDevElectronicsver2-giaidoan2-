/* components.js – thành phần giao diện dùng chung (GĐ1: A).
   Hiện có: thanh trên cùng của admin (.atop) dựng từ một chỗ duy nhất, để mọi trang admin giống nhau.
   Cách dùng ở GĐ2 (_AdminLayout.cshtml):
     <div data-tdev="topbar" data-title="Dashboard"
          data-logout-url="/Admin/Auth/Logout" data-token="@Html.AntiForgeryToken()"></div>
   - data-title      : tiêu đề bên trái thanh
   - data-logout-url : nếu có, nút Sign out gửi POST (kèm token chống giả mạo); không có thì ẩn nút
   - data-token      : chuỗi HTML của @Html.AntiForgeryToken() (tùy chọn)
   Class giữ nguyên để app.js (chuông .bell, nút sáng/tối .tt) và mobile.css (.atop, .ur, .so) nhận ra.
   File nạp TRƯỚC app.js nên app.js thấy thanh đã có sẵn khi khởi động. Trang không có phần tử
   [data-tdev="topbar"] thì file này không làm gì cả. */
(function (g, d) {
  'use strict';

  function esc(s) {
    return String(s == null ? '' : s).replace(/[&<>"']/g, function (c) {
      return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c];
    });
  }
  function icon(id, cls) {
    return '<svg class="' + (cls || 'i') + '"><use href="#i-' + esc(id) + '"/></svg>';
  }

  var TT = '<button class="ib2 tt" type="button" aria-label="Switch light / dark theme" title="Light / dark">' +
    '<svg class="moon" viewBox="0 0 24 24"><path d="M21 12.8A9 9 0 1111.2 3a7 7 0 009.8 9.8z"/></svg>' +
    '<svg class="sun" viewBox="0 0 24 24"><circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"/></svg></button>';

  /* opts: { title, logoutUrl, token } -> phần tử <header class="atop"> */
  function topbar(opts) {
    opts = opts || {};
    var h = d.createElement('header');
    h.className = 'atop';
    var out = opts.logoutUrl
      ? '<form class="so-f" method="post" action="' + esc(opts.logoutUrl) + '" style="display:inline">' + (opts.token || '') +
        '<button class="so bo" type="submit">Sign out</button></form>'
      : '';
    h.innerHTML =
      '<span>' + esc(opts.title || 'Admin') + '</span>' +
      '<div class="ur">' +
        '<button class="ib2 bell" type="button" aria-label="Notifications" aria-expanded="false">' + icon('bell') + '<em></em></button>' +
        TT + out +
      '</div>';
    return h;
  }

  function mount() {
    var slots = d.querySelectorAll('[data-tdev="topbar"]');
    for (var i = 0; i < slots.length; i++) {
      var s = slots[i];
      s.parentNode.replaceChild(topbar({
        title: s.getAttribute('data-title'),
        logoutUrl: s.getAttribute('data-logout-url'),
        token: s.getAttribute('data-token')
      }), s);
    }
  }

  g.TDevComponents = { esc: esc, icon: icon, topbar: topbar, mount: mount };
  mount(); /* script nằm cuối <body>: phần tử đã có sẵn, dựng ngay để app.js thấy */
  if (d.readyState === 'loading') d.addEventListener('DOMContentLoaded', mount);
})(window, document);
