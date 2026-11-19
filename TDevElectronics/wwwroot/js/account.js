/* account.js: Phần việc D (khu tài khoản khách hàng), giai đoạn 1.
 *
 * app.js (từ template) vẫn dựng các tab Overview, Wishlist, Profile và trang Settings.
 * File này chỉ lo phần của trang Account/Orders (hộp thoại hủy đơn có lý do):
 *   1. Chỉ cho mở một hộp "Hủy" cùng lúc.
 *   2. Chọn lý do "Khác" thì ô Chi tiết bắt buộc, tối thiểu 5 ký tự (server kiểm tra lại trong CancelRequestViewModel).
 *   3. Báo lỗi ngay dưới ô, chặn gửi khi chưa hợp lệ, chặn bấm gửi hai lần.
 * Mọi chữ hiển thị đều dùng textContent. Bỏ file này thì form vẫn gửi được, server vẫn kiểm tra.
 */
(function () {
  'use strict';

  var MIN_DETAIL = 5;
  var OTHER = 'Khác';

  function showError(field, msg) {
    var box = field.parentNode.querySelector('.ac-err');
    if (!msg) { if (box) box.remove(); field.removeAttribute('aria-invalid'); return; }
    if (!box) {
      box = document.createElement('small');
      box.className = 'ac-err';
      box.setAttribute('role', 'alert');
      box.style.cssText = 'display:block;color:#ef4444;margin-top:4px';
      field.parentNode.appendChild(box);
    }
    box.textContent = msg;
    field.setAttribute('aria-invalid', 'true');
  }

  function initCancelForms() {
    var boxes = document.querySelectorAll('#acc-orders details');
    if (!boxes.length) return;

    // 1. chỉ mở một hộp hủy cùng lúc
    boxes.forEach(function (d) {
      d.addEventListener('toggle', function () {
        if (!d.open) return;
        boxes.forEach(function (o) { if (o !== d) o.open = false; });
      });
    });

    // 2, 3. kiểm tra từng form
    document.querySelectorAll('#acc-orders form[action$="CancelRequest"]').forEach(function (f) {
      var reason = f.querySelector('[name="Reason"]');
      var detail = f.querySelector('[name="Detail"]');
      var btn = f.querySelector('button[type="submit"]');
      if (!reason || !detail) return;

      function syncRequired() {
        var other = reason.value === OTHER;
        detail.required = other;
        detail.minLength = other ? MIN_DETAIL : 0;
        if (!other) showError(detail, '');
      }
      reason.addEventListener('change', function () { showError(reason, ''); syncRequired(); });
      detail.addEventListener('input', function () { if (detail.value.trim().length >= MIN_DETAIL) showError(detail, ''); });
      syncRequired();

      f.addEventListener('submit', function (e) {
        var ok = true;
        if (!reason.value) { showError(reason, 'Hãy chọn lý do hủy.'); ok = false; }
        if (reason.value === OTHER && detail.value.trim().length < MIN_DETAIL) {
          showError(detail, 'Hãy ghi rõ lý do (ít nhất ' + MIN_DETAIL + ' ký tự).');
          ok = false;
        }
        if (!ok) { e.preventDefault(); return; }
        if (btn) { btn.disabled = true; btn.textContent = 'Đang gửi…'; }
      });
    });
  }

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initCancelForms);
  else initCancelForms();
})();
