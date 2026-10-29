/* =====================================================================
   Hotel Search Results — static markup edition
   All hotel cards and filter controls already exist in the HTML.
   This script only reads/writes state, toggles visibility, reorders
   the DOM, and keeps the desktop + mobile filter panels in sync.
   ===================================================================== */

const PAGE_SIZE = 6;

const state = {
    priceMin: 50,
    priceMax: 500,
    stars: new Set(),
    guestRating: 0,
    types: new Set(),
    amenities: new Set(),
    neighborhoods: new Set(),
    sort: "recommended",
    page: 1,
};

const listEl = document.getElementById("hotel-list");
const cards = Array.from(listEl.querySelectorAll(".hotel-card"));
const emptyEl = document.getElementById("empty-state");
const countEl = document.getElementById("results-count");
const paginationEl = document.getElementById("pagination-row");
const panels = Array.from(document.querySelectorAll(".filter-panel"));

/* ---------- Sync every control (desktop + mobile) to current state ---------- */
function syncControls() {
    panels.forEach((panel) => {
        panel.querySelectorAll(".star-input").forEach((el) => { el.checked = state.stars.has(el.value); });
        panel.querySelectorAll(".type-input").forEach((el) => { el.checked = state.types.has(el.value); });
        panel.querySelectorAll(".amenity-input").forEach((el) => { el.checked = state.amenities.has(el.value); });
        panel.querySelectorAll(".neighborhood-input").forEach((el) => { el.checked = state.neighborhoods.has(el.value); });
        panel.querySelectorAll(".guest-rating-input").forEach((el) => { el.checked = Number(el.value) === state.guestRating; });

        const minRange = panel.querySelector(".price-min-range");
        const maxRange = panel.querySelector(".price-max-range");
        if (minRange) minRange.value = state.priceMin;
        if (maxRange) maxRange.value = state.priceMax;
        const minLabel = panel.querySelector(".price-min-label");
        const maxLabel = panel.querySelector(".price-max-label");
        if (minLabel) minLabel.textContent = "$" + state.priceMin;
        if (maxLabel) maxLabel.textContent = state.priceMax >= 500 ? "$500+" : "$" + state.priceMax;
    });
}

/* ---------- Wire events on both filter panels ---------- */
function wirePanel(panel) {
    panel.querySelectorAll(".star-input").forEach((el) => {
        el.addEventListener("change", () => {
            el.checked ? state.stars.add(el.value) : state.stars.delete(el.value);
            state.page = 1;
            update();
        });
    });
    panel.querySelectorAll(".type-input").forEach((el) => {
        el.addEventListener("change", () => {
            el.checked ? state.types.add(el.value) : state.types.delete(el.value);
            state.page = 1;
            update();
        });
    });
    panel.querySelectorAll(".amenity-input").forEach((el) => {
        el.addEventListener("change", () => {
            el.checked ? state.amenities.add(el.value) : state.amenities.delete(el.value);
            state.page = 1;
            update();
        });
    });
    panel.querySelectorAll(".neighborhood-input").forEach((el) => {
        el.addEventListener("change", () => {
            el.checked ? state.neighborhoods.add(el.value) : state.neighborhoods.delete(el.value);
            state.page = 1;
            update();
        });
    });
    panel.querySelectorAll(".guest-rating-input").forEach((el) => {
        el.addEventListener("change", () => {
            state.guestRating = Number(el.value);
            state.page = 1;
            update();
        });
    });

    const minRange = panel.querySelector(".price-min-range");
    const maxRange = panel.querySelector(".price-max-range");
    if (minRange) {
        minRange.addEventListener("input", () => {
            state.priceMin = Math.min(Number(minRange.value), state.priceMax - 10);
            state.page = 1;
            syncControls();
            renderChips();
            renderList();
        });
    }
    if (maxRange) {
        maxRange.addEventListener("input", () => {
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
panels.forEach(wirePanel);

function resetFilters() {
    state.priceMin = 50;
    state.priceMax = 500;
    state.stars.clear();
    state.guestRating = 0;
    state.types.clear();
    state.amenities.clear();
    state.neighborhoods.clear();
    state.page = 1;
    update();
}

/* ---------- Read a card's data-* attributes ---------- */
function cardData(card) {
    return {
        price: Number(card.dataset.price),
        stars: Number(card.dataset.stars),
        rating: Number(card.dataset.rating),
        type: card.dataset.type,
        neighborhood: card.dataset.neighborhood,
        amenities: card.dataset.amenities ? card.dataset.amenities.split(",") : [],
    };
}

function matchesFilters(card) {
    const d = cardData(card);
    if (d.price < state.priceMin || d.price > state.priceMax) return false;
    if (state.stars.size && !state.stars.has(String(d.stars))) return false;
    if (state.guestRating && d.rating < state.guestRating) return false;
    if (state.types.size && !state.types.has(d.type)) return false;
    if (state.neighborhoods.size && !state.neighborhoods.has(d.neighborhood)) return false;
    if (state.amenities.size) {
        for (const a of state.amenities) if (!d.amenities.includes(a)) return false;
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

/* ---------- Filter, sort, paginate the existing static cards ---------- */
function renderList() {
    const matched = cards.filter(matchesFilters);
    const sorted = [...matched].sort(sortCompare);
    const totalPages = Math.max(1, Math.ceil(sorted.length / PAGE_SIZE));
    state.page = Math.min(state.page, totalPages);
    const start = (state.page - 1) * PAGE_SIZE;
    const visible = new Set(sorted.slice(start, start + PAGE_SIZE));

    countEl.textContent = matched.length
        ? `${matched.length} stay${matched.length > 1 ? "s" : ""} found`
        : "0 stays found";

    // Reorder DOM to match sort order, then show/hide per filter + page
    sorted.forEach((card) => listEl.appendChild(card));
    cards.forEach((card) => {
        const isMatch = matched.includes(card);
        card.classList.toggle("is-hidden-by-filter", !isMatch);
        card.classList.toggle("is-hidden-by-page", isMatch && !visible.has(card));
    });

    emptyEl.classList.toggle("hidden", matched.length > 0);
    listEl.classList.toggle("hidden", matched.length === 0);

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

    const mkBtn = (label, page, active = false, disabled = false) => {
        const btn = document.createElement("button");
        btn.type = "button";
        btn.textContent = label;
        btn.className = active
            ? "btn-primary rounded-lg w-9 h-9 text-sm font-semibold"
            : "btn-ghost rounded-lg w-9 h-9 text-sm font-semibold";
        if (disabled) btn.disabled = true;
        btn.addEventListener("click", () => {
            state.page = page;
            renderList();
            document.querySelector("main").scrollIntoView({ behavior: "smooth", block: "start" });
        });
        return btn;
    };

    paginationEl.appendChild(mkBtn("‹", Math.max(1, state.page - 1), false, state.page === 1));
    for (let p = 1; p <= totalPages; p++) {
        paginationEl.appendChild(mkBtn(String(p), p, p === state.page));
    }
    paginationEl.appendChild(mkBtn("›", Math.min(totalPages, state.page + 1), false, state.page === totalPages));
}

/* ---------- Active filter chips ---------- */
const chipsRow = document.getElementById("chips-row");
function renderChips() {
    const chips = [];
    if (state.priceMin > 50 || state.priceMax < 500) {
        chips.push({ label: `$${state.priceMin} – ${state.priceMax >= 500 ? "$500+" : "$" + state.priceMax}`, clear: () => { state.priceMin = 50; state.priceMax = 500; } });
    }
    state.stars.forEach((s) => chips.push({ label: `${s} star`, clear: () => state.stars.delete(s) }));
    if (state.guestRating) chips.push({ label: `${state.guestRating}+ rating`, clear: () => { state.guestRating = 0; } });
    state.types.forEach((t) => chips.push({ label: t, clear: () => state.types.delete(t) }));
    state.amenities.forEach((a) => chips.push({ label: a, clear: () => state.amenities.delete(a) }));
    state.neighborhoods.forEach((n) => chips.push({ label: n, clear: () => state.neighborhoods.delete(n) }));

    chipsRow.innerHTML = "";
    if (!chips.length) {
        chipsRow.classList.add("hidden");
        chipsRow.classList.remove("flex");
        updateMobileFilterCount();
        return;
    }
    chipsRow.classList.remove("hidden");
    chipsRow.classList.add("flex");

    chips.forEach((c) => {
        const chip = document.createElement("span");
        chip.className = "chip";
        chip.innerHTML = `<span>${c.label}</span>`;
        const btn = document.createElement("button");
        btn.type = "button";
        btn.innerHTML = '<i class="fa-solid fa-xmark text-[10px]"></i>';
        btn.addEventListener("click", () => {
            c.clear();
            state.page = 1;
            update();
        });
        chip.appendChild(btn);
        chipsRow.appendChild(chip);
    });

    const clearAllBtn = document.createElement("button");
    clearAllBtn.type = "button";
    clearAllBtn.className = "text-xs font-semibold text-[var(--orange-600)] hover:underline px-1";
    clearAllBtn.textContent = "Clear all";
    clearAllBtn.addEventListener("click", resetFilters);
    chipsRow.appendChild(clearAllBtn);

    updateMobileFilterCount();
}

function updateMobileFilterCount() {
    const count = state.stars.size + state.types.size + state.amenities.size + state.neighborhoods.size
        + (state.guestRating ? 1 : 0) + ((state.priceMin > 50 || state.priceMax < 500) ? 1 : 0);
    const badge = document.getElementById("mobile-filter-count");
    if (count > 0) {
        badge.textContent = count;
        badge.classList.remove("hidden");
        badge.classList.add("flex");
    } else {
        badge.classList.add("hidden");
        badge.classList.remove("flex");
    }
}

/* ---------- Master update ---------- */
function update() {
    syncControls();
    renderChips();
    renderList();
}

/* ---------- Sort dropdown ---------- */
document.getElementById("sort-select").addEventListener("change", (e) => {
    state.sort = e.target.value;
    state.page = 1;
    renderList();
});

/* ---------- Empty state reset ---------- */
document.getElementById("empty-reset").addEventListener("click", resetFilters);

/* ---------- Mobile filter drawer ---------- */
const drawer = document.getElementById("mobile-filter-drawer");
const openDrawerBtn = document.getElementById("mobile-filter-open");
const closeDrawerBtn = document.getElementById("mobile-filter-close");
const backdrop = document.getElementById("mobile-filter-backdrop");
const applyBtn = document.getElementById("mobile-filter-apply");
const mobileResetBtn = document.getElementById("mobile-filter-reset");

function openDrawer() {
    drawer.classList.remove("hidden");
    document.body.style.overflow = "hidden";
}
function closeDrawer() {
    drawer.classList.add("hidden");
    document.body.style.overflow = "";
}
openDrawerBtn.addEventListener("click", openDrawer);
closeDrawerBtn.addEventListener("click", closeDrawer);
backdrop.addEventListener("click", closeDrawer);
applyBtn.addEventListener("click", closeDrawer);
mobileResetBtn.addEventListener("click", resetFilters);


/* ---------- Initial render ---------- */
update();