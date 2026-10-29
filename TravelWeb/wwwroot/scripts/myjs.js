$(function () {
    if (window.SiteI18n) {
        SiteI18n.setupDatepicker();
    }

    function i18nAlert(key, fallback) {
        alert(window.SiteI18n ? SiteI18n.t(key) : fallback);
    }

    function datepickerLayout() {
        setTimeout(function () {
            $('#ui-datepicker-div').addClass('notranslate');
        }, 0);
        if ($(window).width() < 768) {
            return { numberOfMonths: 1 };
        }
        return { numberOfMonths: 2 };
    }

    function isReturnDateEmpty() {
        var val = $("#txtToDate").val();
        return window.FlightSearchDate ? FlightSearchDate.isEmpty(val) : (!val || val === "+ Add Return");
    }

    function setReturnDateEmpty() {
        if (window.FlightSearchDate) {
            FlightSearchDate.setEmpty();
        } else {
            $("#txtToDate").val("+ Add Return");
        }
    }
    $('#pass_info').click(function (e) {
        e.stopPropagation();


    });

    $("#prsn_done").click(function () {
        $("#FlySearchPassengers").slideUp("slow");
    });
    $('body').click(function () {
    });

    $('#FlySearchPassengers').click(function (e) {   // Change Done by Himanshu
        e.stopPropagation();
    });

    $(".inc_der").click(function () {
        var fld = $(this).attr("data-field");
        var fld_opt = $(this).attr("data-opt");
        var adt = parseInt($("#flight_adult").val());
        var chd = parseInt($("#flight_child").val());
        var inf = parseInt($("#flight_infant").val());
        if (fld == "adult") {
            if (fld_opt == "+") {
                if ((adt + chd) < 9) {
                    var totadlt = adt + 1;
                    $("#flight_adult").val(totadlt);
                }
            }
            else if (fld_opt == "-") {
                if (adt > 1) {
                    $("#flight_adult").val(adt - 1);
                }
            }
        }
        else if (fld == "child") {
            if (fld_opt == "+") {
                if ((adt + chd) < 9) {
                    $("#flight_child").val(chd + 1);
                }
            }
            else if (fld_opt == "-") {
                if (chd > 0) {
                    $("#flight_child").val(chd - 1);
                }
            }
        }
        else if (fld == "infant") {
            if (fld_opt == "+") {
                if (adt > inf) {
                    $("#flight_infant").val(inf + 1);
                }
            }
            else if (fld_opt == "-") {
                if (inf > 0) {
                    $("#flight_infant").val(inf - 1);
                }
            }
        }

        getPassenger();
    });

    $("#flight_class_select").change(function () {
        getPassenger();
    });
    function getPassenger() {
        var str = (parseInt($("#flight_adult").val()) + parseInt($("#flight_child").val()) + parseInt($("#flight_infant").val())) + " Pax";

        var chk_cls = $('#flight_class_select').val();

        if (chk_cls == "1") {
            str += ", First";
        }
        else if (chk_cls == "2") {
            str += ", Business";
        }
        else if (chk_cls == "3") {
            str += ", Premimum";
        }
        else {
            str += ", Economy";
        }

        $("#pass_info").val(str);
    }
  

    $("#flight_submit").click(function () { debugger
     
        $("#FlySearchPassengers").slideUp("slow");
        if ($("#flight_from").val().trim(' ') == "") {
            alert("Please enter your departure airport");
            $("#flight_from").focus();
            return false;
        }
        else if ($("#flight_from").val().trim(' ').indexOf('-') != 3) {
            alert("Kindly select departure airport from auto suggest list!");
            $("#flight_from").focus();
            return false;
        }
        else if ($("#flight_to").val().trim(' ') == "") {
            alert("Please enter your destination airport");
            $("#flight_to").focus();
            return false;
        }
        else if ($("#flight_to").val().trim(' ').indexOf('-') != 3) {
            alert("Kindly select destination airport from auto suggest list!");
            $("#flight_to").focus();
            return false;
        }
        else if ($("#flight_to").val().trim(' ') == $("#flight_from").val().trim(' ')) {
            alert("Departure Airport and Destination Airport can't be same!!");
            $("#flight_to").focus();
            return false;
        }
        else if ($("#txtFromDate").val().trim('') == "dd-mm-yyyy" || $("#txtFromDate").val().trim('') == "") {
            i18nAlert('search.enterDepart', "Please enter your Depart Date");
            $("#txtFromDate").focus();
            return false;
        } else if ($('input[name="trip"]:checked').val() === 'RoundTrip' && isReturnDateEmpty()) {
            i18nAlert('search.enterReturn', "Please enter your Return Date");
            $("#txtToDate").focus();
            return false;
        } else {
            $('#hdntxtToDate').val($("#txtToDate").val());

            //$("#search_popup").show();
            var fway = 2;
            if (isReturnDateEmpty() || $('input[name="trip"]:checked').val() === 'OneWay') {
                fway = 1;
                $('#hdntxtToDate').val("01-01-1900");
                setReturnDateEmpty();
            }

            var fcls = $('#flight_class_select').val();
            if (fcls == '4') {
                cabin = "Y";
            }
            else if (fcls == '2') {
                cabin = "B";
            }
            else if (fcls == '1') {
                cabin = "F";
            }
            else if (fcls == '3') {
                cabin = "P";
            }
            var from = "" + $("#flight_from").val().split("-")[0];
            var to = "" + $("#flight_to").val().split("-")[0];
            var fromDate = "" + $("#txtFromDate").val().split("-")[2] + "-" + $("#txtFromDate").val().split("-")[0] + "-" + $("#txtFromDate").val().split("-")[1];
            
            var toDate = "";

            toDate = "" + $("#hdntxtToDate").val().split("-")[2] + "-" + $("#hdntxtToDate").val().split("-")[0] + "-" + $("#hdntxtToDate").val().split("-")[1];
            if (fway == "2") {
                toDate = "" + $("#hdntxtToDate").val().split("-")[2] + "-" + $("#hdntxtToDate").val().split("-")[0] + "-" + $("#hdntxtToDate").val().split("-")[1];
            }
            var round = ""
            if (fway == "2") {
                round = document.getElementById("RoundTrip").value
                round = "round";

            }
            else {
                round = document.getElementById("OneWay").value
            }
            localStorage.removeItem("moreadultcount");
            localStorage.removeItem("morechildcount");

            var adult = "" + $("#flight_adult").val();
            var child = "" + $("#flight_child").val();

            var moreadultcount = $("#flight_adult").val();
            moreadultcount = parseInt(moreadultcount);

            var morechildcount = $("#flight_child").val();
            morechildcount = parseInt(morechildcount);
            localStorage.setItem("morechildcount", morechildcount);
            localStorage.setItem("moreadultcount", moreadultcount);
            var currency = document.querySelector("[data-selected-currency]").innerText.trim();
            var infant = "" + $("#flight_infant").val();
            var airline = "";

            var isdirect = "false";
            fway_s = fway;
            fcls_s = 0;
            var ch = "RushMyFlight"
            var utm_campaign = "promotional";
            var utm_source = "";
            var url_string = window.location.href;
            var url = new URL(url_string);
            var c = url.searchParams.get("utm_source");
            if (c == "" || c==null) {
                utm_source = "";
            }
            else {
                utm_source = c;
            }
            var utm_medium = "cpc";
            //var cabin = "y";
            var af = "bdfqV.kbQjUML5VDRxLrOg--"

            var str = "/result?depart=" + from + "&arrival=" + to + "&trip=" + round + "&departOn[]=" + fromDate + "&af=" + af + "&ch=" + ch + "&adult=" + adult + "&child=" + child + "&infant=" + infant + "&seniors=" + fcls_s + "&cabin=" + cabin + "&returnOn=" + toDate + "&utm_source=" + utm_source + "&utm_medium=" + utm_medium + "&utm_campaign=" + utm_campaign+ "&currency=" + currency +" ";
                window.location = str;
          /*  }*/
            
        }
    });


    $('#flight_from').keydown(function () {
    });
    var selectFrom = false;
    var selectTo = false;
    var airportSuggestXhr = null;

    function mapAirportSuggestions(data) {
        return $.map(data || [], function (item) {
            return {
                value: item.code,
                label: (item.code + "-" + item.name + ", " + item.cityName)
            };
        });
    }

    function airportAutocompleteSource(request, response) {
        var term = (request.term || "").trim();
        if (term.length < 2) {
            response([]);
            return;
        }
        if (airportSuggestXhr && airportSuggestXhr.readyState !== 4) {
            airportSuggestXhr.abort();
        }
        airportSuggestXhr = $.ajax({
            url: "/result/getairport",
            dataType: "json",
            data: { pickUpLocation: term },
            success: function (data) {
                response(mapAirportSuggestions(data));
            },
            error: function (xhr) {
                if (xhr.statusText !== "abort") {
                    response([]);
                }
            }
        });
    }

    // Bind once per field (avoid double-binding #flight_from which also has .inputvalue)
    var $airportFields = $("#flight_from, #flight_to, #hotel_place");
    $airportFields.each(function () {
        var $el = $(this);
        if ($el.hasClass("ui-autocomplete-input")) {
            try { $el.autocomplete("destroy"); } catch (e) { /* ignore */ }
        }
    });

    $("#flight_from").autocomplete({
        source: airportAutocompleteSource,
        open: function () { selectFrom = false; },
        select: function (event, ui) {
            $(this).val(ui.item.label);
            var toEl = document.getElementById("flight_to");
            if (toEl) toEl.focus();
            selectFrom = false;
            return false;
        },
        close: function () { selectFrom = false; },
        minLength: 2,
        delay: 150,
        autoFocus: true
    }).blur(function () {
        if (selectFrom) {
            $("#flight_from").val($("ul.ui-autocomplete li:first a").text());
        }
    }).focus(function () {
        var val = ($(this).val() || "").trim();
        if (val.length >= 2) {
            $(this).autocomplete("search", val);
        }
    });

    $("#flight_to, #hotel_place").autocomplete({
        source: airportAutocompleteSource,
        open: function () {
            selectTo = $(this).is("#flight_to");
        },
        select: function (event, ui) {
            $(this).val(ui.item.label);
            selectTo = false;
            return false;
        },
        close: function () { selectTo = false; },
        minLength: 2,
        delay: 150,
        autoFocus: true
    }).blur(function () {
        if (selectTo && $(this).is("#flight_to")) {
            $("#flight_to").val($("ul.ui-autocomplete li:first a").text());
        }
    }).focus(function () {
        var val = ($(this).val() || "").trim();
        if (val.length >= 2) {
            $(this).autocomplete("search", val);
        }
    });

    // Warm server cache in background so first typed letter is snappy
    $.ajax({
        url: "/result/getairport",
        dataType: "json",
        data: { pickUpLocation: "Lon" },
        cache: true
    });
    function Add7Days() {
        var date = $("#txtFromDate").val();

        var ndate = DateFromString(date);
        var split = ndate.split("-");
        if (parseInt(split[0]) < 10)
            split[0] = "0" + split[0];

        ndate = split[0] + "-" + split[1] + "-" + split[2];

        return ndate;
    }
    function DateFromString(str) {
        str = str.split(/\D+/);
        str = new Date(str[2], str[0] - 1, (parseInt(str[1]) + 1));

        return MMDDYYYY(str);
    }
    function MMDDYYYY(str) {
        var ndateArr = str.toString().split(' ');
        var Months = 'Jan Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec';
        return (parseInt(Months.indexOf(ndateArr[1]) / 4) + 1) + '-' + ndateArr[2] + '-' + ndateArr[3];
    }

    function Getvalue() {
        if ($(window).width() < 1024) {
            return 1;
        }
        else {
            return 2;
        }
    }
    function onewaytodate() {
        $("#txtToDate").prop('disabled', true);
        txtToDate.style.background = '#D8d8D8';
    }
    $("#txtFromDate").datepicker({
        minDate: 0,
        dateFormat: 'mm-dd-yy',
        beforeShow: datepickerLayout,

        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);

            if (($('input[name="trip"]:checked').val() == "RoundTrip")) {
                var TDate = Add7Days();
                $("#txtToDate").val(TDate);
                $("#txtToDate").datepicker("option", "minDate", dt);
            }   
            else if (($('input[name="trip"]:checked').val() == "Multiselect")) {
                var TDate = Add7Days();
                $("#txtFromDate1").val(TDate);
                
                $("#txtFromDate1").datepicker("option", "minDate", dt);

            }
            else {
                setReturnDateEmpty();
            }
           

        },

        onClose: function () {
            if (($('input[name="trip"]:checked').val() == "RoundTrip")) {

                $("#txtToDate").datepicker("show");

            }
        }
      
    });

   

        $("#txtToDate").datepicker({

        minDate: 0,
        dateFormat: 'mm-dd-yy',
        beforeShow: datepickerLayout,
       // numberOfMonths: 2,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);

            var TDate = Add7Days();
            $("#txtFromDate1").datepicker("option", "minDate", dt);
            $('.closereturn').hide();
        }
    });

    $("#txtFromDate1").datepicker({

        minDate: 0,
        dateFormat: 'mm-dd-yy',
        beforeShow: datepickerLayout,
        // numberOfMonths: 2,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);

            var TDate = Add7Days();
            $("#txtFromDate2").datepicker("option", "minDate", dt);
            $('.closereturn').hide();
        }
    });

    $("#txtFromDate2").datepicker({
        

        minDate: 0,
        dateFormat: 'mm-dd-yy',
        beforeShow: datepickerLayout,
        // numberOfMonths: 2,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);

            var TDate = Add7Days();
            $("#txtFromDate3").val(TDate);
            $("#txtFromDate3").datepicker("option", "minDate", dt);
            $('.closereturn').hide();
        }
    });
   
    $("#txtFromDate3").datepicker({

        minDate: 0,
        dateFormat: 'mm-dd-yy',
        beforeShow: datepickerLayout,
        // numberOfMonths: 2,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);

            var TDate = Add7Days();
            $("#txtFromDate4").val(TDate);
            $("#txtFromDate4").datepicker("option", "minDate", dt);
            $('.closereturn').hide();
        }
    });
   

    $("#txtFromDate4").datepicker({

        minDate: 0,
        dateFormat: 'mm-dd-yy',
        beforeShow: datepickerLayout,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);
            
            
            $('.closereturn').hide();
        }
       
    });

    function isDate(txtDate) {
        var currVal = txtDate;
        if (currVal == '')
            return false;
        var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
        var dtArray = currVal.match(rxDatePattern);

        if (dtArray == null)
            return false;

        dtMonth = dtArray[1];
        dtDay = dtArray[3];
        dtYear = dtArray[5];

        if (dtMonth < 1 || dtMonth > 12)
            return false;
        else if (dtDay < 1 || dtDay > 31)
            return false;
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
            return false;
        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap))
                return false;
        }
        return true;
    }


    function vali(txtToDate) {
        var s = $('#' + txtToDate).val();

        if (isDate(s) == false) {
            $('#' + txtToDate).focus();

        } else {
            $('#' + txtToDate).val(s);
        }
    }

});


//Change border color on text change
//function changeBorderColor(idValue, id) {
//    if (id == 'txtphone' || id == 'txtEmail') {
//            document.getElementById('isvalid1').innerHTML = "";
//    }
//    if (id == id) {
//        document.getElementById(id).style = 'border-color:green;'
//    } else {
//        document.getElementById(id).style = 'border-color:#007bff;'
//    }
//    if (idValue == "0") {
//        document.getElementById(id).style = 'border-color:#007bff;'
//    }
//}

function changeBorderColor(idValue, id) {

    if (id === 'txtphone') {

        var regex = /^[0-9]*$/;

        if (!regex.test(idValue)) {
            alert("Please enter only 10 digit numeric phone number.");
            document.getElementById(id).value = idValue.replace(/[^0-9]/g, '');
            document.getElementById(id).style.borderColor = '#ED4214';
            return false;
        }

        //if (idValue.length > 0 && idValue.length !== 10) {
        //    alert("Phone number must be exactly 10 digits.");
        //    document.getElementById(id).style.borderColor = '#ED4214';
        //    return false;
        //}
    }

    if (id === 'txtphone' || id === 'txtEmail') {
        document.getElementById('isvalid1').innerHTML = "";
    }

    if (idValue !== "" && idValue !== "0") {
        document.getElementById(id).style.borderColor = 'green';
    } else {
        document.getElementById(id).style.borderColor = '#007bff';
    }
}


function genderFieldBorder(idValue, id) {
    if (id == id && idValue!=0) {
        document.getElementById(id).style = 'border-color:green;'
    } else {
        document.getElementById(id).style = 'border-color:#007bff;'
    }

}

function formVal(idValue, id)
 {
    var IdValue = idValue.trim();
    var value = id;
    var emailCheck = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var emailValid = emailCheck.test(IdValue);
        
    if (IdValue == "")
    {
        document.getElementById(value).style ='border-color:#007bff;'
        
    } else if (id == 'txtphone' && IdValue.length <= 10)
    {
       
        document.getElementById(value).style = 'border-color:#007bff;'
        
    } else if (id == 'txtCardNumber' && IdValue.length <= 14)
    {       
       
        document.getElementById(value).style = 'border-color:#007bff;'

    } else if (id == 'txtEmail' && emailValid == false)
    {
       
        document.getElementById(value).style = 'border-color:#007bff;'
    }
    else if (id == value && IdValue == "0")
    {
       
        document.getElementById(value).style = 'border-color:#007bff;'
    }
    else if (id == 'txtCardSecurity' && IdValue.length <= 2)
    {
       
        document.getElementById(value).style = 'border-color:#007bff;'
    }
    else
    {
       
        document.getElementById(value).style = 'border-color:green;'
    }
}

function newAmount() {
    var checkboxValue = document.getElementById('select-main-cabin').value;
    var seatPrice = document.querySelector("#cabin-seat-price");
    var seatPrice2 = document.querySelector("#cabin-seat-price2");

    if (checkboxValue == '1') {
        seatPrice.classList.add('show');
        seatPrice2.classList.add('show');

        
        var hidePreviousAmt = document.querySelector('#previousSubTotal1');
        hidePreviousAmt.classList.remove('show');
        hidePreviousAmt.style.display='none';
        var hidePreviousAmt2 = document.querySelector('#previousSubTotal2');
        hidePreviousAmt2.classList.remove('show');
        hidePreviousAmt2.style.display='none';

        var showNewAmt = document.querySelector('#totalWithPref1');
        showNewAmt.classList.add('show');

        var showNewAmt2 = document.querySelector('#totalWithPref2');
      
        showNewAmt2.classList.add('show');


        var valueInParagraph = document.querySelector('#chargeText1');
        valueInParagraph.classList.remove('show');
        valueInParagraph.style.display = 'none';

        var newValueInParagraph = document.querySelector('#newChargesText1');
        newValueInParagraph.classList.add('show');


        var valueInParagraph2 = document.querySelector('#chargeText2');
        valueInParagraph2.classList.remove('show');
        valueInParagraph2.style.display = 'none';

        var newValueInParagraph2 = document.querySelector('#newChargesText2');
        newValueInParagraph2.classList.add('show');

        var TotalAmountWithPreference = document.getElementById('TotalAmountWithPreference').value;
        var subtotal = document.getElementById('subTotal').textContent = TotalAmountWithPreference;

        var checkboxValue = document.getElementById('select-main-cabin').value = '0';
    }
    else if (checkboxValue == '0')
    {
        seatPrice.classList.remove('show');
        seatPrice2.classList.remove('show');

        var showNewAmt = document.querySelector('#totalWithPref1');
        showNewAmt.classList.remove('show');

        var hidePreviousAmt = document.querySelector("#previousSubTotal1");
        hidePreviousAmt.classList.add('show');
        
        var showNewAmt2 = document.querySelector('#totalWithPref2');
        showNewAmt2.classList.remove('show');

        var hidePreviousAmt2 = document.querySelector("#previousSubTotal2");
        hidePreviousAmt2.classList.add('show');

        var newValueInParagraph = document.querySelector('#newChargesText1');
        newValueInParagraph.classList.remove('show');

        var valueInParagraph = document.querySelector('#chargeText1');
        valueInParagraph.classList.add('show');

        var newValueInParagraph2 = document.querySelector('#newChargesText2');
        newValueInParagraph2.classList.remove('show');

        var valueInParagraph2 = document.querySelector('#chargeText2');
        valueInParagraph2.classList.add('show');
             

        var subtotalAmt = document.getElementById('subTotalAmt').value;
        var subTotal = document.getElementById('subTotal').textContent = subtotalAmt

        var checkboxValue = document.getElementById('select-main-cabin').value = '1';

    }
}



    function postFlight(depart, arrival, trip, departDate, returnDate, adult, child, infant, seniors, cabin) {

        departDate = departDate || $("#txtFromDate").val();
    returnDate = returnDate || $("#txtToDate").val();

    $("#formDepart").val(depart);
    $("#formArrival").val(arrival);
    $("#formTrip").val(trip);
    $("#formDepartDate").val(departDate);
    $("#formReturnDate").val(returnDate);
    $("#formAdult").val(adult || 1);
    $("#formChild").val(child || 0);
    $("#formInfant").val(infant || 0);
    $("#formSeniors").val(seniors || 0);
    $("#formCabin").val(cabin || "Y");

    $("#flightPostForm").submit();
}



function toggleMobileFilters() {
    document.querySelector('.sidebar-search-results').classList.toggle('active');
    document.querySelector('.sidebar-overlay').classList.toggle('active');
    document.body.classList.toggle('sidebar-open');
}
function closeMobileFilters() {
    document.querySelector('.sidebar-search-results').classList.remove('active');
    document.querySelector('.sidebar-overlay').classList.remove('active');
    document.body.classList.remove('sidebar-open');
}

document.addEventListener('DOMContentLoaded', () => {

    const amendBtn = document.getElementById('amendSearchBtn');
    const searchForm = document.getElementById('Search-Section');

    if (amendBtn && searchForm) {

        amendBtn.addEventListener('click', () => {

            searchForm.classList.toggle('opened');

            amendBtn.textContent =
                searchForm.classList.contains('opened')
                    ? 'Hide Search'
                    : 'Amend Search';

        });

    }

});