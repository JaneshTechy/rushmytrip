/**
 * Multicity search — dynamic legs (min 1, max 4) with add/remove icons.
 * Uses same Round Trip visual design; does not modify myjs.js OW/RT handlers.
 */
(function (window, $) {
    'use strict';

    var MIN_LEGS = 1;
    var MAX_LEGS = 4;
    var DEFAULT_LEGS = 2;
    var isAddingLeg = false;

    function cabinCode() {
        var fcls = $('#flight_class_select').val();
        if (fcls === '1') return 'F';
        if (fcls === '2') return 'B';
        if (fcls === '3') return 'P';
        return 'Y';
    }

    function isAirportValue(val) {
        return val && String(val).trim().indexOf('-') === 3;
    }

    function legCount() {
        return $('#mcLegsContainer .mc-leg-block').length;
    }

    function getLegBlocks() {
        return $('#mcLegsContainer .mc-leg-block');
    }

    function reindexLegs() {
        getLegBlocks().each(function (index) {
            var n = index + 1;
            var $leg = $(this);
            $leg.attr('data-leg', n);
            $leg.find('.mc-leg-title').text('Flight ' + n);
            $leg.find('.mc-swap-btn').attr('data-leg', n);

            var $from = $leg.find('.mc-from');
            var $to = $leg.find('.mc-to');
            var $date = $leg.find('.mc-date');
            var $label = $leg.find('.return-label');
            var $dateBox = $leg.find('.mc-date-box');

            $from.attr('id', 'mc_from_' + n);
            $to.attr('id', 'mc_to_' + n);
            $date.attr('id', 'mc_date_' + n);
            $label.attr('for', 'mc_date_' + n);
            $dateBox.off('click').on('click', function () {
                document.getElementById('mc_date_' + n).focus();
            });
        });
        updateLegControls();
        setupDatepickers();
        bindAutoChainLegs();
        if (window.SiteI18n && typeof SiteI18n.reapply === 'function') {
            SiteI18n.reapply();
        }
    }

    function updateLegControls() {
        var count = legCount();
        var $add = $('#mcAddLegBtn');
        var $hint = $('#mcLegHint');

        if (count >= MAX_LEGS) {
            $add.addClass('is-hidden').prop('disabled', true);
        } else {
            $add.removeClass('is-hidden').prop('disabled', false);
        }

        getLegBlocks().each(function () {
            var $btn = $(this).find('.mc-leg-remove');
            if (count <= MIN_LEGS) {
                $btn.addClass('is-hidden').prop('disabled', true);
            } else {
                $btn.removeClass('is-hidden').prop('disabled', false);
            }
        });

        $hint.text(count + ' of ' + MAX_LEGS + ' flights');
    }

    function createLegElement(legNumber) {
        var tpl = document.getElementById('mcLegTemplate');
        if (!tpl) return $();

        // <template> clone must use native DOM — jQuery .find() on DocumentFragment is unreliable
        var fragment = tpl.content.cloneNode(true);
        var el = fragment.querySelector('.mc-leg-block');
        if (!el) return $();

        el.setAttribute('data-leg', String(legNumber));
        var title = el.querySelector('.mc-leg-title');
        if (title) title.textContent = 'Flight ' + legNumber;

        return $(el);
    }

    function addLeg(prefillFrom) {
        // One click = exactly one leg
        if (isAddingLeg) return;
        if (legCount() >= MAX_LEGS) {
            updateLegControls();
            return;
        }

        isAddingLeg = true;
        try {
            var n = legCount() + 1;
            var $leg = createLegElement(n);
            if (!$leg.length) {
                console.error('MulticitySearch: failed to create leg from template');
                return;
            }

            var prevTo = '';
            if (n > 1) {
                prevTo = ($('#mc_to_' + (n - 1)).val() || '').toString();
            }

            $('#mcLegsContainer').append($leg);

            if (prefillFrom) {
                $leg.find('.mc-from').val(prefillFrom);
            } else if (prevTo) {
                $leg.find('.mc-from').val(prevTo);
            }

            reindexLegs();
            initAirportAutocomplete($leg.find('.mc-airport'));
            updateLegControls();
        } finally {
            isAddingLeg = false;
        }
    }

    function removeLeg($leg) {
        if (legCount() <= MIN_LEGS) return;
        var $date = $leg.find('.mc-date');
        if ($date.hasClass('hasDatepicker')) {
            try { $date.datepicker('destroy'); } catch (e) { /* ignore */ }
        }
        $leg.remove();
        reindexLegs();
    }

    function ensureInitialLegs() {
        var $container = $('#mcLegsContainer');
        if (!$container.length) return;

        var defaultAttr = parseInt($('#mcLegsPanel').data('default-legs'), 10);
        if (!isNaN(defaultAttr) && defaultAttr >= MIN_LEGS && defaultAttr <= MAX_LEGS) {
            DEFAULT_LEGS = defaultAttr;
        } else {
            DEFAULT_LEGS = 2;
        }

        $container.empty();
        for (var i = 1; i <= DEFAULT_LEGS; i++) {
            var $leg = createLegElement(i);
            if ($leg.length) {
                $container.append($leg);
            }
        }

        // Fallback if template clone failed once — still try again
        if (legCount() === 0) {
            console.error('MulticitySearch: no legs created; check #mcLegTemplate');
        }

        reindexLegs();
        initAirportAutocomplete($container.find('.mc-airport'));
        updateLegControls();
    }

    function initAirportAutocomplete($inputs) {
        if (!$ || !$.fn.autocomplete || !$inputs || !$inputs.length) return;

        $inputs.each(function () {
            var $el = $(this);
            if ($el.hasClass('ui-autocomplete-input')) {
                try { $el.autocomplete('destroy'); } catch (e) { /* ignore */ }
            }

            $el.autocomplete({
                source: function (request, response) {
                    var term = (request.term || '').trim();
                    if (term.length < 2) {
                        response([]);
                        return;
                    }
                    if ($el.data('airportXhr')) {
                        try { $el.data('airportXhr').abort(); } catch (e) { /* ignore */ }
                    }
                    var xhr = $.ajax({
                        url: '/result/getairport',
                        dataType: 'json',
                        data: { pickUpLocation: term },
                        success: function (data) {
                            response($.map(data || [], function (item) {
                                return {
                                    value: item.code,
                                    label: (item.code + '-' + item.name + ', ' + item.cityName)
                                };
                            }));
                        },
                        error: function (xhrErr) {
                            if (xhrErr.statusText !== 'abort') response([]);
                        }
                    });
                    $el.data('airportXhr', xhr);
                },
                select: function (event, ui) {
                    $(this).val(ui.item.label);
                    var $leg = $(this).closest('.mc-leg-block');
                    if ($(this).hasClass('mc-from')) {
                        $leg.find('.mc-to').focus();
                    }
                    return false;
                },
                minLength: 2,
                delay: 150,
                autoFocus: true
            }).focus(function () {
                var val = ($(this).val() || '').trim();
                if (val.length >= 2) {
                    $(this).autocomplete('search', val);
                }
            });
        });
    }

    function destroyDatepickers() {
        getLegBlocks().find('.mc-date').each(function () {
            var $el = $(this);
            if ($el.hasClass('hasDatepicker')) {
                try { $el.datepicker('destroy'); } catch (e) { /* ignore */ }
            }
        });
    }

    function setupDatepickers() {
        if (!$ || !$.fn.datepicker) return;

        if (window.SiteI18n && SiteI18n.setupDatepicker) {
            SiteI18n.setupDatepicker();
        }

        destroyDatepickers();

        var common = {
            minDate: 0,
            dateFormat: 'mm-dd-yy',
            numberOfMonths: $(window).width() < 768 ? 1 : 2,
            beforeShow: function () {
                setTimeout(function () {
                    $('#ui-datepicker-div').addClass('notranslate');
                }, 0);
            }
        };

        var count = legCount();
        for (var i = 1; i <= count; i++) {
            (function (legIndex) {
                var $date = $('#mc_date_' + legIndex);
                if (!$date.length) return;

                var opts = $.extend({}, common, {
                    onSelect: function (selected) {
                        var dt = new Date(selected);
                        for (var j = legIndex + 1; j <= legCount(); j++) {
                            var $next = $('#mc_date_' + j);
                            if ($next.length && $next.hasClass('hasDatepicker')) {
                                $next.datepicker('option', 'minDate', dt);
                            }
                        }
                    }
                });

                if (legIndex > 1) {
                    var prevVal = $('#mc_date_' + (legIndex - 1)).val();
                    if (prevVal) {
                        opts.minDate = new Date(prevVal);
                    }
                }

                $date.datepicker(opts);
            })(i);
        }
    }

    function showMulticityUi(show) {
        var $grid = $('.flight-search-box .search-grid');

        if (show) {
            // Always show 2 legs by default when opening Multicity
            if (legCount() !== DEFAULT_LEGS && legCount() < DEFAULT_LEGS) {
                ensureInitialLegs();
            } else if (legCount() === 0) {
                ensureInitialLegs();
            }
            $('#mcLegsPanel').show();
            $grid.find('.from-to-wrapper').first().hide();
            $grid.find('.date-range-box').hide();
            $grid.find('.passenger-box').show();
            $grid.find('.search-col').show();
            $grid.css('grid-template-columns', '1fr auto');
            $grid.show();

            $('#flight_submit').hide();
            $('#mc_search_btn').show();

            if ($('#flight_from').val() && !$('#mc_from_1').val()) {
                $('#mc_from_1').val($('#flight_from').val());
            }
            if ($('#flight_to').val() && !$('#mc_to_1').val()) {
                $('#mc_to_1').val($('#flight_to').val());
            }
            if ($('#txtFromDate').val() && !$('#mc_date_1').val()) {
                $('#mc_date_1').val($('#txtFromDate').val());
            }
            if ($('#mc_to_1').val() && !$('#mc_from_2').val() && legCount() >= 2) {
                $('#mc_from_2').val($('#mc_to_1').val());
            }
            updateLegControls();
        } else {
            $('#mcLegsPanel').hide();
            $grid.find('.from-to-wrapper').first().show();
            $grid.find('.date-range-box').show();
            $grid.css('grid-template-columns', '2.5fr 1.5fr 1fr auto');
            $grid.show();

            $('#mc_search_btn').hide();
            $('#flight_submit').show();
        }
    }

    function validateLegs() {
        var count = legCount();
        if (count < MIN_LEGS) {
            alert('Please add at least ' + MIN_LEGS + ' flight.');
            return false;
        }
        if (count > MAX_LEGS) {
            alert('Maximum ' + MAX_LEGS + ' flights allowed.');
            return false;
        }

        for (var i = 1; i <= count; i++) {
            var from = $('#mc_from_' + i).val();
            var to = $('#mc_to_' + i).val();
            var date = $('#mc_date_' + i).val();

            if (!isAirportValue(from)) {
                alert('Please select From airport for Flight ' + i + ' from suggestions.');
                $('#mc_from_' + i).focus();
                return false;
            }
            if (!isAirportValue(to)) {
                alert('Please select To airport for Flight ' + i + ' from suggestions.');
                $('#mc_to_' + i).focus();
                return false;
            }
            if (!date) {
                alert('Please enter Departure date for Flight ' + i + '.');
                $('#mc_date_' + i).focus();
                return false;
            }
            if (String(from).trim() === String(to).trim()) {
                alert('From and To cannot be the same for Flight ' + i + '.');
                $('#mc_to_' + i).focus();
                return false;
            }
        }
        return true;
    }

    function buildRequest() {
        var currency = (window.__localePrefs && window.__localePrefs.currency) || 'USD';
        var legs = [];
        var count = legCount();
        for (var i = 1; i <= count; i++) {
            legs.push({
                from: $('#mc_from_' + i).val(),
                to: $('#mc_to_' + i).val(),
                depDate: $('#mc_date_' + i).val()
            });
        }
        return {
            adult: parseInt($('#flight_adult').val() || '1', 10),
            child: parseInt($('#flight_child').val() || '0', 10),
            infant: parseInt($('#flight_infant').val() || '0', 10),
            cabin: cabinCode(),
            currency: currency,
            utmSource: 'HOME',
            legs: legs
        };
    }

    function submitMulticity() {
        if (!validateLegs()) return;

        var request = buildRequest();
        sessionStorage.setItem('multicityAsyncRequest', JSON.stringify(request));
        window.location.href = '/multicity-async';
    }

    function cabinToSelectValue(cabin) {
        switch ((cabin || 'Y').toString().toUpperCase()) {
            case 'F': case '1': return '1';
            case 'B': case '2': return '2';
            case 'P': case '3': return '3';
            default: return '4';
        }
    }

    function updatePassengerSummary() {
        var adult = parseInt($('#flight_adult').val() || '1', 10);
        var child = parseInt($('#flight_child').val() || '0', 10);
        var infant = parseInt($('#flight_infant').val() || '0', 10);
        var total = adult + child + infant;
        var cabinText = $('#flight_class_select option:selected').text() || 'Economy';
        $('#pass_info').val(total + ' Pax, ' + cabinText.trim());
    }

    /**
     * Prefill Multicity amend/search form from a saved request
     * (sessionStorage multicityAsyncRequest).
     */
    function prefillFromRequest(request) {
        if (!request || !request.legs || !request.legs.length) return;

        $('#flight_adult').val(request.adult || 1);
        $('#flight_child').val(request.child || 0);
        $('#flight_infant').val(request.infant || 0);
        $('#flight_class_select').val(cabinToSelectValue(request.cabin));
        updatePassengerSummary();

        var legs = request.legs.slice(0, MAX_LEGS);
        var targetCount = Math.max(DEFAULT_LEGS, legs.length);
        targetCount = Math.min(MAX_LEGS, Math.max(MIN_LEGS, targetCount));

        var $container = $('#mcLegsContainer');
        $container.empty();
        for (var i = 1; i <= targetCount; i++) {
            var $leg = createLegElement(i);
            if ($leg.length) {
                $container.append($leg);
            }
        }
        reindexLegs();
        initAirportAutocomplete($container.find('.mc-airport'));

        for (var j = 0; j < legs.length && j < targetCount; j++) {
            var n = j + 1;
            var leg = legs[j];
            $('#mc_from_' + n).val(leg.from || '');
            $('#mc_to_' + n).val(leg.to || '');
            $('#mc_date_' + n).val(leg.depDate || '');
        }

        setupDatepickers();
        updateLegControls();
        showMulticityUi(true);
    }

    function bindSwapButtons() {
        $(document).on('click', '.mc-swap-btn', function (e) {
            e.preventDefault();
            var $leg = $(this).closest('.mc-leg-block');
            var $from = $leg.find('.mc-from');
            var $to = $leg.find('.mc-to');
            var tmp = $from.val();
            $from.val($to.val());
            $to.val(tmp);
        });
    }

    function bindAutoChainLegs() {
        $(document).off('blur.mcChain change.mcChain autocompleteclose.mcChain', '.mc-to');
        $(document).on('blur.mcChain change.mcChain autocompleteclose.mcChain', '.mc-to', function () {
            var $leg = $(this).closest('.mc-leg-block');
            var n = parseInt($leg.attr('data-leg'), 10);
            var nextFrom = $('#mc_from_' + (n + 1));
            if (nextFrom.length && $(this).val() && !nextFrom.val()) {
                nextFrom.val($(this).val());
            }
        });
    }

    function bindAddRemove() {
        $(document).off('click.mcAdd', '#mcAddLegBtn').on('click.mcAdd', '#mcAddLegBtn', function (e) {
            e.preventDefault();
            e.stopPropagation();
            addLeg();
            return false;
        });

        $(document).off('click.mcRemove', '.mc-leg-remove').on('click.mcRemove', '.mc-leg-remove', function (e) {
            e.preventDefault();
            e.stopPropagation();
            removeLeg($(this).closest('.mc-leg-block'));
            return false;
        });
    }

    function bindTripToggle() {
        $(document).off('change.mcTrip', 'input[name="trip"], input[name="flight-options"], #Multicity, #OneWay, #RoundTrip')
            .on('change.mcTrip', 'input[name="trip"], input[name="flight-options"], #Multicity, #OneWay, #RoundTrip', function () {
                var val = $(this).val();
                var isMc = val === 'Multicity' || $(this).attr('id') === 'Multicity';
                showMulticityUi(isMc);
            });

        // Label click fallback (some browsers don't fire change reliably with nested spans)
        $(document).off('click.mcTripLabel', 'label[for="Multicity"], #Multicity')
            .on('click.mcTripLabel', 'label[for="Multicity"], #Multicity', function () {
                setTimeout(function () {
                    if ($('#Multicity').is(':checked')) {
                        showMulticityUi(true);
                    }
                }, 0);
            });
    }

    function init() {
        if (!$('#mcLegsPanel').length) return;

        var minAttr = parseInt($('#mcLegsPanel').data('min-legs'), 10);
        var maxAttr = parseInt($('#mcLegsPanel').data('max-legs'), 10);
        if (!isNaN(minAttr) && minAttr > 0) MIN_LEGS = minAttr;
        if (!isNaN(maxAttr) && maxAttr >= MIN_LEGS) MAX_LEGS = maxAttr;

        // Always start with 2 legs for Multicity
        DEFAULT_LEGS = 2;
        ensureInitialLegs();
        bindTripToggle();
        bindSwapButtons();
        bindAutoChainLegs();
        bindAddRemove();

        $('#mc_search_btn').off('click.mcSearch').on('click.mcSearch', function (e) {
            e.preventDefault();
            submitMulticity();
        });

        if ($('#Multicity').is(':checked')) {
            showMulticityUi(true);
        }
    }

    window.MulticitySearch = {
        submit: submitMulticity,
        show: showMulticityUi,
        addLeg: addLeg,
        removeLeg: removeLeg,
        prefill: prefillFromRequest,
        updatePassengerSummary: updatePassengerSummary
    };

    $(init);
})(window, window.jQuery);
