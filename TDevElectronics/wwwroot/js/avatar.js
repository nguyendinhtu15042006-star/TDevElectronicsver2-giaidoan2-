/* TDev: ảnh đại diện (khách hàng + admin) — chọn ảnh, kéo vị trí, phóng to, xoay tự do, lật, bo góc tùy ý.
   Đồng bộ với CSDL qua API (AvatarJson) và lưu cache trong localStorage theo từng tài khoản. */
(() => {
    'use strict';
    const $ = (s, r = document) => r.querySelector(s), $$ = (s, r = document) => [...r.querySelectorAll(s)];
    const KEY = 'tdev-avatar:', SEL = '.lay3>.side>.av,#acm .acm-h .av,.atop .ur .av';

    const getMe = () => {
        try {
            if (!window.TDEV_ME) return null;
            return typeof window.TDEV_ME === 'function' ? window.TDEV_ME() : window.TDEV_ME;
        } catch (e) { return null }
    };

    const who = () => {
        const u = getMe();
        return u ? String(u.email || u.name || '').toLowerCase() : '';
    };

    const load = () => {
        const u = getMe();
        const k = who();
        if (!k) return null;
        // Ưu tiên dữ liệu avatar đã lưu từ CSDL nếu có
        if (u && u.avatar) {
            try {
                const dbVal = typeof u.avatar === 'string' ? JSON.parse(u.avatar) : u.avatar;
                if (dbVal && dbVal.img) {
                    try { localStorage.setItem(KEY + k, JSON.stringify(dbVal)) } catch (e) { }
                    return dbVal;
                }
            } catch (e) { }
        }
        try {
            const v = JSON.parse(localStorage.getItem(KEY + k) || 'null');
            return v && v.img ? v : null;
        } catch (e) { return null }
    };

    async function syncToServer(v) {
        const u = getMe();
        if (!u || !u.authenticated) return;
        const isAdmin = String(u.role || '').toLowerCase() === 'admin' || document.body.classList.contains('admin-body') || document.body.classList.contains('adm');
        const url = (u.saveAvatarUrl) || (isAdmin ? '/Admin/Auth/SaveAvatar' : '/Account/SaveAvatar');
        const tokenEl = document.querySelector('#admin-csrf-form input[name="__RequestVerificationToken"], input[name="__RequestVerificationToken"]');
        const token = tokenEl ? tokenEl.value : '';
        const jsonStr = v ? JSON.stringify(v) : null;

        // Cập nhật lại biến trên RAM để không bị load lại bản cũ
        if (typeof window.TDEV_ME === 'object' && window.TDEV_ME !== null) {
            window.TDEV_ME.avatar = jsonStr;
        }

        try {
            await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ avatarJson: jsonStr })
            });
        } catch (e) { }
    }

    const store = v => {
        const k = who();
        if (!k) return false;
        try {
            if (v) localStorage.setItem(KEY + k, JSON.stringify(v));
            else localStorage.removeItem(KEY + k);
            syncToServer(v);
            return true;
        } catch (e) {
            // Trường hợp localStorage đầy vẫn gửi lên server lưu vào DB
            syncToServer(v);
            return true;
        }
    };

    const toast = m => { const t = document.getElementById('ts'); if (!t) return; t.textContent = m; t.classList.add('on'); clearTimeout(toast.t); toast.t = setTimeout(() => t.classList.remove('on'), 2200) };

    /* ---------- hiển thị ảnh ở mọi nơi có avatar của chính mình ---------- */
    function syncHeader(d) {
        const a = $('.hd .ico a[aria-label="Account"]'); if (!a) return;
        const u = getMe();
        let sp = $('.hav', a);
        if (!d || !u || String(u.role || '').toLowerCase() !== 'customer' || document.body.classList.contains('adm')) { a.classList.remove('has-hav'); if (sp) sp.remove(); return }
        if (!sp) { sp = document.createElement('span'); sp.className = 'hav'; sp.setAttribute('data-notr', ''); a.appendChild(sp) }
        a.classList.add('has-hav'); sp.style.backgroundImage = 'url("' + d.img + '")'; sp.style.borderRadius = (d.r == null ? 50 : d.r) + '%';
    }
    function apply() {
        if (!who()) return;
        const d = load();
        $$(SEL).forEach(el => {
            el.classList.add('av-me');
            if (!el.hasAttribute('role')) { el.tabIndex = 0; el.setAttribute('role', 'button'); el.setAttribute('title', 'Change profile photo'); el.setAttribute('aria-label', 'Change profile photo') }
            if (d) { el.classList.add('has-img'); el.style.backgroundImage = 'url("' + d.img + '")'; el.style.borderRadius = (d.r == null ? 50 : d.r) + '%' }
            else { el.classList.remove('has-img'); el.style.backgroundImage = ''; el.style.borderRadius = '' }
        });
        syncHeader(d);
        const am = $('#acm');
        if (am && !$('a[data-avatar]', am)) { const f = $('a', am); if (f) f.insertAdjacentHTML('afterend', '<a href="#" data-avatar><svg class="i"><use href="#i-camera"/></svg>Change photo</a>') }
    }
    let raf = 0; const sched = () => { cancelAnimationFrame(raf); raf = requestAnimationFrame(apply) };
    new MutationObserver(sched).observe(document.documentElement, { childList: true, subtree: true });
    [0, 300, 1000, 2500].forEach(t => setTimeout(apply, t));

    /* ---------- trình chỉnh ảnh ---------- */
    const S = 280, B = 244, E = 256, dpr = Math.min(2, window.devicePixelRatio || 1);
    let st = null, ctx = null, m = null;
    const close = () => { $$('.avm').forEach(x => x.remove()); st = null };
    function rr(c, x, y, w, h, r) { r = Math.min(r, w / 2, h / 2); c.moveTo(x + r, y); c.arcTo(x + w, y, x + w, y + h, r); c.arcTo(x + w, y + h, x, y + h, r); c.arcTo(x, y + h, x, y, r); c.arcTo(x, y, x + w, y, r); c.closePath() }
    function paint(c, size, box, mask) {
        const iw = st.img.naturalWidth || st.img.width, ih = st.img.naturalHeight || st.img.height, base = Math.max(B / iw, B / ih), k = box / B;
        c.clearRect(0, 0, size, size);
        if (mask) { c.fillStyle = '#161F38'; c.fillRect(0, 0, size, size) }
        c.save(); c.translate(size / 2 + st.ox * k, size / 2 + st.oy * k); c.rotate(st.rot * Math.PI / 180); c.scale(st.flip * st.zoom * base * k, st.zoom * base * k); c.drawImage(st.img, -iw / 2, -ih / 2); c.restore();
        if (mask) {
            const x = (size - box) / 2, r = box * st.r / 100;
            c.save(); c.beginPath(); c.rect(0, 0, size, size); rr(c, x, x, box, box, r); c.fillStyle = 'rgba(8,12,24,.66)'; c.fill('evenodd'); c.restore();
            c.beginPath(); rr(c, x, x, box, box, r); c.strokeStyle = 'rgba(255,255,255,.95)'; c.lineWidth = 2; c.stroke()
        }
    }
    const draw = () => { if (!st || !ctx) return; ctx.setTransform(dpr, 0, 0, dpr, 0, 0); paint(ctx, S, B, true) };
    function syncUI() {
        if (!m) return;
        $('[data-f="zoom"]', m).value = Math.round(st.zoom * 100); $('[data-o="zoom"]', m).textContent = Math.round(st.zoom * 100) + '%';
        $('[data-f="rot"]', m).value = Math.round(st.rot); $('[data-o="rot"]', m).textContent = Math.round(st.rot) + '°';
        $('[data-f="r"]', m).value = st.r; $('[data-o="r"]', m).textContent = st.r + '%';
    }
    function err(t) { const e = $('.er', m); e.hidden = !t; e.textContent = t || '' }
    function show(view) { $$('[data-view]', m).forEach(x => x.hidden = x.dataset.view !== view) }
    function loadSrc(src, r) {
        const img = new Image();
        img.onload = () => { st = { img, ox: 0, oy: 0, zoom: 1, rot: 0, flip: 1, r: r == null ? 50 : r }; show('edit'); err(''); syncUI(); draw() };
        img.onerror = () => err('We couldn’t read this image. Try another one.');
        img.src = src;
    }
    function handleFile(f) {
        if (!f) return; err('');
        if (!/^image\//.test(f.type)) { err('Please choose an image file (JPG, PNG or WebP).'); return }
        if (f.size > 8 * 1024 * 1024) { err('This image is larger than 8 MB. Please choose a smaller one.'); return }
        const rd = new FileReader(); rd.onload = () => loadSrc(rd.result, load() ? load().r : 50); rd.onerror = () => err('We couldn’t read this image. Try another one.'); rd.readAsDataURL(f);
    }
    function exportImg() {
        const o = document.createElement('canvas'); o.width = o.height = E;
        paint(o.getContext('2d'), E, E, false);
        let d = o.toDataURL('image/webp', .9); if (d.indexOf('data:image/webp') !== 0) d = o.toDataURL('image/png');
        return d;
    }
    function open() {
        if (!who()) return;
        close();
        const cur = load(), el = $('.av-me'), ini = el ? (el.textContent || '').trim() : '';
        m = document.createElement('div'); m.className = 'mdl avm';
        m.innerHTML = `<div class="cd" role="dialog" aria-modal="true" aria-label="Profile photo">
  <div class="hx"><h2>Profile photo</h2><button class="x2" data-mc type="button" aria-label="Close">✕</button></div>
  <div data-view="pick">
    <div class="avm-cur"${cur ? ` style="background-image:url('${cur.img}');border-radius:${cur.r}%;font-size:0"` : ''}>${esc(ini)}</div>
    <div class="avm-drop" data-av="pick" tabindex="0" role="button"><svg class="i"><use href="#i-camera"/></svg><b>Drag a photo here, or click to choose a file</b><small class="m">JPG, PNG or WebP · up to 8 MB</small></div>
    <input type="file" accept="image/*" hidden>
    <p class="m avm-note">Your photo is synced to your account profile.</p>
    <div class="mf"><button class="bt" type="button" data-av="pick">Choose photo</button>${cur ? '<button class="bo" type="button" data-av="edit-cur">Edit current photo</button><button class="bo rd" type="button" data-av="remove">Remove photo</button>' : ''}</div>
  </div>
  <div data-view="edit" hidden>
    <div class="avm-stage"><canvas width="${S * dpr}" height="${S * dpr}" style="width:${S}px;height:${S}px"></canvas></div>
    <p class="m avm-note">Drag the photo to reposition it. Scroll or use the sliders to zoom and rotate.</p>
    <div class="avm-row"><label>Zoom</label><input type="range" data-f="zoom" min="50" max="400" step="1"><output data-o="zoom"></output></div>
    <div class="avm-row"><label>Rotate</label><input type="range" data-f="rot" min="-180" max="180" step="1"><output data-o="rot"></output></div>
    <div class="avm-row"><label>Corners</label><input type="range" data-f="r" min="0" max="50" step="1"><output data-o="r"></output></div>
    <div class="avm-tools"><button class="bo" type="button" data-av="rot-l" title="Rotate left 90°" aria-label="Rotate left 90°">⟲ 90°</button><button class="bo" type="button" data-av="rot-r" title="Rotate right 90°" aria-label="Rotate right 90°">⟳ 90°</button><button class="bo" type="button" data-av="flip" title="Flip" aria-label="Flip">⇋</button><button class="bo" type="button" data-av="reset">Reset</button></div>
    <div class="mf"><button class="bt" type="button" data-av="save">Save photo</button><button class="bo" type="button" data-av="back">Choose another</button></div>
  </div>
  <p class="er" hidden></p></div>`;
        document.body.appendChild(m);
        ctx = $('canvas', m).getContext('2d');
        const fi = $('input[type=file]', m);
        fi.addEventListener('change', () => { handleFile(fi.files[0]); fi.value = '' });
        const dz = $('.avm-drop', m);
        ['dragenter', 'dragover'].forEach(n => dz.addEventListener(n, e => { e.preventDefault(); dz.classList.add('on') }));
        ['dragleave', 'drop'].forEach(n => dz.addEventListener(n, e => { e.preventDefault(); dz.classList.remove('on') }));
        dz.addEventListener('drop', e => handleFile(e.dataTransfer && e.dataTransfer.files[0]));
        dz.addEventListener('keydown', e => { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); fi.click() } });
        m.addEventListener('click', e => {
            const b = e.target.closest('[data-av]'); if (!b) return;
            const a = b.dataset.av;
            if (a === 'pick') fi.click();
            else if (a === 'edit-cur' && cur) loadSrc(cur.img, cur.r);
            else if (a === 'remove') { store(null); apply(); close(); toast('Profile photo removed') }
            else if (a === 'back') { show('pick'); err('') }
            else if (a === 'reset' && st) { Object.assign(st, { ox: 0, oy: 0, zoom: 1, rot: 0, flip: 1, r: 50 }); syncUI(); draw() }
            else if (a === 'rot-l' && st) { st.rot = wrap(st.rot - 90); syncUI(); draw() }
            else if (a === 'rot-r' && st) { st.rot = wrap(st.rot + 90); syncUI(); draw() }
            else if (a === 'flip' && st) { st.flip *= -1; draw() }
            else if (a === 'save' && st) {
                if (store({ img: exportImg(), r: st.r })) { apply(); close(); toast('Profile photo updated') }
                else err('Couldn’t save the photo.');
            }
        });
        m.addEventListener('input', e => {
            const f = e.target.dataset && e.target.dataset.f; if (!f || !st) return;
            const v = +e.target.value;
            if (f === 'zoom') st.zoom = v / 100; else if (f === 'rot') st.rot = v; else if (f === 'r') st.r = v;
            syncUI(); draw();
        });
        const sg = $('.avm-stage', m); let drag = null;
        sg.addEventListener('pointerdown', e => { if (!st) return; drag = { x: e.clientX, y: e.clientY }; sg.setPointerCapture(e.pointerId) });
        sg.addEventListener('pointermove', e => { if (!drag || !st) return; st.ox += e.clientX - drag.x; st.oy += e.clientY - drag.y; drag = { x: e.clientX, y: e.clientY }; draw() });
        ['pointerup', 'pointercancel'].forEach(n => sg.addEventListener(n, () => { drag = null }));
        sg.addEventListener('wheel', e => { if (!st) return; e.preventDefault(); st.zoom = Math.min(4, Math.max(.5, st.zoom * Math.exp(-e.deltaY * .0015))); syncUI(); draw() }, { passive: false });
    }
    const wrap = a => { a = ((a + 180) % 360 + 360) % 360 - 180; return a };
    const esc = s => String(s).replace(/[&<>"]/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]));

    document.addEventListener('click', e => {
        const l = e.target.closest('a[data-avatar]');
        if (l) { e.preventDefault(); const am = $('#acm'); if (am) am.hidden = true; open(); return }
        const a = e.target.closest('.av-me');
        if (a) { e.preventDefault(); open() }
    });
    document.addEventListener('keydown', e => {
        if ((e.key === 'Enter' || e.key === ' ') && e.target.classList && e.target.classList.contains('av-me')) { e.preventDefault(); open() }
    });
})();