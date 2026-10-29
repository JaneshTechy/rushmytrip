(function () {
    'use strict';

    var $ = function (s, c) { return (c || document).querySelector(s); };
    var $$ = function (s, c) { return Array.prototype.slice.call((c || document).querySelectorAll(s)); };

    /* ---------- Header: solid background after scroll ---------- */
    var header = $('#header');
    function onScroll() { header.classList.toggle('is-scrolled', window.scrollY > 24); }
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });

    /* ---------- Mobile menu ---------- */
    var menuBtn = $('#menuBtn'), menu = $('#mobileMenu');
    function setMenu(open) {
        menu.hidden = !open;
        menuBtn.setAttribute('aria-expanded', String(open));
        menuBtn.setAttribute('aria-label', open ? 'Close menu' : 'Open menu');
        $('[data-icon]', menuBtn).className = open ? 'fa-solid fa-xmark' : 'fa-solid fa-bars';
        header.classList.toggle('is-scrolled', open || window.scrollY > 24);
    }
    menuBtn.addEventListener('click', function () { setMenu(menu.hidden); });
    $$('a', menu).forEach(function (a) { a.addEventListener('click', function () { setMenu(false); }); });
    document.addEventListener('keydown', function (e) { if (e.key === 'Escape') { setMenu(false); closePax(); } });

    /* ---------- Reveal on scroll ---------- */
    var reveals = $$('.reveal');
    if ('IntersectionObserver' in window) {
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (en) {
                if (!en.isIntersecting) return;
                var el = en.target, d = el.getAttribute('data-delay');
                if (d) {
                    el.style.transitionDelay = d + 'ms';
                    setTimeout(function () { el.style.transitionDelay = ''; }, 1000 + Number(d));
                }
                el.classList.add('is-in');
                io.unobserve(el);
            });
        }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });
        reveals.forEach(function (el) { io.observe(el); });
    } else {
        reveals.forEach(function (el) { el.classList.add('is-in'); });
    }

    /* ---------- Search form ---------- */
    var form = $('#searchForm'), from = $('#from'), to = $('#to'),
        fromCode = $('#fromCode'), toCode = $('#toCode'),
        depart = $('#depart'), ret = $('#return'), retField = $('#returnField'),
        err = $('#formError');

    function iso(d) { return d.getFullYear() + '-' + String(d.getMonth() + 1).padStart(2, '0') + '-' + String(d.getDate()).padStart(2, '0'); }
    function addDays(n) { var d = new Date(); d.setDate(d.getDate() + n); return iso(d); }

    var today = addDays(0);
    depart.min = today; ret.min = today;
    depart.value = addDays(14); ret.value = addDays(21);

    depart.addEventListener('change', function () {
        ret.min = depart.value || today;
        if (ret.value && ret.value < ret.min) ret.value = ret.min;
    });

    function isRound() { return $('input[name="trip"]:checked', form).value === 'round'; }
    $$('input[name="trip"]', form).forEach(function (r) {
        r.addEventListener('change', function () {
            var round = isRound();
            ret.disabled = !round;
            retField.classList.toggle('opacity-40', !round);
            retField.classList.toggle('pointer-events-none', !round);
        });
    });

    /* ---------- Airport autocomplete (CityXml) ---------- */
    var citiesUrl = form.getAttribute('data-cities-url');
    var airports = null, byCity = {}, loading = null;

    function txt(node, tag) { var n = node.getElementsByTagName(tag)[0]; return n ? n.textContent.trim() : ''; }

    function loadAirports() {
        if (loading) return loading;
        loading = fetch(citiesUrl)
            .then(function (r) { if (!r.ok) throw new Error('City list not found: ' + citiesUrl); return r.text(); })
            .then(function (xml) {
                var doc = new DOMParser().parseFromString(xml, 'application/xml');
                airports = Array.prototype.map.call(doc.getElementsByTagName('City'), function (c) {
                    var a = {
                        code: txt(c, 'Code').toUpperCase(),
                        name: txt(c, 'Name'),
                        cityCode: txt(c, 'CityCode').toUpperCase(),
                        cityName: txt(c, 'CityName'),
                        stateName: txt(c, 'StateName'),
                        countryName: txt(c, 'CountryName')
                    };
                    a.cityCode = a.cityCode || a.code;
                    a.k = {
                        code: a.code.toLowerCase(),
                        cityCode: a.cityCode.toLowerCase(),
                        city: a.cityName.toLowerCase(),
                        name: a.name.toLowerCase(),
                        country: a.countryName.toLowerCase()
                    };
                    return a;
                });
                byCity = {};
                airports.forEach(function (a) { (byCity[a.cityCode] = byCity[a.cityCode] || []).push(a); });
                return airports;
            })
            .catch(function (e) { console.error(e); loading = null; airports = airports || []; return airports; });
        return loading;
    }

    /* Display formats: change these two functions to change how options look */
    function cityLabel(cc, list) { return cc + ' - ' + list[0].cityName + ' City All Airports, ' + list[0].cityName; }
    function airportLabel(a) { return a.code + ' - ' + a.name + ', ' + a.cityName; }
    function airportSub(a) { return [a.stateName, a.countryName].filter(Boolean).join(', '); }

    function score(a, term) {
        var k = a.k;
        if (k.code === term || k.cityCode === term) return 100;
        if (k.city === term) return 90;
        if (k.city.indexOf(term) === 0) return 70;
        if (k.code.indexOf(term) === 0 || k.cityCode.indexOf(term) === 0) return 60;
        if (k.name.indexOf(term) === 0) return 50;
        if (k.city.indexOf(' ' + term) > -1 || k.name.indexOf(' ' + term) > -1) return 40;
        if (k.name.indexOf(term) > -1) return 30;
        if (k.country.indexOf(term) === 0) return 10;
        return 0;
    }

    function search(raw) {
        var term = raw.trim().toLowerCase();
        if (term.length < 2 || !airports) return [];
        var best = {};
        airports.forEach(function (a) {
            var s = score(a, term);
            if (s && (!best[a.cityCode] || best[a.cityCode].s < s)) best[a.cityCode] = { s: s, hit: a };
        });
        var cities = Object.keys(best)
            .sort(function (x, y) { return best[y].s - best[x].s || x.localeCompare(y); })
            .slice(0, 6);

        var items = [];
        cities.forEach(function (cc) {
            var list = byCity[cc], hit = best[cc].hit;
            if (list.length > 1) {
                items.push({ type: 'city', code: cc, label: cityLabel(cc, list), sub: list[0].countryName });
                list.slice()
                    .sort(function (x, y) { return (y.code === hit.code) - (x.code === hit.code) || x.code.localeCompare(y.code); })
                    .forEach(function (a) { items.push({ type: 'airport', child: true, code: a.code, label: airportLabel(a), sub: airportSub(a) }); });
            } else {
                items.push({ type: 'airport', code: list[0].code, label: airportLabel(list[0]), sub: airportSub(list[0]) });
            }
        });
        return items.slice(0, 15);
    }

    /* Turns "New York (NYC)", "NYC" or "EWR - Newark..." into a proper option */
    function resolve(value) {
        if (!value || !airports) return null;
        var m = /\(([A-Za-z]{3})\)\s*$/.exec(value) || /^\s*([A-Za-z]{3})\s*(-|$)/.exec(value);
        if (!m) return null;
        var code = m[1].toUpperCase();
        if (byCity[code] && byCity[code].length > 1) return { code: code, label: cityLabel(code, byCity[code]) };
        for (var i = 0; i < airports.length; i++) {
            if (airports[i].code === code) return { code: code, label: airportLabel(airports[i]) };
        }
        if (byCity[code]) return { code: byCity[code][0].code, label: airportLabel(byCity[code][0]) };
        return null;
    }

    function cityOf(code) {
        if (byCity[code]) return code;
        for (var i = 0; i < airports.length; i++) if (airports[i].code === code) return airports[i].cityCode;
        return code;
    }

    function setField(input, hidden, raw) {
        var r = resolve(raw);
        input.value = r ? r.label : raw;
        hidden.value = r ? r.code : '';
    }

    function setupAutocomplete(input, hidden) {
        var box = input.closest('label');
        var list = document.createElement('ul');
        list.id = input.id + 'List';
        list.hidden = true;
        list.setAttribute('role', 'listbox');
        list.className = 'absolute left-0 top-[calc(100%+6px)] z-40 max-h-80 w-full min-w-[300px] overflow-y-auto rounded-2xl bg-white p-1.5 text-left shadow-[0_30px_60px_-15px_rgba(10,15,28,.45)] ring-1 ring-slate-200 lg:min-w-[360px]';
        box.appendChild(list);

        input.setAttribute('role', 'combobox');
        input.setAttribute('aria-autocomplete', 'list');
        input.setAttribute('aria-controls', list.id);
        input.setAttribute('aria-expanded', 'false');

        var items = [], active = -1, timer;

        function close() { list.hidden = true; input.setAttribute('aria-expanded', 'false'); input.removeAttribute('aria-activedescendant'); }

        function render() {
            list.innerHTML = '';
            if (!items.length) {
                if (input.value.trim().length < 2) { close(); return; }
                var empty = document.createElement('li');
                empty.className = 'px-4 py-3 text-sm text-slate-500';
                empty.textContent = 'No matching city or airport';
                list.appendChild(empty);
            }
            items.forEach(function (it, i) {
                var li = document.createElement('li');
                li.id = list.id + '-' + i;
                li.setAttribute('role', 'option');
                li.setAttribute('data-i', i);
                li.setAttribute('aria-selected', String(i === active));
                li.className = 'flex cursor-pointer items-start gap-3 rounded-xl px-3 py-2.5 transition ' +
                    (it.child ? 'pl-9 ' : '') + (i === active ? 'bg-brand-50' : 'hover:bg-slate-50');

                var icon = document.createElement('i');
                icon.className = (it.type === 'city' ? 'fa-solid fa-city' : 'fa-solid fa-plane') + ' mt-1 text-xs text-brand-500';
                var wrap = document.createElement('div');
                wrap.className = 'min-w-0';
                var main = document.createElement('p');
                main.className = 'text-sm ' + (it.type === 'city' ? 'font-bold' : 'font-semibold') + ' text-ink-900';
                main.textContent = it.label;
                var sub = document.createElement('p');
                sub.className = 'text-xs text-slate-500';
                sub.textContent = it.sub;

                wrap.appendChild(main);
                if (it.sub) wrap.appendChild(sub);
                li.appendChild(icon);
                li.appendChild(wrap);
                list.appendChild(li);
            });
            list.hidden = false;
            input.setAttribute('aria-expanded', 'true');
            if (active > -1) {
                input.setAttribute('aria-activedescendant', list.id + '-' + active);
                var el = document.getElementById(list.id + '-' + active);
                if (el) el.scrollIntoView({ block: 'nearest' });
            }
        }

        function choose(i) {
            var it = items[i];
            if (!it) return;
            input.value = it.label;
            hidden.value = it.code;
            close();
            hideError();
        }

        input.addEventListener('focus', loadAirports);
        input.addEventListener('input', function () {
            hidden.value = '';
            clearTimeout(timer);
            timer = setTimeout(function () {
                loadAirports().then(function () {
                    items = search(input.value);
                    active = items.length ? 0 : -1;
                    render();
                });
            }, 120);
        });
        input.addEventListener('keydown', function (e) {
            if (list.hidden) return;
            if (e.key === 'ArrowDown') { e.preventDefault(); if (items.length) { active = (active + 1) % items.length; render(); } }
            else if (e.key === 'ArrowUp') { e.preventDefault(); if (items.length) { active = (active - 1 + items.length) % items.length; render(); } }
            else if (e.key === 'Enter') { if (active > -1) { e.preventDefault(); choose(active); } }
            else if (e.key === 'Escape') { close(); }
            else if (e.key === 'Tab') { if (active > -1 && !hidden.value) choose(active); }
        });
        input.addEventListener('blur', close);
        list.addEventListener('mousedown', function (e) {
            e.preventDefault(); /* keep focus in the input so blur doesn't close the list first */
            var li = e.target.closest('[data-i]');
            if (li) choose(Number(li.getAttribute('data-i')));
        });
    }

    setupAutocomplete(from, fromCode);
    setupAutocomplete(to, toCode);

    $('#swapBtn').addEventListener('click', function (e) {
        e.preventDefault();
        var t = from.value; from.value = to.value; to.value = t;
        var c = fromCode.value; fromCode.value = toCode.value; toCode.value = c;
        var icon = $('i', this);
        icon.animate([{ transform: 'rotate(0)' }, { transform: 'rotate(180deg)' }], { duration: 350, easing: 'ease-in-out' });
    });

    /* Click-to-fill: destination cards, route cards and popular chips */
    $$('[data-fill-to]').forEach(function (el) {
        el.addEventListener('click', function (e) {
            e.preventDefault();
            hideError();
            $('#search').scrollIntoView({ behavior: 'smooth', block: 'center' });
            loadAirports().then(function () {
                if (el.hasAttribute('data-fill-from')) setField(from, fromCode, el.getAttribute('data-fill-from'));
                setField(to, toCode, el.getAttribute('data-fill-to'));
            });
            setTimeout(function () { (from.value ? depart : from).focus({ preventScroll: true }); }, 500);
        });
    });

    /* ---------- Travellers popover ---------- */
    var pax = { adults: 1, children: 0, infants: 0 };
    var PAX_ROWS = [
        { key: 'adults', label: 'Adults', hint: '12+ years', min: 1 },
        { key: 'children', label: 'Children', hint: '2 to 11 years', min: 0 },
        { key: 'infants', label: 'Infants', hint: 'Under 2 years', min: 0 }
    ];
    var paxBtn = $('#paxBtn'), paxPanel = $('#paxPanel'), paxRows = $('#paxRows'), cabin = $('#cabin');

    function total() { return pax.adults + pax.children + pax.infants; }
    function paxText() {
        var n = pax.adults + pax.children + pax.infants;
        return n + ' Pax, ' + cabin.value;
    }
    function renderPax() {
        paxRows.innerHTML = PAX_ROWS.map(function (r) {
            return '<div class="flex items-center justify-between gap-3">' +
                '<div><p class="text-sm font-bold text-ink-900">' + r.label + '</p><p class="text-xs text-slate-500">' + r.hint + '</p></div>' +
                '<div class="flex items-center gap-3">' +
                '<button type="button" data-pax="' + r.key + '" data-dir="-1" aria-label="Fewer ' + r.label.toLowerCase() + '" class="flex h-8 w-8 items-center justify-center rounded-full border border-slate-300 text-slate-700 transition hover:border-brand-500 hover:text-brand-600 disabled:cursor-not-allowed disabled:opacity-40"' + (pax[r.key] <= r.min ? ' disabled' : '') + '><i class="fa-solid fa-minus text-xs"></i></button>' +
                '<span class="w-5 text-center text-sm font-bold text-ink-900" aria-live="polite">' + pax[r.key] + '</span>' +
                '<button type="button" data-pax="' + r.key + '" data-dir="1" aria-label="More ' + r.label.toLowerCase() + '" class="flex h-8 w-8 items-center justify-center rounded-full bg-brand-500 text-white transition hover:bg-brand-600 disabled:cursor-not-allowed disabled:opacity-40"' + (canAdd(r.key) ? '' : ' disabled') + '><i class="fa-solid fa-plus text-xs"></i></button>' +
                '</div></div>';
        }).join('');
        $('#paxLabel').textContent = paxText();
    }
    function canAdd(key) {
        if (total() >= 9) return false;
        if (key === 'infants') return pax.infants < pax.adults;
        return true;
    }
    paxRows.addEventListener('click', function (e) {
        e.stopPropagation(); /* rows re-render, so the clicked node is detached before the outside-click check */
        var b = e.target.closest('button[data-pax]');
        if (!b || b.disabled) return;
        var key = b.getAttribute('data-pax'), dir = Number(b.getAttribute('data-dir'));
        if (dir > 0 && !canAdd(key)) return;
        pax[key] += dir;
        if (key === 'adults' && pax.infants > pax.adults) pax.infants = pax.adults;
        renderPax();
    });
    cabin.addEventListener('change', renderPax);
    function openPax() { paxPanel.hidden = false; paxBtn.setAttribute('aria-expanded', 'true'); }
    function closePax() { paxPanel.hidden = true; paxBtn.setAttribute('aria-expanded', 'false'); }
    paxBtn.addEventListener('click', function () { paxPanel.hidden ? openPax() : closePax(); });
    $('#paxDone').addEventListener('click', closePax);
    document.addEventListener('click', function (e) {
        if (!paxPanel.hidden && !paxPanel.contains(e.target) && !paxBtn.contains(e.target)) closePax();
    });
    renderPax();

    /* ---------- Validation + trip summary ---------- */
    function showError(msg) { err.textContent = msg; err.classList.remove('hidden'); }
    function hideError() { err.classList.add('hidden'); err.textContent = ''; }
    function fmt(v) {
        if (!v) return '';
        var p = v.split('-');
        return new Date(Number(p[0]), Number(p[1]) - 1, Number(p[2])).toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric', year: 'numeric' });
    }

    var modal = $('#resultModal');
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        hideError();
        loadAirports().then(function () {
            var f = from.value.trim(), t = to.value.trim();
            if (!f) { showError('Please enter where you are flying from.'); from.focus(); return; }
            if (!fromCode.value) setField(from, fromCode, f);
            if (!fromCode.value) { showError('Please select your departure city or airport from the list.'); from.focus(); return; }
            if (!t) { showError('Please enter where you want to go.'); to.focus(); return; }
            if (!toCode.value) setField(to, toCode, t);
            if (!toCode.value) { showError('Please select your destination city or airport from the list.'); to.focus(); return; }
            if (cityOf(fromCode.value) === cityOf(toCode.value)) { showError('Origin and destination cannot be the same.'); to.focus(); return; }
            if (!depart.value) { showError('Please choose a departure date.'); depart.focus(); return; }
            if (depart.value < today) { showError('Departure date cannot be in the past.'); depart.focus(); return; }
            if (isRound()) {
                if (!ret.value) { showError('Please choose a return date, or switch to one way.'); ret.focus(); return; }
                if (ret.value < depart.value) { showError('Return date must be on or after the departure date.'); ret.focus(); return; }
            }
            goToResults();
        });
    });

    /* Builds the same /result URL the old search engine used */
    var CABIN_CODES = { 'Economy': 'Y', 'Premium Economy': 'P', 'Business': 'B', 'First': 'F' };
    var TRIP_ONEWAY = 'oneway'; /* set this to the value of the old #OneWay radio if it was different */

    function goToResults() {
        var round = isRound();
        var departOn = depart.value;                    /* already yyyy-MM-dd */
        var returnOn = round ? ret.value : '1900-01-01'; /* old code sent 01-01-1900 for one way */

        var currencyEl = $('#currency') || $('select[aria-label="Currency"]');
        var currency = currencyEl ? currencyEl.value : 'USD';

        var utmSource = new URL(window.location.href).searchParams.get('utm_source') || '';

        try {
            localStorage.setItem('moreadultcount', pax.adults);
            localStorage.setItem('morechildcount', pax.children);
        } catch (e) { /* storage blocked, search still works */ }

        var params = [
            ['depart', fromCode.value],
            ['arrival', toCode.value],
            ['trip', round ? 'round' : TRIP_ONEWAY],
            ['departOn[]', departOn],
            ['af', 'bdfqV.kbQjUML5VDRxLrOg--'],
            ['ch', 'RushMyFlight'],
            ['adult', pax.adults],
            ['child', pax.children],
            ['infant', pax.infants],
            ['seniors', 0],
            ['cabin', CABIN_CODES[cabin.value] || 'Y'],
            ['returnOn', returnOn],
            ['utm_source', utmSource],
            ['utm_medium', 'cpc'],
            ['utm_campaign', 'promotional'],
            ['currency', currency]
        ];

        window.location.href = '/result?' + params.map(function (p) {
            return p[0] + '=' + encodeURIComponent(p[1]);
        }).join('&');
    }
    function closeModal() { if (modal.close) modal.close(); else modal.removeAttribute('open'); }
    $('#modalClose').addEventListener('click', closeModal);
    $('#modalEdit').addEventListener('click', closeModal);
    modal.addEventListener('click', function (e) { if (e.target === modal) closeModal(); });

    /* ---------- Route carousel ---------- */
    var track = $('#track');
    function slide(dir) { track.scrollBy({ left: dir * track.clientWidth * 0.85, behavior: 'smooth' }); }
    $('#prevBtn').addEventListener('click', function () { slide(-1); });
    $('#nextBtn').addEventListener('click', function () { slide(1); });

    /* ---------- FAQ: one open at a time ---------- */
    var faqs = $$('details.faq');
    faqs.forEach(function (d) {
        d.addEventListener('toggle', function () {
            if (d.open) faqs.forEach(function (o) { if (o !== d) o.open = false; });
        });
    });

    /* ---------- Footer year ---------- */
    $('#year').textContent = new Date().getFullYear();
})();