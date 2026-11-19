/* mobile.js – giao diện điện thoại (≤900px). Nạp SAU i18n.js. Chỉ THÊM phần tử, không sửa logic của app.js.
   Làm 5 việc (class do css/mobile.css định nghĩa):
   1. Menu trượt (drawer): nút ☰ .mn-burger, lớp phủ .mn-scrim, đầu menu .mn-navhd, khối tiện ích .mn-tools
   2. Thanh tab đáy .mn-bar: Home · Shop · Quote · Cart · Account (storefront). Admin dùng chính .sb làm thanh tab
   3. Bộ lọc cửa hàng thành bottom-sheet: aside.fs -> .mn-sheet, nút .mn-ftoggle
   4. Bảng ≥4 cột thành thẻ: gắn .mn-card và data-label theo tiêu đề cột
   5. Trang tài khoản: gói các link của .lay3>.side vào dải cuộn ngang .mn-sl
   Phụ thuộc cấu trúc (đừng đổi tên): .hd, .hd .r1, .hd .r1 nav, .hd .ico, .lgt, .cgt, .tt, #wlb,
   a[aria-label=Account], .cb em, .lay > aside.fs, .lay3 > .side, .tw table, .sb, .atop, body.adm */
(function () {
  'use strict';
  var d = document, b = d.body, mq = window.matchMedia ? window.matchMedia('(max-width:900px)') : { matches: false };
  var ADM = b.classList.contains('adm');

  /* ---------- tiện ích ---------- */
  function $(s, r) { return (r || d).querySelector(s); }
  function $$(s, r) { return Array.prototype.slice.call((r || d).querySelectorAll(s)); }
  function el(tag, cls, html) { var e = d.createElement(tag); if (cls) e.className = cls; if (html != null) e.innerHTML = html; return e; }
  function ico(id) { return '<svg class="i"><use href="#i-' + id + '"/></svg>'; }
  function lang() { return (d.documentElement.lang || 'vi').slice(0, 2) === 'vi' ? 'vi' : 'en'; }
  var L = {
    menu: ['Menu', 'Menu'], close: ['Close', 'Đóng'], filters: ['Filters', 'Bộ lọc'], results: ['Show results', 'Xem kết quả'],
    wish: ['Wishlist', 'Yêu thích'], home: ['Home', 'Trang chủ'], shop: ['Shop', 'Cửa hàng'], quote: ['Quote', 'Báo giá'],
    cart: ['Cart', 'Giỏ hàng'], acc: ['Account', 'Tài khoản']
  };
  function t(k) { return L[k][lang() === 'vi' ? 1 : 0]; }
  var X = '<svg viewBox="0 0 24 24"><path d="M6 6l12 12M18 6L6 18"/></svg>';
  var BURGER = '<svg viewBox="0 0 24 24"><path d="M4 7h16M4 12h16M4 17h16"/></svg>';
  var FILTER = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.9" stroke-linecap="round"><path d="M4 6h16M7 12h10M10 18h4"/></svg>';

  /* khóa dịch của riêng mobile.js: nhãn gắn data-notr để i18n.js không dịch trùng, tự cập nhật khi đổi ngôn ngữ */
  var labelFns = [];
  function label(node, key) {
    node.setAttribute('data-notr', '');
    function paint() { node.textContent = t(key); }
    paint(); labelFns.push(paint); return node;
  }
  function aria(node, attr, key) {
    function paint() { node.setAttribute(attr, t(key)); }
    paint(); labelFns.push(paint);
  }
  d.addEventListener('tdev-lang', function () { labelFns.forEach(function (f) { f(); }); relabelTables(); });

  /* ---------- đóng/mở ---------- */
  function setOpen(cls, on) {
    b.classList.toggle(cls, !!on);
    var bg = $('.mn-burger'); if (bg) bg.setAttribute('aria-expanded', String(b.classList.contains('mn-open')));
  }
  function closeAll() { setOpen('mn-open', false); setOpen('mn-fopen', false); }
  d.addEventListener('keydown', function (e) { if (e.key === 'Escape') closeAll(); });

  /* =====================================================================
     1. STOREFRONT: menu trượt + tiện ích + thanh tab đáy
     ===================================================================== */
  var moved = [];   // {node, parent, next} để trả về chỗ cũ khi quay lại desktop
  function placeTools(mobile) {
    var tools = $('.mn-tools'); if (!tools) return;
    if (mobile) {
      if (moved.length) return;
      $$('.hd .lgt, .hd .cgt, .hd .tt').forEach(function (n) {
        if (n.closest('.mn-tools')) return;
        moved.push({ node: n, parent: n.parentNode, next: n.nextSibling });
        tools.insertBefore(n, $('.mn-wl', tools));
      });
    } else {
      while (moved.length) {
        var m = moved.pop();
        if (m.next && m.next.parentNode === m.parent) m.parent.insertBefore(m.node, m.next); else m.parent.appendChild(m.node);
      }
    }
  }

  function buildStorefront() {
    var hd = $('.hd'), r1 = hd && $('.r1', hd), nav = r1 && $('nav', r1);
    if (!hd || !r1 || !nav || $('.mn-burger')) return;

    var burger = el('button', 'mn-burger', BURGER);
    burger.type = 'button'; burger.setAttribute('aria-expanded', 'false'); aria(burger, 'aria-label', 'menu');
    r1.insertBefore(burger, r1.firstChild);

    var scrim = el('div', 'mn-scrim'); scrim.setAttribute('aria-hidden', 'true'); b.appendChild(scrim);

    var logo = $('.lg', r1), home = logo ? logo.getAttribute('href') || '/' : '/';
    var navhd = el('div', 'mn-navhd', '<a class="lg" href="' + home + '"><b>T</b>TDev</a>');
    var x = el('button', 'mn-x', X); x.type = 'button'; aria(x, 'aria-label', 'close');
    navhd.appendChild(x); nav.insertBefore(navhd, nav.firstChild);

    var wl = $('#wlb', hd), tools = el('div', 'mn-tools');
    var wla = el('a', 'mn-wl', ico('heart')); wla.href = wl ? wl.getAttribute('href') || '#' : '#';
    if (wl && wl.getAttribute('data-view')) wla.setAttribute('data-view', wl.getAttribute('data-view'));
    wla.appendChild(label(el('span'), 'wish')); tools.appendChild(wla); nav.appendChild(tools);

    burger.addEventListener('click', function () { setOpen('mn-fopen', false); setOpen('mn-open', !b.classList.contains('mn-open')); });
    x.addEventListener('click', closeAll);
    scrim.addEventListener('click', closeAll);
    nav.addEventListener('click', function (e) { if (e.target.closest('a[href]') && !e.target.closest('.mn-tools')) closeAll(); });

    buildTabBar();
    placeTools(mq.matches);
  }

  /* thanh tab đáy */
  function hrefOf(sel, fallback) { var a = $(sel); var h = a && a.getAttribute('href'); return h && h !== '#' ? h : fallback; }
  function activeTab() {
    var pg = (b.getAttribute('data-page') || '').toLowerCase(), p = location.pathname.toLowerCase();
    if (pg === 'home' || p === '/' || p === '/home' || p === '/home/index') return 'home';
    if (/^(shop|product|cat)/.test(pg) || /^\/(shop|product)/.test(p)) return 'shop';
    if (/^(rfq|quote|business)/.test(pg) || /^\/quotes/.test(p) || /businessbuying/.test(p)) return 'quote';
    if (/^(cart|checkout)/.test(pg) || /^\/(cart|checkout)/.test(p)) return 'cart';
    if (/^(account|login|register)/.test(pg) || /^\/account/.test(p)) return 'acc';
    return '';
  }
  function buildTabBar() {
    if ($('.mn-bar')) return;
    var items = [
      ['home', 'home', hrefOf('.hd .r1 .lg', '/')],
      ['shop', 'bag', hrefOf('.hd .r1 nav>a', '/Shop')],
      ['quote', 'file', hrefOf('.hd .r1 nav>a[href*="Business"], .hd .r1 nav>a[href*="Rfq"], .hd .r1 nav>a[href*="Quote"]', '/Home/BusinessBuying')],
      ['cart', 'cart', hrefOf('.hd a.cb', '/Cart')],
      ['acc', 'user', hrefOf('.hd a[aria-label="Account"]', '/Account')]
    ], bar = el('nav', 'mn-bar'), on = activeTab();
    bar.setAttribute('aria-label', 'Tabs');
    items.forEach(function (it) {
      var a = el('a', it[0] === on ? 'on' : '', ico(it[1]));
      a.href = it[2]; if (it[0] === on) a.setAttribute('aria-current', 'page');
      a.appendChild(label(el('span'), it[0]));
      if (it[0] === 'cart') a.appendChild(el('em'));
      bar.appendChild(a);
    });
    b.appendChild(bar);
    syncBadge();
  }
  /* số trên tab Giỏ hàng = số trên icon giỏ ở header (app.js cập nhật .cb em) */
  function syncBadge() {
    var src = $('.hd .cb em'), dst = $('.mn-bar em'); if (!dst) return;
    function copy() { var n = src ? (src.textContent || '').trim() : ''; dst.textContent = n === '0' ? '' : n; dst.setAttribute('data-n', n || '0'); }
    copy();
    if (src && window.MutationObserver) new MutationObserver(copy).observe(src, { childList: true, characterData: true, subtree: true });
  }

  /* =====================================================================
     2. ADMIN: .sb thành thanh tab đáy (CSS lo), JS chỉ đánh dấu mục đang mở
     ===================================================================== */
  function buildAdmin() {
    var sb = $('.sb'); if (!sb) return;
    var p = location.pathname.toLowerCase(), best = null, bl = 0;
    $$('a:not(.lg)', sb).forEach(function (a) {
      var h = (a.getAttribute('href') || '').split('?')[0].toLowerCase();
      if (h && h !== '#' && p.indexOf(h) === 0 && h.length > bl) { best = a; bl = h.length; }
    });
    if (best && !$('a.on', sb)) best.classList.add('on');
    var on = $('a.on', sb);
    if (on && on.scrollIntoView && mq.matches) { try { sb.scrollLeft = on.offsetLeft - (sb.clientWidth - on.offsetWidth) / 2; } catch (e) {} }
  }

  /* =====================================================================
     3. BỘ LỌC CỬA HÀNG -> BOTTOM SHEET
     ===================================================================== */
  function buildFilter() {
    var fs = $('.lay>aside.fs'); if (!fs || fs.classList.contains('mn-sheet')) return;
    fs.classList.add('mn-sheet');
    var hd = el('div', 'mn-sheethd', '<b></b>'); label($('b', hd), 'filters');
    var x = el('button', 'mn-x', X); x.type = 'button'; aria(x, 'aria-label', 'close'); hd.appendChild(x);
    fs.insertBefore(hd, fs.firstChild);
    var ft = el('div', 'mn-sheetft'), ok = el('button', 'bt'); ok.type = 'button'; label(ok, 'results'); ft.appendChild(ok); fs.appendChild(ft);

    var lay = fs.parentNode, tg = el('button', 'mn-ftoggle', FILTER); tg.type = 'button';
    tg.appendChild(label(el('span'), 'filters')); tg.appendChild(el('em'));
    lay.parentNode.insertBefore(tg, lay);

    function count() {
      var n = 0;
      $$('input', fs).forEach(function (i) {
        if (i.type === 'checkbox' || i.type === 'radio') { if (i.checked !== i.defaultChecked) n++; else if (i.checked && i.type === 'checkbox') n++; }
        else if (i.type !== 'hidden' && i.type !== 'button' && i.type !== 'submit' && i.value && i.value !== i.defaultValue) n++;
      });
      $$('select', fs).forEach(function (s) { if (s.selectedIndex > 0) n++; });
      var em = $('em', tg); em.textContent = n || ''; em.setAttribute('data-n', n);
    }
    count();
    fs.addEventListener('change', count); fs.addEventListener('input', count);
    tg.addEventListener('click', function () { setOpen('mn-open', false); setOpen('mn-fopen', true); });
    x.addEventListener('click', closeAll); ok.addEventListener('click', closeAll);
    $('.mn-scrim') || b.appendChild(el('div', 'mn-scrim'));
    $('.mn-scrim').addEventListener('click', closeAll);
  }

  /* =====================================================================
     4. BẢNG -> THẺ
     ===================================================================== */
  var tables = [];
  function headCells(tb) {
    var tr = $('thead tr', tb) || $('tr', tb);
    return tr ? $$('th,td', tr) : [];
  }
  function labelTable(tb) {
    var hc = headCells(tb), n = hc.length, rows = $$('tr', tb), htr = hc.length ? hc[0].parentNode : null;
    if (n < 4) { tb.classList.remove('mn-card'); var w0 = tb.closest('.tw'); if (w0) w0.classList.remove('mn-card'); return; }
    tb.classList.add('mn-card'); var w = tb.closest('.tw'); if (w) w.classList.add('mn-card');
    var names = hc.map(function (h) { return (h.getAttribute('data-label') || h.textContent || '').replace(/\s+/g, ' ').trim(); });
    rows.forEach(function (tr) {
      if (tr === htr && ($('thead', tb) || $('th', tr))) { tr.classList.add('mn-h'); return; }
      var tds = $$(':scope>td', tr);
      tds.forEach(function (td, i) {
        td.setAttribute('data-label', names[i] || '');
        if (i === tds.length - 1 && $('a,button', td) && td.textContent.replace(/\s+/g, '').length <= 24) td.classList.add('act');
      });
    });
  }
  function scanTables() {
    $$('.tw table').forEach(function (tb) { if (tables.indexOf(tb) < 0) tables.push(tb); labelTable(tb); });
  }
  function relabelTables() { tables = tables.filter(function (t) { return d.contains(t); }); tables.forEach(labelTable); }

  /* =====================================================================
     5. TÀI KHOẢN: gói link của .side vào .mn-sl (chỉ khi ở điện thoại)
     ===================================================================== */
  function accountStrip(mobile) {
    $$('.lay3>.side').forEach(function (side) {
      var strip = $('.mn-sl', side);
      if (mobile) {
        var links = $$(':scope>a', side);
        if (!links.length) return;
        if (!strip) { strip = el('div', 'mn-sl'); side.appendChild(strip); }
        links.forEach(function (a) { strip.appendChild(a); });
      } else if (strip) {
        while (strip.firstChild) side.insertBefore(strip.firstChild, strip);
        side.removeChild(strip);
      }
    });
  }

  /* ---------- chạy ---------- */
  function onMode() {
    var m = !!mq.matches;
    if (!ADM) placeTools(m);
    accountStrip(m);
    if (!m) closeAll();
    if (ADM) buildAdmin();
  }
  function init() {
    if (!ADM) buildStorefront();
    buildFilter(); scanTables(); accountStrip(!!mq.matches); if (ADM) buildAdmin();
    if (mq.addEventListener) mq.addEventListener('change', onMode); else if (mq.addListener) mq.addListener(onMode);
    /* nội dung do JS vẽ sau (giỏ hàng, đơn hàng, bảng admin...): quét lại, gom nhịp để khỏi chạy liên tục */
    if (window.MutationObserver) {
      var tm = 0;
      new MutationObserver(function (list) {
        if (list.some(function (r) { return r.target.closest && r.target.closest('.mn-bar,.mn-navhd,.mn-tools,.mn-sheethd'); })) return;
        clearTimeout(tm);
        tm = setTimeout(function () { buildFilter(); scanTables(); accountStrip(!!mq.matches); }, 80);
      }).observe($('main') || b, { childList: true, subtree: true });
    }
  }
  if (d.readyState === 'loading') d.addEventListener('DOMContentLoaded', init); else init();
})();
