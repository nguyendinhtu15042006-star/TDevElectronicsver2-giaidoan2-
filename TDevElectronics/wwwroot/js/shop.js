/* shop.js (Thành viên B, giai đoạn 1): bộ lọc /Shop, bảng lọc trang danh mục, trang chi tiết sản phẩm.
   Dữ liệu do server dựng sẵn (SampleData.cs); file này chỉ lo tương tác.
   Thẻ sản phẩm (.pc), tim yêu thích (.hr) và nút "Add to Cart" (.bx, .kx) do app.js xử lý. */
(function () {
  'use strict';
  var $ = function (s, r) { return (r || document).querySelector(s); };
  var $$ = function (s, r) { return Array.prototype.slice.call((r || document).querySelectorAll(s)); };
  var money = function (n) { return '$' + n.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); };
  var toast = function (m) {
    if (window.TDEV_API && window.TDEV_API.toast) return window.TDEV_API.toast(m);
    var t = $('#ts'); if (!t) return;
    t.textContent = m; t.classList.add('on'); clearTimeout(toast.t);
    toast.t = setTimeout(function () { t.classList.remove('on'); }, 2200);
  };

  /* ---------------- /Shop: tự gửi form khi đổi bộ lọc ---------------- */
  var shop = $('#shop-page');
  if (shop) {
    var form = $('#shop-filter'), timer;
    var submit = function () { if (form.requestSubmit) form.requestSubmit(); else form.submit(); };
    document.addEventListener('change', function (e) {
      var el = e.target;
      if (!form || !el.form || el.form !== form) return;
      clearTimeout(timer); timer = setTimeout(submit, 120);
    });
    /* URL gọn: bỏ qua ô giá trống và kiểu sắp xếp mặc định khi gửi form */
    form.addEventListener('submit', function () {
      $$('input[name=min], input[name=max]', form).forEach(function (i) { if (!i.value) i.disabled = true; });
      var s = $('select[name=sort]'); if (s && s.value === 'recommended') s.disabled = true;
    });
    window.addEventListener('pageshow', function () {
      $$('input[name=min], input[name=max]', form).forEach(function (i) { i.disabled = false; });
      var s = $('select[name=sort]'); if (s) s.disabled = false;
    });
    /* ô tìm kiếm trên header giữ lại từ khóa đang tìm */
    var hs = $('#hs'), q = shop.getAttribute('data-query');
    if (hs && q && !hs.value) hs.value = q;
  }

  /* ---------------- Trang danh mục: tìm, lọc, sắp xếp ---------------- */
  var cat = $('#cat-page'), panel = $('#cpn');
  if (cat && panel) {
    var low = +panel.getAttribute('data-low'), high = +panel.getAttribute('data-high');
    var items = $$('.kc', cat);
    items.forEach(function (el, i) { el.setAttribute('data-o', i); });
    var st = { q: '', br: {}, ty: '', pr: 0, so: 0 };
    var inPrice = function (v) { return st.pr === 0 || (st.pr === 1 ? v < low : st.pr === 2 ? (v >= low && v < high) : v >= high); };
    var cmp = function (a, b) {
      var pa = +a.dataset.price, pb = +b.dataset.price;
      var d = st.so === 1 ? pa - pb : st.so === 2 ? pb - pa : st.so === 3 ? (+b.dataset.rating) - (+a.dataset.rating) : 0;
      return d || (+a.dataset.o) - (+b.dataset.o);
    };
    var brandsOn = function () { return Object.keys(st.br).filter(function (k) { return st.br[k]; }); };
    var apply = function () {
      var bs = brandsOn(), q = st.q.trim().toLowerCase(), shown = 0;
      /* sắp xếp trong từng nhóm giá */
      $$('.bg4', cat).forEach(function (grid) {
        $$('.kc', grid).sort(cmp).forEach(function (el) { grid.appendChild(el); });
      });
      items.forEach(function (el) {
        var ok = (!bs.length || bs.indexOf(el.dataset.brand) > -1) && (!st.ty || el.dataset.type === st.ty) &&
                 inPrice(+el.dataset.price) && (!q || el.dataset.text.indexOf(q) > -1);
        el.hidden = !ok; if (ok) shown++;
      });
      $$('.band', cat).forEach(function (b) {
        var n = $$('.kc:not([hidden])', b).length, tag = $('.tg.n', b);
        b.hidden = !n; if (tag) tag.textContent = n + (n === 1 ? ' product' : ' products');
      });
      var c = $('.cpn-n', panel), e = $('.cpn-e', panel);
      if (c) c.textContent = shown + (shown === 1 ? ' product' : ' products');
      if (e) e.hidden = shown > 0;
    };
    panel.addEventListener('input', function (e) {
      if (e.target.closest('.cpn-q')) { st.q = e.target.value; apply(); }
    });
    panel.addEventListener('change', function (e) {
      if (e.target.closest('.cpn-so')) { st.so = +e.target.value; apply(); }
    });
    panel.addEventListener('click', function (e) {
      var b = e.target.closest('.cpc');
      if (b) {
        if (b.hasAttribute('data-br')) {
          var k = b.getAttribute('data-br'); st.br[k] = !st.br[k];
          b.classList.toggle('on', st.br[k]); b.setAttribute('aria-pressed', st.br[k]);
        } else if (b.hasAttribute('data-ty')) {
          st.ty = b.getAttribute('data-ty');
          $$('.cpc[data-ty]', panel).forEach(function (x) { var on = x === b; x.classList.toggle('on', on); x.setAttribute('aria-pressed', on); });
        } else if (b.hasAttribute('data-pr')) {
          st.pr = +b.getAttribute('data-pr');
          $$('.cpc[data-pr]', panel).forEach(function (x) { var on = x === b; x.classList.toggle('on', on); x.setAttribute('aria-pressed', on); });
        }
        apply(); return;
      }
      if (e.target.closest('.cpn-r')) {
        st = { q: '', br: {}, ty: '', pr: 0, so: 0 };
        $('.cpn-q input', panel).value = ''; $('.cpn-so select', panel).value = '0';
        $$('.cpc', panel).forEach(function (x) {
          var on = x.hasAttribute('data-pr') ? x.getAttribute('data-pr') === '0' : (x.hasAttribute('data-ty') && x.getAttribute('data-ty') === '');
          x.classList.toggle('on', on); x.setAttribute('aria-pressed', on);
        });
        apply();
      }
    });
    apply();
  }

  /* ---------------- Trang chi tiết sản phẩm ---------------- */
  var prd = $('#prd');
  if (prd) {
    var key = prd.getAttribute('data-k'), name = prd.getAttribute('data-name');
    var tiers = []; try { tiers = JSON.parse(prd.getAttribute('data-tiers') || '[]'); } catch (_) {}
    var qty = 1;

    /* Thư viện ảnh: ảnh lớn đổi theo ảnh nhỏ / nút trước-sau / phím mũi tên */
    var gal = $('#prd-gal'), thumbs = $$('#prd-th .ph'), cap = $('#gcap'), vi = 0;
    var show = function (i) {
      if (!thumbs.length) return;
      vi = (i + thumbs.length) % thumbs.length;
      gal.style.cssText = thumbs[vi].getAttribute('style') || '';
      thumbs.forEach(function (t, j) { t.classList.toggle('on', j === vi); });
      if (cap) cap.textContent = thumbs[vi].getAttribute('data-label') + ' · ' + (vi + 1) + ' of ' + thumbs.length;
    };
    thumbs.forEach(function (t, j) {
      t.addEventListener('click', function () { show(j); });
      t.addEventListener('keydown', function (e) { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); show(j); } });
    });
    $$('[data-gv]', prd).forEach(function (b) { b.addEventListener('click', function () { show(vi + (+b.getAttribute('data-gv'))); }); });
    prd.addEventListener('keydown', function (e) {
      if (!e.target.closest || !e.target.closest('#prd-th, #prd-gal')) return;
      if (e.key === 'ArrowRight') { show(vi + 1); e.preventDefault(); }
      if (e.key === 'ArrowLeft') { show(vi - 1); e.preventDefault(); }
    });

    /* Tab Overview / Specifications / Delivery.
       Lắng nghe ở pha capture và chặn nổi bọt: bộ xử lý ".tabs a" chung của app.js sẽ lọc bảng giá nếu để lọt. */
    var tabs = $('#prd-tabs'), panels = $$('#prd-panels > .cd'), rel = $('#prd-rel');
    var openTab = function (name) {
      $$('a', tabs).forEach(function (a) { a.classList.toggle('on', a.getAttribute('data-tab') === name); });
      var shown = panels.filter(function (p) { return p.getAttribute('data-panel').split(' ').indexOf(name) > -1; });
      panels.forEach(function (p) { var on = shown.indexOf(p) > -1; p.hidden = !on; p.style.gridColumn = on && shown.length === 1 ? '1/-1' : ''; });
      if (rel) rel.style.display = name === 'overview' ? '' : 'none';
    };
    tabs.addEventListener('click', function (e) {
      var a = e.target.closest('a[data-tab]'); if (!a) return;
      e.stopPropagation(); openTab(a.getAttribute('data-tab'));
    }, true);
    openTab('overview');

    /* Số lượng, đơn giá theo mốc, hàng được tô sáng trong bảng giá sỉ */
    var unit = function (q) { var u = tiers.length ? tiers[0][1] : 0; tiers.forEach(function (t) { if (q >= t[0]) u = t[1]; }); return u; };
    var refresh = function () {
      $('#prd-qty').textContent = qty;
      $('#prd-price').textContent = money(unit(qty));
      var h = 0; tiers.forEach(function (t, i) { if (qty >= t[0]) h = i; });
      $$('#prd-tiers tr:not(:first-child)').forEach(function (r, i) { r.classList.toggle('hl', i === h); });
    };
    prd.addEventListener('click', function (e) {
      var q = e.target.closest('[data-qty]');
      if (q) { qty = Math.max(1, Math.min(99, qty + (+q.getAttribute('data-qty')))); refresh(); return; }
      var add = e.target.closest('[data-padd]');
      if (add) {
        var u = window.TDEV_ME && window.TDEV_ME();
        if (u && u.role === 'admin') { toast('Admins can’t place orders. Sign in as a customer to shop.'); return; }
        if (window.TDEV_API && window.TDEV_API.addToCart(key, qty)) toast('Added ' + qty + ' × ' + name + ' to your cart');
      }
    });
    refresh();
  }
})();
