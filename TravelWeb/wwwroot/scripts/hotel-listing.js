/* Hotel listing — filters + pending search (common.css / listing-hotels.js patterns) */
(function () {
    const PAGE_SIZE = 6;
    const cfg = window.HotelListingConfig || {};
    const currencySymbol = cfg.currencySymbol || "$";
    const fallbackImg = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80";

    const state = {
        priceMin: Number(cfg.priceMin || 50),
        priceMax: Number(cfg.priceMax || 500),
        stars: new Set(),
        guestRating: 0,
        types: new Set(),
        amenities: new Set(),
        neighborhoods: new Set(),
        sort: "recommended",
        page: 1,
    };

    let defaultPriceMin = state.priceMin;
    let defaultPriceMax = state.priceMax;

    const listEl = document.getElementById("hotel-list");
    const emptyEl = document.getElementById("empty-state");
    const countEl = document.getElementById("results-count");
    const paginationEl = document.getElementById("pagination-row");
    const overlay = document.getElementById("hotel-loading-overlay");
    const resultsArea = document.getElementById("results-area");
    const panels = Array.from(document.querySelectorAll(".filter-panel"));
    let cards = listEl ? Array.from(listEl.querySelectorAll(".hotel-card")) : [];

    function setLoading(on, message) {
        if (overlay) overlay.classList.toggle("is-active", !!on);
        if (resultsArea) resultsArea.classList.toggle("is-loading-blur", !!on);
        const sub = document.getElementById("hotel-loading-sub");
        if (sub && message) sub.textContent = message;
    }

    function currencySym(code) {
        switch ((code || "USD").toUpperCase()) {
            case "INR": return "₹";
            case "EUR": return "€";
            case "GBP": return "£";
            default: return "$";
        }
    }

    function ratingLabel(rating) {
        if (rating >= 4.5) return "Exceptional";
        if (rating >= 4.0) return "Very Good";
        if (rating >= 3.5) return "Good";
        if (rating > 0) return "Fair";
        return "Unrated";
    }

    function escapeHtml(str) {
        return String(str || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }

    function buildCard(h) {
        const sym = currencySym(h.currency);
        const stars = Number(h.stars || 0);
        const rating = Number(h.rating || 0);
        const amenities = Array.isArray(h.amenities) ? h.amenities : [];
        const amenityAttr = amenities.join(",");
        const img = h.picture || fallbackImg;
        const typeLabel = "Hotel";
        const providerLabel = h.provider || "WIKI";
        let starsHtml = "";
        for (let i = 0; i < stars; i++) starsHtml += '<i class="fa-solid fa-star text-[11px]"></i>';
        const amenityHtml = amenities.slice(0, 4).map(function (a) {
            return '<span class="inline-flex items-center gap-1"><i class="fa-solid fa-check text-[10px] text-[var(--green)]"></i>' + escapeHtml(a) + "</span>";
        }).join("");

        const ratingHtml = rating > 0
            ? '<div class="flex items-center gap-2 mt-3"><span class="bg-[var(--orange-600)] text-white text-xs font-extrabold rounded-md px-1.5 py-0.5">' +
              rating.toFixed(1) + '</span><span class="text-xs font-semibold">' + ratingLabel(rating) + "</span></div>"
            : "";

        return (
            '<article class="hotel-card card-hover rounded-2xl border border-[var(--line)] bg-white overflow-hidden flex flex-col sm:flex-row fade-up in"' +
            ' data-name="' + escapeHtml(h.name) + '"' +
            ' data-hotel-code="' + escapeHtml(h.hotelCode || "") + '"' +
            ' data-provider="' + escapeHtml(providerLabel) + '"' +
            ' data-search-id="' + escapeHtml(h.searchId || cfg.searchId || "") + '"' +
            ' data-price="' + Math.round(h.nightly || 0) + '"' +
            ' data-stars="' + stars + '"' +
            ' data-rating="' + rating.toFixed(1) + '"' +
            ' data-type="' + escapeHtml(typeLabel) + '"' +
            ' data-neighborhood="' + escapeHtml(h.address || "") + '"' +
            ' data-amenities="' + escapeHtml(amenityAttr) + '">' +
            '<div class="relative sm:w-72 h-56 sm:h-auto shrink-0 overflow-hidden">' +
            '<img src="' + escapeHtml(img) + '" alt="' + escapeHtml(h.name) + '" class="w-full h-full object-cover" loading="lazy" onerror="this.src=\'' + fallbackImg + '\'">' +
            '<span class="absolute top-3 left-3 bg-white/95 text-xs font-semibold px-2.5 py-1 rounded-full">' + escapeHtml(typeLabel) + "</span></div>" +
            '<div class="flex-1 p-5 flex flex-col sm:flex-row sm:items-center gap-4"><div class="flex-1 min-w-0">' +
            '<div class="flex items-start justify-between gap-3"><div>' +
            '<h3 class="font-bold text-lg leading-snug">' + escapeHtml(h.name) + "</h3>" +
            '<p class="text-xs text-[var(--ink-500)] mt-1 flex items-center gap-1"><i class="fa-solid fa-location-dot text-[10px]"></i><span>' +
            escapeHtml(h.address || "") + "</span></p></div>" +
            (stars > 0 ? '<div class="flex items-center gap-0.5 shrink-0 text-[var(--orange-500)]">' + starsHtml + "</div>" : "") +
            "</div>" + ratingHtml +
            (amenityHtml ? '<div class="flex flex-wrap gap-x-3 gap-y-1 mt-3 text-xs text-[var(--ink-500)]">' + amenityHtml + "</div>" : "") +
            '</div><div class="sm:text-right sm:pl-4 sm:border-l sm:border-[var(--line)] shrink-0 flex sm:flex-col items-center sm:items-end justify-between gap-1"><div>' +
            '<p class="text-[11px] text-[var(--ink-500)]">from</p>' +
            '<p class="font-extrabold text-xl text-[var(--orange-600)]">' + sym + Math.round(h.nightly || 0) + "</p>" +
            '<p class="text-[11px] text-[var(--ink-500)]">per night · total ' + sym + Math.round(h.total || 0) + "</p></div>" +
            '<button type="button" class="hotel-view-deal btn-primary rounded-xl px-5 py-2.5 text-sm font-semibold whitespace-nowrap">View Deal</button>' +
            "</div></div></article>"
        );
    }

    function updateAmenityFilters(hotels) {
        const wrap = document.getElementById("amenity-filters-list");
        if (!wrap) return;
        const set = new Set();
        (hotels || []).forEach(function (h) {
            (h.amenities || []).forEach(function (a) {
                if (a) set.add(a);
            });
        });
        let list = Array.from(set).slice(0, 10);
        if (!list.length) list = ["Free Wi-Fi", "Pool", "Breakfast", "Parking", "Air conditioning"];
        wrap.innerHTML = list.map(function (a) {
            return '<label class="flex items-center gap-2.5 text-sm cursor-pointer"><input type="checkbox" class="filter-checkbox amenity-input" value="' +
                escapeHtml(a) + '"><span>' + escapeHtml(a) + "</span></label>";
        }).join("");
        panels.forEach(function (panel) {
            panel.querySelectorAll(".amenity-input").forEach(function (el) {
                el.addEventListener("change", function () {
                    el.checked ? state.amenities.add(el.value) : state.amenities.delete(el.value);
                    state.page = 1;
                    update();
                });
            });
        });
    }

    function updatePriceBounds(hotels) {
        if (!hotels || !hotels.length) return;
        const prices = hotels.map(function (h) { return Number(h.nightly || 0); }).filter(function (n) { return n > 0; });
        if (!prices.length) return;
        defaultPriceMin = Math.max(10, Math.floor(Math.min.apply(null, prices)));
        defaultPriceMax = Math.max(defaultPriceMin + 10, Math.ceil(Math.max.apply(null, prices)));
        state.priceMin = defaultPriceMin;
        state.priceMax = defaultPriceMax;
        panels.forEach(function (panel) {
            const minRange = panel.querySelector(".price-min-range");
            const maxRange = panel.querySelector(".price-max-range");
            [minRange, maxRange].forEach(function (el) {
                if (!el) return;
                el.min = defaultPriceMin;
                el.max = defaultPriceMax;
            });
            if (minRange) minRange.value = defaultPriceMin;
            if (maxRange) maxRange.value = defaultPriceMax;
        });
    }

    function bindHotels(data) {
        const hotels = (data && (data.hotels || data.Hotels)) || [];
        if (data.searchId || data.SearchId) cfg.searchId = data.searchId || data.SearchId;
        const dest = data.destination || data.Destination || cfg.destination || "Hotels";
        if (listEl) {
            listEl.innerHTML = hotels.map(function (h) {
                // Normalize PascalCase / camelCase from API
                var card = {
                    hotelCode: h.hotelCode || h.HotelCode || "",
                    name: h.name || h.HotelName || h.Name || "",
                    picture: h.picture || h.HotelPicture || h.Picture || "",
                    address: h.address || h.HotelAddress || h.Address || "",
                    stars: h.stars != null ? h.stars : h.Stars,
                    rating: h.rating != null ? h.rating : h.Rating,
                    nightly: h.nightly != null ? h.nightly : h.Nightly,
                    total: h.total != null ? h.total : h.Total,
                    currency: h.currency || h.Currency || "USD",
                    amenities: h.amenities || h.Amenities || [],
                    provider: h.provider || h.Provider || "WIKI",
                    searchId: h.searchId || h.SearchId || cfg.searchId || ""
                };
                return buildCard(card);
            }).join("");
            listEl.classList.toggle("hidden", hotels.length === 0);
            if (hotels.length > 0) {
                listEl.classList.remove("hidden");
                listEl.classList.add("flex");
                listEl.style.display = "flex";
            } else {
                listEl.style.display = "none";
            }
        }
        cards = listEl ? Array.from(listEl.querySelectorAll(".hotel-card")) : [];

        if (emptyEl) {
            emptyEl.classList.toggle("hidden", hotels.length > 0);
        }

        // Update URL with searchId so View Deal / refresh keep context
        if (cfg.searchId && window.history && window.history.replaceState) {
            try {
                var u = new URL(window.location.href);
                u.searchParams.set("searchId", cfg.searchId);
                ["destination", "checkIn", "checkOut", "adults", "children", "rooms"].forEach(function (k) {
                    u.searchParams.delete(k);
                });
                window.history.replaceState({}, "", u.toString());
            } catch (e) { /* ignore */ }
        }

        const heroTitle = document.getElementById("hero-title");
        if (heroTitle) heroTitle.textContent = "Hotels in " + dest;
        const heading = document.getElementById("results-heading");
        if (heading) heading.textContent = "Stays in " + dest;
        const crumb = document.getElementById("breadcrumb-dest");
        if (crumb) crumb.textContent = dest;
        const metaCount = document.querySelector("#meta-count span");
        if (metaCount) metaCount.textContent = hotels.length + " properties";

        updateAmenityFilters(hotels.map(function (h) {
            return {
                amenities: h.amenities || h.Amenities || [],
                nightly: h.nightly != null ? h.nightly : h.Nightly
            };
        }));
        updatePriceBounds(hotels.map(function (h) {
            return { nightly: h.nightly != null ? h.nightly : h.Nightly };
        }));
        update();
    }

    async function runPendingSearch() {
        const place = (document.getElementById("listing-place-input") || {}).value || cfg.destination || "";
        const checkin = (document.getElementById("listing-checkin") || {}).value || cfg.checkIn || "";
        const checkout = (document.getElementById("listing-checkout") || {}).value || cfg.checkOut || "";
        if (!place || !checkin || !checkout) {
            setLoading(false);
            if (countEl) countEl.textContent = "Missing search details.";
            return;
        }

        setLoading(true, "Finding the best stays in " + place + ". Please wait.");
        try {
            const res = await fetch(cfg.searchUrl || "/Hotels/Search", {
                method: "POST",
                headers: { "Content-Type": "application/json", "Accept": "application/json" },
                body: JSON.stringify({
                    destination: place,
                    checkIn: checkin,
                    checkOut: checkout,
                    adults: counts.adults,
                    children: counts.children,
                    rooms: counts.rooms,
                    currency: "USD",
                    guestNationality: "US"
                })
            });
            const data = await res.json();
            const ok = data && (data.success === true || data.Success === true);
            if (!ok) {
                if (emptyEl) {
                    emptyEl.classList.remove("hidden");
                    emptyEl.innerHTML = '<p class="font-bold text-lg mb-2">Search failed</p><p class="text-sm text-[var(--ink-500)]">' +
                        escapeHtml((data && (data.error || data.Error)) || "Please try again.") + "</p>";
                }
                if (countEl) countEl.textContent = "0 stays found";
                if (listEl) {
                    listEl.innerHTML = "";
                    listEl.classList.add("hidden");
                }
                return;
            }
            bindHotels(data);
        } catch (e) {
            if (emptyEl) {
                emptyEl.classList.remove("hidden");
                emptyEl.innerHTML = '<p class="font-bold text-lg mb-2">Search failed</p><p class="text-sm text-[var(--ink-500)]">Please ensure MetaService is running on https://localhost:44393.</p>';
            }
            if (listEl) {
                listEl.innerHTML = "";
                listEl.classList.add("hidden");
            }
        } finally {
            setLoading(false);
        }
    }

    function syncControls() {
        panels.forEach(function (panel) {
            panel.querySelectorAll(".star-input").forEach(function (el) { el.checked = state.stars.has(el.value); });
            panel.querySelectorAll(".type-input").forEach(function (el) { el.checked = state.types.has(el.value); });
            panel.querySelectorAll(".amenity-input").forEach(function (el) { el.checked = state.amenities.has(el.value); });
            panel.querySelectorAll(".neighborhood-input").forEach(function (el) { el.checked = state.neighborhoods.has(el.value); });
            panel.querySelectorAll(".guest-rating-input").forEach(function (el) { el.checked = Number(el.value) === state.guestRating; });
            const minRange = panel.querySelector(".price-min-range");
            const maxRange = panel.querySelector(".price-max-range");
            if (minRange) minRange.value = state.priceMin;
            if (maxRange) maxRange.value = state.priceMax;
            const minLabel = panel.querySelector(".price-min-label");
            const maxLabel = panel.querySelector(".price-max-label");
            if (minLabel) minLabel.textContent = currencySymbol + state.priceMin;
            if (maxLabel) maxLabel.textContent = currencySymbol + state.priceMax;
        });
    }

    function wirePanel(panel) {
        panel.querySelectorAll(".star-input").forEach(function (el) {
            el.addEventListener("change", function () {
                el.checked ? state.stars.add(el.value) : state.stars.delete(el.value);
                state.page = 1;
                update();
            });
        });
        panel.querySelectorAll(".type-input").forEach(function (el) {
            el.addEventListener("change", function () {
                el.checked ? state.types.add(el.value) : state.types.delete(el.value);
                state.page = 1;
                update();
            });
        });
        panel.querySelectorAll(".neighborhood-input").forEach(function (el) {
            el.addEventListener("change", function () {
                el.checked ? state.neighborhoods.add(el.value) : state.neighborhoods.delete(el.value);
                state.page = 1;
                update();
            });
        });
        panel.querySelectorAll(".amenity-input").forEach(function (el) {
            el.addEventListener("change", function () {
                el.checked ? state.amenities.add(el.value) : state.amenities.delete(el.value);
                state.page = 1;
                update();
            });
        });
        panel.querySelectorAll(".guest-rating-input").forEach(function (el) {
            el.addEventListener("change", function () {
                state.guestRating = Number(el.value);
                state.page = 1;
                update();
            });
        });
        const minRange = panel.querySelector(".price-min-range");
        const maxRange = panel.querySelector(".price-max-range");
        if (minRange) {
            minRange.addEventListener("input", function () {
                state.priceMin = Math.min(Number(minRange.value), state.priceMax - 10);
                state.page = 1;
                syncControls();
                renderChips();
                renderList();
            });
        }
        if (maxRange) {
            maxRange.addEventListener("input", function () {
                state.priceMax = Math.max(Number(maxRange.value), state.priceMin + 10);
                state.page = 1;
                syncControls();
                renderChips();
                renderList();
            });
        }
        const resetBtn = panel.querySelector(".reset-filters-btn");
        if (resetBtn) resetBtn.addEventListener("click", resetFilters);
    }

    function resetFilters() {
        state.priceMin = defaultPriceMin;
        state.priceMax = defaultPriceMax;
        state.stars.clear();
        state.guestRating = 0;
        state.types.clear();
        state.amenities.clear();
        state.neighborhoods.clear();
        state.page = 1;
        update();
    }

    function cardData(card) {
        return {
            price: Number(card.dataset.price),
            stars: Number(card.dataset.stars),
            rating: Number(card.dataset.rating),
            type: card.dataset.type || "",
            neighborhood: card.dataset.neighborhood || "",
            amenities: card.dataset.amenities ? card.dataset.amenities.split(",") : [],
        };
    }

    function matchesFilters(card) {
        const d = cardData(card);
        if (d.price < state.priceMin || d.price > state.priceMax) return false;
        if (state.stars.size && !state.stars.has(String(d.stars))) return false;
        if (state.guestRating && d.rating < state.guestRating) return false;
        if (state.types.size && !state.types.has(d.type)) return false;
        if (state.neighborhoods.size) {
            var nMatch = false;
            state.neighborhoods.forEach(function (n) {
                if ((d.neighborhood || "").toLowerCase().indexOf(String(n).toLowerCase()) >= 0) nMatch = true;
            });
            if (!nMatch) return false;
        }
        if (state.amenities.size) {
            for (const a of state.amenities) {
                if (!d.amenities.some(function (x) { return x.toLowerCase() === a.toLowerCase(); })) return false;
            }
        }
        return true;
    }

    function sortCompare(a, b) {
        const da = cardData(a), db = cardData(b);
        switch (state.sort) {
            case "price-asc": return da.price - db.price;
            case "price-desc": return db.price - da.price;
            case "rating-desc": return db.rating - da.rating;
            case "stars-desc": return db.stars - da.stars;
            default: return (db.rating * db.stars) - (da.rating * da.stars);
        }
    }

    function renderList() {
        if (!listEl || !countEl || !emptyEl || !paginationEl) return;
        cards = Array.from(listEl.querySelectorAll(".hotel-card"));
        const matched = cards.filter(matchesFilters);
        const sorted = matched.slice().sort(sortCompare);
        const totalPages = Math.max(1, Math.ceil(sorted.length / PAGE_SIZE));
        state.page = Math.min(Math.max(1, state.page), totalPages);
        const start = (state.page - 1) * PAGE_SIZE;
        const pageCards = sorted.slice(start, start + PAGE_SIZE);
        const visible = new Set(pageCards);

        countEl.textContent = matched.length
            ? matched.length + " stay" + (matched.length > 1 ? "s" : "") + " found"
            : "0 stays found";

        // Keep sort order in the DOM
        sorted.forEach(function (card) { listEl.appendChild(card); });

        cards.forEach(function (card) {
            const isMatch = matched.indexOf(card) >= 0;
            const hideByFilter = !isMatch;
            const hideByPage = isMatch && !visible.has(card);
            card.classList.toggle("is-hidden-by-filter", hideByFilter);
            card.classList.toggle("is-hidden-by-page", hideByPage);
            // Explicit display — avoid Bootstrap/Tailwind !important fights
            card.style.setProperty("display", (hideByFilter || hideByPage) ? "none" : "flex", "important");
        });

        emptyEl.classList.toggle("hidden", matched.length > 0);
        if (matched.length === 0) {
            listEl.classList.add("hidden");
            listEl.style.setProperty("display", "none", "important");
        } else {
            listEl.classList.remove("hidden");
            listEl.classList.add("flex");
            listEl.style.setProperty("display", "flex", "important");
        }
        renderPagination(totalPages);
    }

    function renderPagination(totalPages) {
        paginationEl.innerHTML = "";
        if (totalPages <= 1) {
            paginationEl.classList.add("hidden");
            paginationEl.classList.remove("flex");
            return;
        }
        paginationEl.classList.remove("hidden");
        paginationEl.classList.add("flex");
        function mkBtn(label, page, active, disabled) {
            const btn = document.createElement("button");
            btn.type = "button";
            btn.textContent = label;
            btn.className = active
                ? "btn-primary rounded-lg w-9 h-9 text-sm font-semibold"
                : "btn-ghost rounded-lg w-9 h-9 text-sm font-semibold";
            if (disabled) {
                btn.disabled = true;
                btn.setAttribute("aria-disabled", "true");
            }
            btn.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                if (disabled || page === state.page) return;
                state.page = page;
                renderList();
                var target = document.getElementById("hotel-list") || document.getElementById("results-area");
                if (target) target.scrollIntoView({ behavior: "smooth", block: "start" });
            });
            return btn;
        }
        paginationEl.appendChild(mkBtn("‹", Math.max(1, state.page - 1), false, state.page === 1));
        for (let p = 1; p <= totalPages; p++) paginationEl.appendChild(mkBtn(String(p), p, p === state.page, false));
        paginationEl.appendChild(mkBtn("›", Math.min(totalPages, state.page + 1), false, state.page === totalPages));
    }

    const chipsRow = document.getElementById("chips-row");
    function renderChips() {
        if (!chipsRow) return;
        const chips = [];
        if (state.priceMin > defaultPriceMin || state.priceMax < defaultPriceMax) {
            chips.push({
                label: currencySymbol + state.priceMin + " – " + currencySymbol + state.priceMax,
                clear: function () { state.priceMin = defaultPriceMin; state.priceMax = defaultPriceMax; }
            });
        }
        state.stars.forEach(function (s) {
            chips.push({ label: s + " star", clear: function () { state.stars.delete(s); } });
        });
        if (state.guestRating) {
            chips.push({ label: state.guestRating + "+ rating", clear: function () { state.guestRating = 0; } });
        }
        state.types.forEach(function (t) {
            chips.push({ label: t, clear: function () { state.types.delete(t); } });
        });
        state.neighborhoods.forEach(function (n) {
            chips.push({ label: n, clear: function () { state.neighborhoods.delete(n); } });
        });
        state.amenities.forEach(function (a) {
            chips.push({ label: a, clear: function () { state.amenities.delete(a); } });
        });

        if (!chips.length) {
            chipsRow.classList.add("hidden");
            chipsRow.classList.remove("flex");
            chipsRow.innerHTML = "";
            return;
        }
        chipsRow.classList.remove("hidden");
        chipsRow.classList.add("flex");
        chipsRow.innerHTML = "";
        chips.forEach(function (chip) {
            const el = document.createElement("button");
            el.type = "button";
            el.className = "chip";
            el.innerHTML = "<span>" + chip.label + "</span><i class='fa-solid fa-xmark text-[10px]'></i>";
            el.addEventListener("click", function () {
                chip.clear();
                state.page = 1;
                update();
            });
            chipsRow.appendChild(el);
        });
    }

    function update() {
        syncControls();
        renderChips();
        renderList();
    }

    panels.forEach(wirePanel);

    const sortSelect = document.getElementById("sort-select");
    if (sortSelect) {
        sortSelect.addEventListener("change", function () {
            state.sort = sortSelect.value;
            state.page = 1;
            renderList();
        });
    }

    // Mobile filters drawer (listing-hotels style)
    (function setupMobileFilters() {
        var openBtn = document.getElementById("mobile-filter-open");
        var drawer = document.getElementById("mobile-filter-drawer");
        var closeBtn = document.getElementById("mobile-filter-close");
        var backdrop = document.getElementById("mobile-filter-backdrop");
        var applyBtn = document.getElementById("mobile-filter-apply");
        if (!openBtn || !drawer) return;

        function openDrawer() {
            drawer.classList.add("is-open");
            drawer.classList.remove("hidden");
        }
        function closeDrawer() {
            drawer.classList.remove("is-open");
            drawer.classList.add("hidden");
        }

        openBtn.addEventListener("click", openDrawer);
        if (closeBtn) closeBtn.addEventListener("click", closeDrawer);
        if (backdrop) backdrop.addEventListener("click", closeDrawer);
        if (applyBtn) applyBtn.addEventListener("click", closeDrawer);
        var resetMobile = document.getElementById("mobile-filter-reset");
        if (resetMobile) resetMobile.addEventListener("click", resetFilters);
    })();

    const counts = {
        adults: Number(cfg.adults || 2),
        children: Number(cfg.children || 0),
        rooms: Number(cfg.rooms || 1)
    };
    const limits = { adults: [1, 10], children: [0, 8], rooms: [1, 6] };

    function renderGuestSummary() {
        const summary = document.getElementById("listing-guest-summary");
        const adultsEl = document.getElementById("listing-adults-count");
        const childrenEl = document.getElementById("listing-children-count");
        const roomsEl = document.getElementById("listing-rooms-count");
        if (adultsEl) adultsEl.textContent = counts.adults;
        if (childrenEl) childrenEl.textContent = counts.children;
        if (roomsEl) roomsEl.textContent = counts.rooms;
        document.querySelectorAll("#listing-guest-panel [data-action='dec']").forEach(function (btn) {
            btn.disabled = counts[btn.dataset.target] <= limits[btn.dataset.target][0];
        });
        document.querySelectorAll("#listing-guest-panel [data-action='inc']").forEach(function (btn) {
            btn.disabled = counts[btn.dataset.target] >= limits[btn.dataset.target][1];
        });
        if (!summary) return;
        const childText = counts.children ? (" · " + counts.children + " Child" + (counts.children > 1 ? "ren" : "")) : "";
        summary.textContent = counts.adults + " Adult" + (counts.adults > 1 ? "s" : "") + childText + " · " + counts.rooms + " Room" + (counts.rooms > 1 ? "s" : "");
    }

    function initListingSearchForm() {
        const $ = window.jQuery;
        if ($ && $.fn.datepicker) {
            const $in = $("#listing-checkin");
            const $out = $("#listing-checkout");
            if ($in.length) {
                $in.datepicker({
                    minDate: 0,
                    dateFormat: "mm-dd-yy",
                    onSelect: function (selected) {
                        const dt = new Date(selected);
                        dt.setDate(dt.getDate() + 1);
                        $out.datepicker("option", "minDate", dt);
                        const outVal = $out.datepicker("getDate");
                        if (outVal && outVal <= new Date(selected)) $out.datepicker("setDate", dt);
                    }
                });
            }
            if ($out.length) $out.datepicker({ minDate: 1, dateFormat: "mm-dd-yy" });
        }

        const panel = document.getElementById("listing-guest-panel");
        const toggle = document.getElementById("listing-guest-toggle");
        const done = document.getElementById("listing-guest-done");
        if (toggle && panel) {
            toggle.addEventListener("click", function (e) {
                e.stopPropagation();
                panel.classList.toggle("hidden");
            });
        }
        if (done && panel) done.addEventListener("click", function () { panel.classList.add("hidden"); });
        document.addEventListener("click", function (e) {
            if (!panel || panel.classList.contains("hidden")) return;
            if (!e.target.closest("#listing-guest-panel") && !e.target.closest("#listing-guest-toggle")) {
                panel.classList.add("hidden");
            }
        });

        document.querySelectorAll("#listing-guest-panel .stepper-btn").forEach(function (btn) {
            btn.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                const key = btn.dataset.target;
                const min = limits[key][0];
                const max = limits[key][1];
                if (btn.dataset.action === "inc" && counts[key] < max) counts[key]++;
                if (btn.dataset.action === "dec" && counts[key] > min) counts[key]--;
                renderGuestSummary();
            });
        });
        renderGuestSummary();

        const form = document.getElementById("listing-hotel-search-form");
        if (!form) return;
        form.addEventListener("submit", function (e) {
            e.preventDefault();
            const place = (document.getElementById("listing-place-input").value || "").trim();
            const checkin = document.getElementById("listing-checkin").value;
            const checkout = document.getElementById("listing-checkout").value;
            const formMsg = document.getElementById("listing-form-msg");
            if (!place || !checkin || !checkout) {
                if (formMsg) {
                    formMsg.classList.remove("hidden");
                    formMsg.textContent = "Please choose a destination and both dates to search.";
                }
                return;
            }
            // Re-search: immediate reload of listing shell with loading
            const qs = new URLSearchParams({
                destination: place,
                checkIn: checkin,
                checkOut: checkout,
                adults: String(counts.adults),
                children: String(counts.children),
                rooms: String(counts.rooms)
            });
            window.location.href = (cfg.listingUrl || "/Hotels/Listing") + "?" + qs.toString();
        });
    }

    function openHotelDetails(card, btn) {
        if (!card) return;
        var searchId = card.getAttribute("data-search-id") || cfg.searchId || "";
        if (!searchId) {
            try {
                searchId = new URL(window.location.href).searchParams.get("searchId") || "";
            } catch (e) { /* ignore */ }
        }
        var hotelCode = card.getAttribute("data-hotel-code") || "";
        var provider = card.getAttribute("data-provider") || "WIKI";
        if (!searchId || !hotelCode) {
            alert("Missing search or hotel id. Please run a hotel search again.");
            return;
        }

        var originalText = btn ? btn.textContent : "";
        if (btn) {
            btn.disabled = true;
            btn.textContent = "Loading…";
        }
        setLoading(true, "Loading hotel details…");

        fetch(cfg.detailsUrl || "/Hotels/Details", {
            method: "POST",
            headers: { "Content-Type": "application/json", "Accept": "application/json" },
            body: JSON.stringify({
                searchId: searchId,
                hotelCode: hotelCode,
                provider: provider
            })
        })
            .then(function (r) { return r.json(); })
            .then(function (data) {
                if (!data || !data.success) {
                    throw new Error((data && data.error) || "Unable to load hotel details.");
                }
                window.location.href = data.redirectUrl ||
                    ((cfg.detailsUrl || "/Hotels/Details") + "?searchId=" + encodeURIComponent(searchId) + "&hotelCode=" + encodeURIComponent(hotelCode));
            })
            .catch(function (err) {
                setLoading(false);
                if (btn) {
                    btn.disabled = false;
                    btn.textContent = originalText || "View Deal";
                }
                alert(err.message || "Hotel details failed.");
            });
    }

    function bindViewDealClicks() {
        document.addEventListener("click", function (e) {
            var btn = e.target && e.target.closest ? e.target.closest(".hotel-view-deal") : null;
            if (!btn) return;
            e.preventDefault();
            e.stopPropagation();
            var card = btn.closest(".hotel-card");
            openHotelDetails(card, btn);
        });
    }

    function boot() {
        // Prefer searchId from query string when listing was loaded with results
        try {
            var qsId = new URL(window.location.href).searchParams.get("searchId");
            if (qsId) cfg.searchId = qsId;
        } catch (e) { /* ignore */ }

        bindViewDealClicks();
        initListingSearchForm();
        if (cfg.isPending) {
            runPendingSearch();
        } else {
            update();
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();
