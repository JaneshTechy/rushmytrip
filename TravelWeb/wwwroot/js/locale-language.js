(function (window, $) {
    'use strict';

    var currentLang = 'en';

    var messages = {
        en: {
            'nav.home': 'Home',
            'nav.about': 'About Us',
            'nav.contact': 'Contact Us',
            'nav.service': '24/7 Live Service',
            'hero.subtitle': 'Amazing Flights, Unbelievable Price!',
            'hero.title': 'Hop On-Board and Travel the World',
            'search.oneWay': 'One Way',
            'search.roundTrip': 'Round Trip',
            'search.multiCity': 'MultiCity',
            'search.fromCity': 'From City',
            'search.toCity': 'To City',
            'search.departure': 'Departure',
            'search.returning': 'Returning',
            'search.addDate': 'Add Date',
            'search.addReturn': '+ Add Return',
            'search.passengers': 'Passengers',
            'search.search': 'Search',
            'search.cityPlaceholder': 'Country, city or airport',
            'search.economy': 'Economy',
            'search.premium': 'Premium',
            'search.business': 'Business',
            'search.first': 'First',
            'search.adults': 'Adults',
            'search.children': 'Children',
            'search.infant': 'Infant',
            'search.done': 'Done',
            'search.enterReturn': 'Please enter your Return Date',
            'search.enterDepart': 'Please enter your Departure Date',
            'search.enterFrom': 'Please enter your Departure City',
            'search.enterTo': 'Please enter your Arrival City',
            'modal.language': 'Language',
            'modal.currency': 'Currency',
            'footer.about': 'About RushMyFlight',
            'footer.contact': 'Contact Us',
            'footer.support': 'Customer Support',
            'footer.privacy': 'Privacy Policy',
            'footer.terms': 'Terms of Use',
            'footer.disclaimer': 'Disclaimer',
            'footer.blogs': 'Blogs',
            'footer.explore': 'Explore',
            'footer.book': 'Book',
            'footer.travelers': 'From Our Travelers',
            'footer.connect': 'Connect with us',
            'result.amendSearch': 'Amend Search',
            'result.hideSearch': 'Hide Search',
            'result.searchAgain': 'Search Again',
            'result.departureDate': 'Departure Date',
            'result.returnDate': 'Return Date',
            'search.arrival': 'Arrival',
            'search.adult': 'Adult',
            'search.child': 'Child',
            'search.infants': 'Infants',
            'result.filters': 'Filters',
            'result.clearAll': 'Clear All',
            'result.stops': 'Stops',
            'result.reset': 'Reset',
            'result.nonStop': 'Non-stop',
            'result.bookPrice': 'Book at This Price',
            'result.lowestPrice': 'Lowest Price Found',
            'result.perPerson': 'per person',
            'result.loadMore': 'Load More Flights',
            'result.pricesUpdating': 'Prices Updating Fast',
            'result.noResults': 'Sorry! No Result Found',
            'result.pleaseWait': 'Please Wait For Your Flights...',
            'result.searchingDeals': 'Let us search the best Flight Deals For You.',
            'result.time': 'Time',
            'result.departureTime': 'Departure Time',
            'result.arrivalTime': 'Arrival Time',
            'details.backToResults': 'Back to Results',
            'details.flightSelected': 'Flight Selected',
            'details.passengerDetails': 'Passenger Details',
            'details.payment': 'Payment',
            'details.confirmation': 'Confirmation',
            'details.bookingSummary': 'Booking Summary',
            'details.tripType': 'Trip Type',
            'details.passengers': 'Passengers',
            'details.depart': 'Depart',
            'details.return': 'Return',
            'details.route': 'Route',
            'details.priceDetails': 'Price Details',
            'details.travellerCharges': 'Traveller Charges',
            'details.feesTaxes': 'Fees & Taxes',
            'details.taxesFees': 'Taxes & Fees',
            'details.assistanceFees': '24/7 support,fare monitoring & after-sales',
            'details.total': 'Total',
            'details.fareNotePrefix': 'Please Note: All fares are quoted in USD. Your card may be billed in multiple charges totalling the final total fare',
            'details.fareNoteSuffix': 'Additional Baggage fees may apply as per the airline policies.',
            'details.fareRules': 'Fare Rules',
            'details.refundable': 'Refundable',
            'details.cancellationFee': 'Cancellation fee applies',
            'details.noFreeChanges': 'No Free Changes',
            'details.rebookingFee': 'Rebooking fee + fare diff',
            'details.secureCheckout': 'Secure Checkout',
            'details.sslEncrypted': 'SSL Encrypted',
            'details.noHiddenFees': 'No Hidden Fees',
            'details.needHelp': 'Need help booking?',
            'details.needHelpText': 'Our travel experts are available 24/7 to find you the best deal.',
            'details.contactDetails': 'Contact Details',
            'details.contactSubtitle': 'Used for booking communication only',
            'details.contactNotice': 'We will use the below contact details for any further booking communication and updates.',
            'details.phoneNumber': 'Phone Number',
            'details.emailAddress': 'Email Address',
            'details.phonePlaceholder': 'Phone number',
            'details.emailPlaceholder': 'your@email.com',
            'details.enterPassengerDetails': 'Enter Passenger Details',
            'details.passportNote': "(*Please ensure information matches passenger's passport.)",
            'details.nameMatchWarning': 'Name must exactly match the passport. Name changes after ticketing are subject to airline fees.',
            'details.title': 'Title',
            'details.firstName': 'First Name',
            'details.middleName': 'Middle Name',
            'details.lastName': 'Last Name',
            'details.gender': 'Gender',
            'details.dateOfBirth': 'Date of Birth',
            'details.asOnPassport': '(As in Passport)',
            'details.month': 'Month',
            'details.day': 'Day',
            'details.year': 'Year',
            'details.asOnPassportPlaceholder': 'As on passport',
            'details.male': 'Male',
            'details.female': 'Female',
            'details.continue': 'Continue',
            'details.paymentDetails': 'Payment Details',
            'details.safeBilling': 'Safe & Secure Billing',
            'details.safeBillingSub': 'Your credit card information is protected by a Secure SSL Encrypted Transaction',
            'details.fullNameOnCard': 'Full Name on Card',
            'details.cardNumber': 'Card Number',
            'details.cardHolderPlaceholder': 'Card Holder Name',
            'details.expiryMonth': 'Expiry Month',
            'details.expiryYear': 'Expiry Year',
            'details.cvv': 'CVV',
            'details.cvvDigits': '3 or 4 digits',
            'details.cvvHint': '3 or 4 digits on your card (CVV location varies by card type)',
            'details.enterBillingInfo': 'Enter Billing Information',
            'details.billingSubtitle': 'Billing address of your Credit/Debit Card',
            'details.billingAddress': 'Billing Address',
            'details.billingNote': '* Please provide the Billing address of your Credit/Debit Card',
            'details.billingMatchNotice': 'Billing address must match the address registered with your card issuer to avoid payment declines.',
            'details.address': 'Address',
            'details.streetPlaceholder': 'Street address',
            'details.zipCode': 'Zip / Postal Code',
            'details.zipPlaceholder': 'Zip Code',
            'details.city': 'City',
            'details.country': 'Country',
            'details.selectCountry': 'Select Country',
            'details.stateRegion': 'State / Region',
            'details.statePlaceholder': 'State/Region',
            'details.paymentEncrypted': 'Your payment data is encrypted and never stored on our servers.',
            'details.confirmBooking': 'Confirm Booking',
            'details.showDetails': 'Show Details',
            'details.hideDetails': 'Hide Details',
            'details.onTime': 'On time',
            'details.layover': 'Layover',
            'details.unknownCard': 'Unknown card',
            'details.flight': 'Flight',
            'features.heading': 'What We Offer?',
            'features.cheapFlight.title': 'Cheap Flight Tickets',
            'features.cheapFlight.desc': 'Book affordable flights at guaranteed low prices with exclusive travel deals.',

            'features.deals.title': 'Great Deals',
            'features.deals.desc': 'Get great flight deals with excellent customer support — you can call us anytime for help, even at the airport.',

            'features.support.title': 'Multilingual Support',
            'features.support.desc': 'Get assistance in multiple languages for a smooth and convenient booking experience.',

            'features.assistance.title': '24 Hour Assistance',
            'features.assistance.desc': 'Get 24/7 human support in English, Spanish, and Portuguese anytime you need assistance.',
            'deals.heading': 'Best Flight Deals',
            'deals.subtitle': 'Limited-time Offer: Hurry before it’s too late.',

            'deals.nyc.city': 'NEW YORK',
            'deals.nyc.route': 'NYC - From Los Angeles, CA',
            'deals.nyc.note': '*Fare refreshed hourly',

            'deals.lax.city': 'LOS ANGELES',
            'deals.lax.route': 'LAX - From New York, NY',
            'deals.lax.note': "Editor's Pick",

            'deals.mia.city': 'MIAMI',
            'deals.mia.route': 'MIA - From Chicago, IL',
            'deals.mia.note': 'Smart Value',

            'deals.help.title': 'Need Assistance with your Bookings?',
            'deals.help.subtitle': 'Our experienced travel agents will help with your flight deals and bookings.',
            'deals.help.call': 'Call Now to Book!',
            'lastMinute.heading': 'Last Minute Flights with 24/7 Human Support',
            'lastMinute.subtitle': 'Grab spontaneous travel deals before they’re gone.',
            'lastMinute.checkNow': 'Check Now ➜',
            'why.heading': 'RushMyFlight: Simply the Best!',
            'why.subtitle': 'We are your trusted travel partner.',

            'why.price.title': 'Best Competitive Prices',
            'why.price.desc': 'Guaranteed amazing flight deals at the best competitive prices.',

            'why.booking.title': 'Easy Booking Experience',
            'why.booking.desc': 'Enjoy an easy and hassle-free flight booking experience.',

            'why.support.title': 'Reliable Assistance',
            'why.support.desc': 'Get 24/7 customer support whenever and wherever.',

            'why.options.title': 'Countless Options',
            'why.options.desc': 'Discover countless destinations throughout the world.',
        },
        es: {
            'nav.home': 'Inicio',
            'nav.about': 'Sobre nosotros',
            'nav.contact': 'Contáctenos',
            'nav.service': 'Servicio en vivo 24/7',
            'hero.subtitle': '¡Vuelos increíbles, precio increíble!',
            'hero.title': 'Súbete a bordo y viaja por el mundo',
            'search.oneWay': 'Solo ida',
            'search.roundTrip': 'Ida y vuelta',
            'search.multiCity': 'MultiCity',
            'search.fromCity': 'Ciudad de origen',
            'search.toCity': 'Ciudad de destino',
            'search.departure': 'Salida',
            'search.returning': 'Regreso',
            'search.addDate': 'Agregar fecha',
            'search.addReturn': '+ Agregar regreso',
            'search.passengers': 'Pasajeros',
            'search.search': 'Buscar',
            'search.cityPlaceholder': 'País, ciudad o aeropuerto',
            'search.economy': 'Económica',
            'search.premium': 'Premium',
            'search.business': 'Ejecutiva',
            'search.first': 'Primera',
            'search.adults': 'Adultos',
            'search.children': 'Niños',
            'search.infant': 'Bebé',
            'search.done': 'Listo',
            'search.enterReturn': 'Por favor ingrese su fecha de regreso',
            'search.enterDepart': 'Por favor ingrese su fecha de salida',
            'search.enterFrom': 'Por favor ingrese su ciudad de salida',
            'search.enterTo': 'Por favor ingrese su ciudad de llegada',
            'modal.language': 'Idioma',
            'modal.currency': 'Divisa',
            'footer.about': 'Sobre RushMyFlight',
            'footer.contact': 'Contáctenos',
            'footer.support': 'Atención al cliente',
            'footer.privacy': 'Política de privacidad',
            'footer.terms': 'Términos de uso',
            'footer.disclaimer': 'Aviso legal',
            'footer.blogs': 'Blogs',
            'footer.explore': 'Explorar',
            'footer.book': 'Reservar',
            'footer.travelers': 'De nuestros viajeros',
            'footer.connect': 'Conéctese con nosotros',
            'result.amendSearch': 'Modificar búsqueda',
            'result.hideSearch': 'Ocultar búsqueda',
            'result.searchAgain': 'Buscar de nuevo',
            'result.departureDate': 'Fecha de salida',
            'result.returnDate': 'Fecha de regreso',
            'search.arrival': 'Llegada',
            'search.adult': 'Adulto',
            'search.child': 'Niño',
            'search.infants': 'Bebés',
            'result.filters': 'Filtros',
            'result.clearAll': 'Borrar todo',
            'result.stops': 'Escalas',
            'result.reset': 'Restablecer',
            'result.nonStop': 'Sin escalas',
            'result.bookPrice': 'Reservar a este precio',
            'result.lowestPrice': 'Precio más bajo encontrado',
            'result.perPerson': 'por persona',
            'result.loadMore': 'Cargar más vuelos',
            'result.pricesUpdating': 'Precios actualizándose rápido',
            'result.noResults': '¡Lo sentimos! No se encontraron resultados',
            'result.pleaseWait': 'Por favor espere sus vuelos...',
            'result.searchingDeals': 'Busquemos las mejores ofertas de vuelos para usted.',
            'result.time': 'Hora',
            'result.departureTime': 'Hora de salida',
            'result.arrivalTime': 'Hora de llegada',
            'details.backToResults': 'Volver a resultados',
            'details.flightSelected': 'Vuelo seleccionado',
            'details.passengerDetails': 'Datos del pasajero',
            'details.payment': 'Pago',
            'details.confirmation': 'Confirmación',
            'details.bookingSummary': 'Resumen de reserva',
            'details.tripType': 'Tipo de viaje',
            'details.passengers': 'Pasajeros',
            'details.depart': 'Salida',
            'details.return': 'Regreso',
            'details.route': 'Ruta',
            'details.priceDetails': 'Detalles del precio',
            'details.travellerCharges': 'Cargos por viajero',
            'details.feesTaxes': 'Tarifas e impuestos',
            'details.taxesFees': 'Impuestos y tarifas',
            'details.assistanceFees': 'Soporte 24/7, monitoreo de tarifas y servicio posventa',
            'details.total': 'Total',
            'details.fareNotePrefix': 'Nota: Todas las tarifas están en USD. Su tarjeta puede facturarse en varios cargos que suman la tarifa total final de',
            'details.fareNoteSuffix': 'Pueden aplicarse tarifas adicionales de equipaje según las políticas de la aerolínea.',
            'details.fareRules': 'Reglas de tarifa',
            'details.refundable': 'Reembolsable',
            'details.cancellationFee': 'Se aplica tarifa de cancelación',
            'details.noFreeChanges': 'Sin cambios gratuitos',
            'details.rebookingFee': 'Tarifa de cambio + diferencia de tarifa',
            'details.secureCheckout': 'Pago seguro',
            'details.sslEncrypted': 'Cifrado SSL',
            'details.noHiddenFees': 'Sin cargos ocultos',
            'details.needHelp': '¿Necesita ayuda para reservar?',
            'details.needHelpText': 'Nuestros expertos en viajes están disponibles 24/7 para encontrarle la mejor oferta.',
            'details.contactDetails': 'Datos de contacto',
            'details.contactSubtitle': 'Solo para comunicación de la reserva',
            'details.contactNotice': 'Usaremos los datos de contacto siguientes para cualquier comunicación y actualización de la reserva.',
            'details.phoneNumber': 'Número de teléfono',
            'details.emailAddress': 'Correo electrónico',
            'details.phonePlaceholder': 'Número de teléfono',
            'details.emailPlaceholder': 'su@email.com',
            'details.enterPassengerDetails': 'Ingrese datos del pasajero',
            'details.passportNote': '(*Asegúrese de que la información coincida con el pasaporte del pasajero.)',
            'details.nameMatchWarning': 'El nombre debe coincidir exactamente con el pasaporte. Los cambios de nombre después de emitir el boleto están sujetos a tarifas de la aerolínea.',
            'details.title': 'Título',
            'details.firstName': 'Nombre',
            'details.middleName': 'Segundo nombre',
            'details.lastName': 'Apellido',
            'details.gender': 'Género',
            'details.dateOfBirth': 'Fecha de nacimiento',
            'details.asOnPassport': '(Como en el pasaporte)',
            'details.month': 'Mes',
            'details.day': 'Día',
            'details.year': 'Año',
            'details.asOnPassportPlaceholder': 'Como en el pasaporte',
            'details.male': 'Masculino',
            'details.female': 'Femenino',
            'details.continue': 'Continuar',
            'details.paymentDetails': 'Detalles de pago',
            'details.safeBilling': 'Facturación segura',
            'details.safeBillingSub': 'La información de su tarjeta está protegida por una transacción cifrada SSL segura',
            'details.fullNameOnCard': 'Nombre completo en la tarjeta',
            'details.cardNumber': 'Número de tarjeta',
            'details.cardHolderPlaceholder': 'Nombre del titular',
            'details.expiryMonth': 'Mes de vencimiento',
            'details.expiryYear': 'Año de vencimiento',
            'details.cvv': 'CVV',
            'details.cvvDigits': '3 o 4 dígitos',
            'details.cvvHint': '3 o 4 dígitos en su tarjeta (la ubicación del CVV varía según el tipo de tarjeta)',
            'details.enterBillingInfo': 'Ingrese información de facturación',
            'details.billingSubtitle': 'Dirección de facturación de su tarjeta de crédito/débito',
            'details.billingAddress': 'Dirección de facturación',
            'details.billingNote': '* Proporcione la dirección de facturación de su tarjeta de crédito/débito',
            'details.billingMatchNotice': 'La dirección de facturación debe coincidir con la registrada en su emisor de tarjeta para evitar rechazos de pago.',
            'details.address': 'Dirección',
            'details.streetPlaceholder': 'Dirección',
            'details.zipCode': 'Código postal',
            'details.zipPlaceholder': 'Código postal',
            'details.city': 'Ciudad',
            'details.country': 'País',
            'details.selectCountry': 'Seleccionar país',
            'details.stateRegion': 'Estado / Región',
            'details.statePlaceholder': 'Estado/Región',
            'details.paymentEncrypted': 'Sus datos de pago están cifrados y nunca se almacenan en nuestros servidores.',
            'details.confirmBooking': 'Confirmar reserva',
            'details.showDetails': 'Mostrar detalles',
            'details.hideDetails': 'Ocultar detalles',
            'details.onTime': 'A tiempo',
            'details.layover': 'Escala',
            'details.unknownCard': 'Tarjeta desconocida',
            'details.flight': 'Vuelo',
            'features.heading': '¿Qué ofrecemos?',

            'features.cheapFlight.title': 'Boletos de avión baratos',
            'features.cheapFlight.desc': 'Reserve vuelos asequibles con precios bajos garantizados y ofertas exclusivas de viaje.',

            'features.deals.title': 'Grandes ofertas',
            'features.deals.desc': 'Obtenga excelentes ofertas de vuelos con un servicio de atención al cliente excepcional. Puede llamarnos en cualquier momento, incluso desde el aeropuerto.',

            'features.support.title': 'Soporte multilingüe',
            'features.support.desc': 'Reciba asistencia en varios idiomas para disfrutar de una experiencia de reserva cómoda y sin complicaciones.',

            'features.assistance.title': 'Asistencia las 24 horas',
            'features.assistance.desc': 'Obtenga soporte humano las 24 horas del día, los 7 días de la semana, en inglés, español y portugués siempre que necesite ayuda.',
            'deals.heading': 'Las mejores ofertas de vuelos',
            'deals.subtitle': 'Oferta por tiempo limitado. ¡Apresúrate antes de que termine!',

            'deals.nyc.city': 'NUEVA YORK',
            'deals.nyc.route': 'NYC - Desde Los Ángeles, CA',
            'deals.nyc.note': '*Tarifa actualizada cada hora',

            'deals.lax.city': 'LOS ÁNGELES',
            'deals.lax.route': 'LAX - Desde Nueva York, NY',
            'deals.lax.note': 'Selección del editor',

            'deals.mia.city': 'MIAMI',
            'deals.mia.route': 'MIA - Desde Chicago, IL',
            'deals.mia.note': 'Mejor relación calidad-precio',

            'deals.help.title': '¿Necesita ayuda con sus reservas?',
            'deals.help.subtitle': 'Nuestros agentes de viajes con experiencia le ayudarán con las mejores ofertas y reservas de vuelos.',
            'deals.help.call': '¡Llame ahora para reservar!',
            'lastMinute.heading': 'Vuelos de última hora con asistencia humana 24/7',
            'lastMinute.subtitle': 'Aproveche las ofertas de viaje de último momento antes de que se agoten.',
            'lastMinute.checkNow': 'Ver ahora ➜',
            'why.heading': 'RushMyFlight: ¡Simplemente el mejor!',
            'why.subtitle': 'Somos su socio de viajes de confianza.',

            'why.price.title': 'Los mejores precios competitivos',
            'why.price.desc': 'Ofrecemos increíbles ofertas de vuelos con los mejores precios competitivos garantizados.',

            'why.booking.title': 'Experiencia de reserva sencilla',
            'why.booking.desc': 'Disfrute de una experiencia de reserva de vuelos fácil y sin complicaciones.',

            'why.support.title': 'Asistencia confiable',
            'why.support.desc': 'Obtenga atención al cliente las 24 horas, los 7 días de la semana, cuando y donde la necesite.',

            'why.options.title': 'Innumerables opciones',
            'why.options.desc': 'Descubra innumerables destinos en todo el mundo.',

        }
    };

    function readLanguageFromCookie() {
        var match = document.cookie.match(/(?:^|;\s*)SelectedLanguage=([^;]*)/);
        return match ? decodeURIComponent(match[1]) : null;
    }

    function readCurrencyFromCookie() {
        var match = document.cookie.match(/(?:^|;\s*)SelectedCurrency=([^;]*)/);
        return match ? decodeURIComponent(match[1]) : null;
    }

    function t(key) {
        var table = messages[currentLang] || messages.en;
        return table[key] || messages.en[key] || key;
    }

    function clearGoogleTranslate() {
        var host = window.location.hostname;
        document.cookie = 'googtrans=;path=/;expires=Thu, 01 Jan 1970 00:00:00 GMT';
        if (host) {
            document.cookie = 'googtrans=;path=/;domain=' + host + ';expires=Thu, 01 Jan 1970 00:00:00 GMT';
        }
        document.documentElement.classList.remove('translated-ltr', 'translated-rtl');
        document.body.classList.remove('translated-ltr', 'translated-rtl');
    }

    function setupDatepickerRegional() {
        if (!$ || !$.datepicker) return;

        $.datepicker.regional.es = {
            closeText: 'Cerrar',
            prevText: 'Ant',
            nextText: 'Sig',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            weekHeader: 'Sm',
            dateFormat: 'mm-dd-yy',
            firstDay: 0,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ''
        };

        if (currentLang === 'es') {
            $.datepicker.setDefaults($.datepicker.regional.es);
        } else {
            $.datepicker.setDefaults($.datepicker.regional['']);
        }
    }

    function apply(lang) {
        currentLang = lang === 'es' ? 'es' : 'en';
        clearGoogleTranslate();

        document.documentElement.lang = currentLang;

        document.querySelectorAll('[data-i18n]').forEach(function (el) {
            var key = el.getAttribute('data-i18n');
            var value = t(key);
            if (el.tagName === 'INPUT' || el.tagName === 'TEXTAREA') {
                if (el.hasAttribute('data-i18n-placeholder')) {
                    el.setAttribute('placeholder', value);
                } else {
                    el.value = value;
                }
            } else {
                el.textContent = value;
            }
        });

        document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
            el.setAttribute('placeholder', t(el.getAttribute('data-i18n-placeholder')));
        });

        document.querySelectorAll('option[data-i18n]').forEach(function (el) {
            el.textContent = t(el.getAttribute('data-i18n'));
        });

        setupDatepickerRegional();

        if (window.FlightSearchDate) {
            window.FlightSearchDate.refreshPlaceholders();
        }

        if (typeof window.updatePassengerSummary === 'function') {
            window.updatePassengerSummary();
        }
    }

    function reapply() {
        var lang = readLanguageFromCookie() || currentLang || 'en';
        apply(lang);
    }

    window.SiteI18n = {
        t: t,
        apply: apply,
        reapply: reapply,
        setupDatepicker: setupDatepickerRegional,
        getLanguage: function () { return currentLang; }
    };

    window.refreshPageI18n = reapply;

    window.FlightSearchDate = {
        isEmpty: function (val) {
            if (!val) return true;
            var trimmed = String(val).trim();
            return trimmed === '' || trimmed === '+ Add Return' || trimmed === '+ Agregar regreso';
        },
        setEmpty: function () {
            var $el = $('#txtToDate');
            if (!$el.length) return;
            $el.val('');
            $el.attr('placeholder', t('search.addReturn'));
        },
        refreshPlaceholders: function () {
            $('#txtFromDate, #txtToDate, #txtFromDate1, #txtFromDate2, #txtFromDate3, #txtFromDate4').each(function () {
                if (!$(this).val()) {
                    var key = this.id === 'txtToDate' ? 'search.addReturn' : 'search.addDate';
                    $(this).attr('placeholder', t(key));
                }
            });
            $('#flight_from, #flight_to').attr('placeholder', t('search.cityPlaceholder'));
        }
    };

    function initSiteI18n() {
        var prefs = window.__localePrefs || {};
        var lang = prefs.language || readLanguageFromCookie() || 'en';
        if (prefs.currency == null && readCurrencyFromCookie()) {
            prefs.currency = readCurrencyFromCookie();
        }
        window.__localePrefs = {
            language: lang,
            currency: prefs.currency || 'USD',
            sitePath: prefs.sitePath || '/'
        };
        apply(lang);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSiteI18n);
    } else {
        initSiteI18n();
    }
})(window, window.jQuery);
