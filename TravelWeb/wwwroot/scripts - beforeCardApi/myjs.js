$(function () {

    $('#pass_info').click(function (e) {
        //  $('#FlySearchPassengers').slideToggle();
        //e.preventDefault();
        e.stopPropagation();
    });

    $("#prsn_done").click(function () {
        $("#FlySearchPassengers").slideUp("slow");
    });
    $('body').click(function () {
        // $("#FlySearchPassengers").slideUp("slow");
    });
    $('.prp_popup').click(function (e) {
        e.stopPropagation();
    });

    $(".inc_der").click(function () {
        var fld = $(this).attr("data-field");
        var fld_opt = $(this).attr("data-opt");
        var adt = parseInt($("#flight_adult").val());
        var chd = parseInt($("#flight_child").val());
        var inf = parseInt($("#flight_infant").val());
        debugger;
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

    $("input[name=flight_class]").change(function () {
        getPassenger();
    });
    function getPassenger() {
        var str = (parseInt($("#flight_adult").val()) + parseInt($("#flight_child").val()) + parseInt($("#flight_infant").val())) + " Pax";
        var chk_cls = $('input[name=flight_class]:checked').val();

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




    $("#flight_submit").click(function () {
        debugger;
        // alert(67575);
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
        else if ($("#txtFromDate").val().trim('') == "") {
            alert("Please enter your Depart Date");
            $("#txtFromDate").focus();
            return false;
        }
        else {

            $('#hdntxtToDate').val($("#txtToDate").val());

            //$("#search_popup").show();
            var fway = 2;
            if ($("#txtToDate").val() == "+ Add Return") {
                fway = 1;
                $('#hdntxtToDate').val("01-01-1900");
                $("#txtToDate").val("+ Add Return");
            }

            //if (isDate($("#txtFromDate").val())) {
            //    alert("Please enter valid Depart Date");
            //    $("#txtFromDate").focus(); return false;
            //}

            //if (isDate($("#txtToDate").val())) {
            //    alert("Please enter valid Return Date");
            //    $("#txtToDate").focus(); return false;
            //}

            //var fway = 2;//$('input[name=flight_way]:checked').val();
            //  var fcls = $('#flight_class').val();  //$('input[name=flight_class]:checked').val();
            var fcls = $('input[name=flight_class]:checked').val();
            var from = "" + $("#flight_from").val().split("-")[0];
            var to = "" + $("#flight_to").val().split("-")[0];
            var fromDate = "" + $("#txtFromDate").val().split("-")[2] + "-" + $("#txtFromDate").val().split("-")[0] + "-" + $("#txtFromDate").val().split("-")[1];
            var toDate = "";

            toDate = "" + $("#hdntxtToDate").val().split("-")[2] + "-" + $("#hdntxtToDate").val().split("-")[0] + "-" + $("#hdntxtToDate").val().split("-")[1];
            if (fway == "2") {
                toDate = "" + $("#hdntxtToDate").val().split("-")[2] + "-" + $("#hdntxtToDate").val().split("-")[0] + "-" + $("#hdntxtToDate").val().split("-")[1];
            }

            var adult = "" + $("#flight_adult").val();
            var child = "" + $("#flight_child").val();
            var infant = "" + $("#flight_infant").val();
            var airline = "";
            //" + $("#flight_airline").val();
            var isdirect = "false";
            fway_s = fway;
            fcls_s = fcls;


            var str = "/result?qs=HOME|Home|" + from + "|" + to + "|" + fromDate + "|" + toDate + "|" + adult + "|" + child + "|" + infant + "|" + fway_s + "|" + fcls_s + "|false";


            window.location = str;
        }
    });


    $('#flight_from').keydown(function () {
        //  $('#flight_from').val('');
    });
    var selectFrom = false;
    var selectTo = false;

    $('#flight_from').autocomplete({
        source: function (request, response) {
            debugger;
            $.ajax({
                url: "/result/getairport",
                dataType: "json",
                data: {
                    pickUpLocation: request.term
                },
                success: function (data) {
                    // alert(data);
                    response($.map(data, function (item) {
                        return {
                            value: item.code,
                            label: (item.code + "-" + item.name + ", " + item.cityName)
                        }
                    }));
                }
            })
        },
        open: function (event, ui) { selectFrom = true; },
        select: function (event, ui) {
            $(this).val(ui.item.label);
            document.getElementById('flight_to').focus();
            selectFrom = false;
            return false;
        },
        close: function (event, ui) {
            selectFrom = false;
        },
        minLength: 3,
        autoFocus: true
    }).blur(function () {
        if (selectFrom) {
            $("#flight_from").val($('ul.ui-autocomplete li:first a').text());
        }
    }).focus(function () {
        $(this).autocomplete("search");
    });

    $("#flight_to").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/result/getairport",
                dataType: "json",
                data: {
                    pickUpLocation: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            value: item.code,
                            label: (item.code + "-" + item.name + ", " + item.cityName)
                        }
                    }));
                }
            })
        },
        open: function (event, ui) {
            selectTo = true;
        },
        select: function (event, ui) {
            $(this).val(ui.item.label);
            selectTo = false;
            return false;
        },
        close: function (event, ui) {
            selectTo = false;
        },
        minLength: 3,
        autoFocus: true
    }).blur(function () {
        if (selectTo) {
            $("#flight_to").val($('ul.ui-autocomplete li:first a').text());
        }
    }).focus(function () {
        $(this).autocomplete("search");
    });




    // calander double from date to date


    //$("#txtFromDate").datepicker({
    //   startDate: '-0m',
    //    minDate: 0,
    //    dateFormat: 'mm-dd-yy',
    //    numberOfMonths: 2,
    //    onSelect: function (selected) {
    //        var dt = new Date(selected);
    //        dt.setDate(dt.getDate() + 1);
    //        //$("#txtToDate").datepicker("option", "minDate", dt);
    //    }
    //});
    //$("#txtToDate").datepicker({
    //    startDate: '-0m',
    //    minDate: 0,
    //    dateFormat: 'mm-dd-yy',
    //    numberOfMonths: 2,
    //    onSelect: function (selected) {
    //        var dt = new Date(selected);
    //        dt.setDate(dt.getDate());
    //        $("#txtFromDate").datepicker("option", "maxDate", dt);
    //        $('.closereturn').show();
    //    }
    //});


    //$("#txtFromDate").datepicker({
    //    minDate: 0,
    //    dateFormat: 'mm-dd-yy',
    //    numberOfMonths: 2,
    //    onSelect: function (selected) {
    //        var dt = new Date(selected);
    //        dt.setDate(dt.getDate() + 1);
    //        // $("#txtToDate").datepicker("option", "minDate", dt);
    //        //  $("#txtToDate").val($("#txtFromDate").val());
    //        $("#txtToDate").datepicker("option", "minDate", dt);
    //        $("#txtToDate").val("+ Add Return");
           
    //    }
    //});
    //$("#txtToDate").datepicker({

    //    minDate: 0,
    //    dateFormat: 'mm-dd-yy',
        
    //    numberOfMonths: 2,
    //    onSelect: function (selected) {
    //        var dt = new Date(selected);
    //        dt.setDate(dt.getDate() + 1);
            
    //       // $("#txtFromDate").datepicker("option", "maxDate", dt);
    //        $('.closereturn').show();
    //    }
    //});
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
        //if (parseInt(str[0]) < 10)
        // str[0] = "0" + str[0];

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
    $("#txtFromDate").datepicker({
        minDate: 0,
        dateFormat: 'mm-dd-yy',
        beforeShow: function () {
            if ($(window).width() < 768) {
                return { numberOfMonths: 1 };
            } else {
                return { numberOfMonths: 2 };
            }
        },
       // numberOfMonths: 2,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);
            // $("#txtToDate").datepicker("option", "minDate", dt);
            // $("#txtToDate").val($("#txtFromDate").val());
            // var roundtrip = $("input[name='flight-options']:checked").val();
           
            if (($("input:radio[name='flight-options']:checked").val() == "RoundTrip")) {
                //if ($(this).is(':checked') && $(this).val() == 'RoundTrip') {
                var TDate = Add7Days();
                $("#txtToDate").val(TDate);
                //$("#txtToDate").focus();
                //$('#txtToDate').datepicker('show');
            }
            else {
                $("#txtToDate").val("+ Add Return");
            }

            //$("#txtToDate").datepicker("option", "minDate", dt);
           

        },

        onClose: function () {
            if (($("input:radio[name='flight-options']:checked").val() == "RoundTrip")) {



                $("#txtToDate").datepicker("show").load();

                //$("#txtToDate").focus();
                //$("#txtToDate").datepicker({

                //    mindate: 0,
                //    dateformat: 'mm-dd-yy',

                //    numberofmonths: 2,
                //    onselect: function (selected) {
                //        var dt = new date(selected);
                //        dt.setdate(dt.getdate() + 1);

                //        // $("#txtfromdate").datepicker("option", "maxdate", dt);
                //       // $('.closereturn').show();

                //    }
                //});
            }
        }
      
    });

        $("#txtToDate").datepicker({

        minDate: 0,
        dateFormat: 'mm-dd-yy',
        beforeShow: function () {
            if ($(window).width() < 768) {
                return { numberOfMonths: 1 };
            } else {
                return { numberOfMonths: 2 };
            }
        },
       // numberOfMonths: 2,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);

            // $("#txtFromDate").datepicker("option", "maxDate", dt);
            $('.closereturn').show();
        }
    });



    //    $("#txtFromDate").datepicker({
    //        numberOfMonths: 2, dateFormat: 'mm-dd-yy',
    //        onSelect: function (selected) {
    //            var dt = new Date(selected);
    //            dt.setDate(dt.getDate() + 1);
    //           // $("#txtTo").datepicker("option", "minDate", dt);
    //        }
    //    });
    //$("#txtToDate").datepicker({
    //    numberOfMonths: 2, dateFormat: 'mm-dd-yy',
    //        onSelect: function (selected) {
    //            var dt = new Date(selected);
    //            dt.setDate(dt.getDate() - 1);
    //            $("#txtFromDate").datepicker("option", "maxDate", dt);
    //        }
    //    });
   

   



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


    $('.closereturn').click(function () {
        $(this).hide();
        $("#txtToDate").val("+ Add Return");
    });
});





//Form-Validation for UI

function formVal(idValue, id)
 {
    var IdValue = idValue.trim();
    var value = id;
    var emailCheck = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var emailValid = emailCheck.test(IdValue);
    

    
    if (IdValue == "")
    {
       
        document.getElementById(value).style='border-color:red;'
        
    } else if (id == 'txtphone' && IdValue.length <= 9)
    {
       
        document.getElementById(value).style = 'border-color:red;'
        
    } else if (id == 'txtCardNumber' && IdValue.length <= 15)
    {       
       
        document.getElementById(value).style = 'border-color:red;'

    } else if (id == 'txtEmail' && emailValid == false)
    {
       
        document.getElementById(value).style = 'border-color:red;'
    }
    else if (id == value && IdValue == "0")
    {
       
        document.getElementById(value).style = 'border-color:red;'
    }
    //else if (id == value && IdValue == "(1) Jan")
    //{
    //   
    //    document.getElementById(value).style = 'border-color:red;'
    //}
    //else if (id == value && IdValue == "2022")
    //{
    //   
    //    document.getElementById(value).style = 'border-color:red;'
    //}
    else if (id == 'txtCardSecurity' && IdValue.length <= 2)
    {
       
        document.getElementById(value).style = 'border-color:red;'
    }
    else
    {
       
        document.getElementById(value).style = 'border-color:green;'
    }
}