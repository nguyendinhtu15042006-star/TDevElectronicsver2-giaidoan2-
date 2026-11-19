/* cart-checkout.js — Phần việc C (giỏ hàng, thanh toán, đơn hàng), giai đoạn 1.
 *
 * app.js đã dựng giao diện giỏ hàng và checkout bằng localStorage (giống template). File này nối phần giao diện đó
 * với server ASP.NET Core:
 *   1. Đồng bộ giỏ trong localStorage lên Session (POST /Cart/Sync) để server luôn có bản sao giỏ.
 *   2. Khi khách bấm Place Order và app.js hiện xác nhận, gửi đơn lên POST /Checkout/PlaceOrder để server
 *      kiểm tra lại địa chỉ (CountryData), tính lại tiền theo SampleData và lưu đơn để trang Success hiển thị.
 * Không sửa logic của app.js, chỉ lắng nghe sự kiện nên có thể bỏ file này mà giao diện vẫn chạy.
 */
(function () {
  'use strict';

  var CART_KEY = 'tdev-cart';
  var PAGE = document.body ? document.body.dataset.page : '';

  function readCart() {
    try {
      var v = JSON.parse(localStorage.getItem(CART_KEY) || '{}');
      return v && typeof v === 'object' && !Array.isArray(v) ? v : {};
    } catch (e) { return {}; }
  }

  // Token chống giả mạo (Html.AntiForgeryToken trong Checkout/Index.cshtml); các trang khác không có thì không gửi
  function antiToken() {
    var t = document.querySelector('input[name="__RequestVerificationToken"]');
    return t ? t.value : '';
  }

  function post(url, body) {
    var headers = { 'Content-Type': 'application/json' }, tk = antiToken();
    if (tk) headers['RequestVerificationToken'] = tk;
    return fetch(url, {
      method: 'POST',
      headers: headers,
      body: JSON.stringify(body),
      credentials: 'same-origin',
      keepalive: true
    });
  }

  function say(msg) {
    var t = document.getElementById('ts');
    if (!t) return;
    t.textContent = msg;
    t.classList.add('on');
    clearTimeout(say.t);
    say.t = setTimeout(function () { t.classList.remove('on'); }, 3200);
  }

  /* ---------- 1. Đồng bộ giỏ hàng lên server ---------- */
  var lastSent = null, timer = null;

  function pushCart() {
    var cart = readCart(), s = JSON.stringify(cart);
    if (s === lastSent) return;
    lastSent = s;
    post('/Cart/Sync', cart).catch(function () { lastSent = null; });
  }

  function schedulePush() {
    clearTimeout(timer);
    timer = setTimeout(pushCart, 250);
  }

  // app.js lưu giỏ qua localStorage.setItem('tdev-cart', ...): bắt đúng khóa đó để biết giỏ vừa đổi.
  try {
    var origSet = Storage.prototype.setItem;
    Storage.prototype.setItem = function (k) {
      var r = origSet.apply(this, arguments);
      if (this === window.localStorage && k === CART_KEY) schedulePush();
      return r;
    };
  } catch (e) { /* trình duyệt chặn ghi đè: bỏ qua, giỏ vẫn chạy ở phía trình duyệt */ }

  /* ---------- 2. Đặt hàng ---------- */
  var pending = null;

  function val(id) {
    var e = document.getElementById(id);
    return e ? e.value.trim() : '';
  }

  function collect() {
    var pay = document.querySelector('#checkout .pm input:checked');
    var items = [], cart = readCart();
    Object.keys(cart).forEach(function (k) {
      var q = parseInt(cart[k], 10);
      if (q > 0) items.push({ key: k, qty: q });
    });
    return {
      fullName: val('ad-n'), company: val('ad-co'), email: val('ad-e'), phone: val('ad-ph'),
      countryCode: val('ad-cc'), line1: val('ad-l1'), line2: val('ad-l2'), ward: val('ad-w'),
      district: val('ad-d'), city: val('ad-c'), region: val('ad-r'), postalCode: val('ad-p'),
      payment: pay && pay.dataset.m ? pay.dataset.m : 'card',
      items: items
    };
  }

  function firstError(errors) {
    var k = Object.keys(errors || {})[0];
    return k ? errors[k] : 'Please check your details.';
  }

  function submitOrder(snapshot, ref) {
    snapshot.payload.ref = ref;
    post('/Checkout/PlaceOrder', snapshot.payload)
      .then(function (r) { return r.json().then(function (j) { return { ok: r.ok, data: j }; }); })
      .then(function (res) {
        if (res.ok && res.data && res.data.successUrl) {
          lastSent = '{}';
          try { history.replaceState(null, '', res.data.successUrl); } catch (e) {}
        } else {
          say('Order check: ' + firstError(res.data && res.data.errors));
        }
      })
      .catch(function () { /* mất kết nối: đơn vẫn hiển thị ở trình duyệt */ });
  }

  function watchOrder() {
    var place = document.querySelector('#checkout [data-b="place"]');
    var conf = document.querySelector('#checkout [data-b="conf"]');
    if (!place || !conf) return;

    // Chụp dữ liệu ngay lúc bấm vì app.js làm trống giỏ khi đặt xong.
    document.addEventListener('click', function (e) {
      if (e.target.closest && e.target.closest('#checkout [data-b="place"]')) {
        pending = { payload: collect(), before: conf.textContent };
      }
    }, true);

    new MutationObserver(function () {
      if (!pending) return;
      var m = /^(TD-\d+)/.exec(conf.textContent || '');
      if (!m || conf.textContent === pending.before) return;
      var snap = pending;
      pending = null;
      submitOrder(snap, m[1]);
    }).observe(conf, { childList: true, characterData: true, subtree: true });
  }

  function init() {
    pushCart();
    if (PAGE === 'checkout') watchOrder();
  }

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init);
  else init();
})();
