(function () {
    var overlay = null;
    var selectedLanguage = 'en';
    var selectedCurrency = 'USD';
    var sitePath = '';

    function updateTriggerDisplay() {
        document.querySelectorAll('[data-locale-trigger]').forEach(function (el) {
            var codeEl = el.querySelector('[data-selected-currency]');
            if (codeEl) codeEl.textContent = selectedCurrency;
        });

        document.querySelectorAll('.locale-lang-tab').forEach(function (tab) {
            tab.classList.toggle('active', tab.getAttribute('data-lang') === selectedLanguage);
        });

        document.querySelectorAll('.locale-currency-item').forEach(function (item) {
            item.classList.toggle('active', item.getAttribute('data-currency') === selectedCurrency);
        });
    }

    function applyLanguage(lang) {
        selectedLanguage = lang || 'en';
        if (window.SiteI18n) {
            SiteI18n.apply(selectedLanguage);
        }
    }

    function openModal() {
        if (overlay) overlay.classList.add('open');
        document.body.style.overflow = 'hidden';
    }

    function closeModal() {
        if (overlay) overlay.classList.remove('open');
        document.body.style.overflow = '';
    }

    function savePreferences(language, currency, reload) {
        $.ajax({
            type: 'POST',
            url: sitePath + 'Common/SetLocale',
            data: { language: language, currency: currency },
            success: function (response) {
                if (response && response.success) {
                    selectedLanguage = response.language || language;
                    selectedCurrency = response.currency || currency;
                    updateTriggerDisplay();
                    applyLanguage(selectedLanguage);
                    closeModal();
                    if (reload) {
                        window.location.reload();
                    }
                }
            }
        });
    }

    function bindEvents() {
        document.querySelectorAll('[data-locale-open]').forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                openModal();
            });
        });

        if (overlay) {
            overlay.addEventListener('click', function (e) {
                if (e.target === overlay) closeModal();
            });
        }

        var closeBtn = document.querySelector('[data-locale-close]');
        if (closeBtn) {
            closeBtn.addEventListener('click', closeModal);
        }

        document.querySelectorAll('.locale-lang-tab').forEach(function (tab) {
            tab.addEventListener('click', function () {
                var lang = tab.getAttribute('data-lang');
                if (lang && lang !== selectedLanguage) {
                    savePreferences(lang, selectedCurrency, true);
                }
            });
        });

        document.querySelectorAll('.locale-currency-item').forEach(function (item) {
            item.addEventListener('click', function () {
                var currency = item.getAttribute('data-currency');
                if (currency && currency !== selectedCurrency) {
                    localStorage.setItem("selectedCurrency", currency);
                    savePreferences(selectedLanguage, currency, true);
                } else {
                    closeModal();
                }
            });
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') closeModal();
        });
    }

    function applyPreferences(prefs) {
        selectedLanguage = prefs.language || 'en';
        selectedCurrency = prefs.currency || 'USD';
        // Prefer USD for flight pricing; clear stale MXN from older sessions
        if (!selectedCurrency || selectedCurrency.toUpperCase() === 'MXN') {
            selectedCurrency = 'USD';
        }
        try { localStorage.setItem('selectedCurrency', selectedCurrency); } catch (e) { }
        sitePath = prefs.sitePath || '/';
        updateTriggerDisplay();
        applyLanguage(selectedLanguage);
    }

    window.LocaleCurrency = {
        init: function (config) {
            overlay = document.getElementById('localeModalOverlay');
            applyPreferences(config || window.__localePrefs || {});
            bindEvents();
        }
    };
})();
