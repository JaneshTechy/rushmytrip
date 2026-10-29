(function ($) {
    // Messagebox
    var ExistMsg = 0,
        SmartMSGboxCount = 0,
        PrevTop = 0;

    OpenSmartBox = function (settings, callback) {
        var SmartMSG, Content;
        settings = $.extend({
            title: "",
            content: "",
            NormalButton: undefined,
            ActiveButton: undefined,
            buttons: undefined,
            input: undefined,
            inputValue: undefined,
            placeholder: "",
            options: undefined
        }, settings);

        SmartMSGboxCount = SmartMSGboxCount + 1;

        if (ExistMsg == 0) {
            ExistMsg = 1;
            SmartMSG = "<div class='divMessageBox animated fadeIn fast' id='MsgBoxBack'></div>";
            $("body").append(SmartMSG);
        }

        var InputType = "";
        var HasInput = 0;
        var HasTimer = false;
        if (settings.input != undefined) {
            HasInput = 1;
            settings.input = settings.input.toLowerCase();

            switch (settings.input) {
                case "text":
                    settings.inputValue = $.type(settings.inputValue) === 'string' ? settings.inputValue.replace(/'/g, "&#x27;") : settings.inputValue;
                    InputType = "<input class='form-control' type='" + settings.input + "' id='txt" +
                        SmartMSGboxCount + "' placeholder='" + settings.placeholder + "' value='" + settings.inputValue + "'/><br/><br/>";
                    break;
                case "password":
                    InputType = "<input class='form-control' type='" + settings.input + "' id='txt" +
                        SmartMSGboxCount + "' placeholder='" + settings.placeholder + "'/><br/><br/>";
                    break;
                case "timer":
                    InputType = "<span class='form-control' id='fingerprint-timer'></span><br/><br/>";
                    HasTimer = true;
                    break;
                case "select":
                    var selectedid = 0;
                    if (settings.optionselected != undefined && settings.optionselected != null) {
                        selectedid = settings.optionselected;
                    }
                    if (settings.options == undefined) {
                        alert("For this type of input, the options parameter is required.");
                    } else {
                        InputType = "<select class='form-control' id='txt" + SmartMSGboxCount + "'>";
                        for (var i = 0; i <= settings.options.length - 1; i++) {
                            if (settings.options[i] == "[") {
                                Name = "";
                            } else {
                                if (settings.options[i] == "]") {
                                    NumBottons = NumBottons + 1;
                                    var keyvalue = Name.split(",");
                                    if(keyvalue[1] == selectedid)
                                        Name = "<option selected='true' value='" + keyvalue[1] + "'>" + keyvalue[0] + "</option>";
                                    else
                                        Name = "<option value='" + keyvalue[1] + "'>" + keyvalue[0] + "</option>";
                                    InputType += Name;
                                } else {
                                    Name += settings.options[i];
                                }
                            }
                        };
                        InputType += "</select>"
                    }

                    break;
                case "file":
                    var display = "block";
                    if (settings.inputdisplay != undefined && settings.inputdisplay != null)
                        display = settings.inputdisplay;
                    InputType = "<input onchange='" + settings.inputfileonchange + "(this);' style='display:" + display + "' class='form-control' type='" + settings.input + "' id='txt" +
                        SmartMSGboxCount + "' placeholder='" + settings.placeholder + "'/><br/><br/>";
                    break;
                default:
                    alert("That type of input is not handled yet");
            }

        }

        Content = "<div class='MessageBoxContainer animated fadeIn fast' id='Msg-" + SmartMSGboxCount +
            "'>";
        Content += "<div class='MessageBoxMiddle'>";
        if (settings.titleicon != "undefined" && settings.titleicon != null)
            Content += '<i class="' + settings.titleicon + '"></i>';
        Content += "<span class='MsgTitle'>" + settings.title + "</span class='MsgTitle'>";
        Content += "<p class='pText'>" + settings.content + "</p>";
        Content += InputType;
        Content += "<div class='MessageBoxButtonSection'>";
        if (settings.buttons == undefined || settings.buttons == null) {
            //Fade the message with some timeout
            var timeout = 1000;
            if (settings.timeout != undefined && settings.timeout != null)
                timeout = settings.timeout;
            setTimeout(function () { CloseSmartBox(); }, timeout);
        } else if (typeof (settings.buttons) === "object" && settings.buttons.length !== undefined) {
            settings.buttons = settings.buttons.join(',');
        }

        settings.buttons = $.trim(settings.buttons);
        settings.buttons = settings.buttons.split('');

        var Name = "";
        var NumBottons = 0;
        if (settings.NormalButton == undefined) {
            settings.NormalButton = "#232323";
        }

        if (settings.ActiveButton == undefined) {
            settings.ActiveButton = "#ed145b";
        }

        for (var i = 0; i <= settings.buttons.length - 1; i++) {

            if (settings.buttons[i] == "[") {
                Name = "";
            } else {
                if (settings.buttons[i] == "]") {
                    NumBottons = NumBottons + 1;
                    Name = "<button id='bot" + NumBottons + "-Msg-" + SmartMSGboxCount +
                        "' class='btn btn-default btn-sm botTempo'> " + Name + "</button>";
                    Content += Name;
                } else {
                    Name += settings.buttons[i];
                }
            }
        };

        Content += "</div>";
        //MessageBoxButtonSection
        Content += "</div>";
        //MessageBoxMiddle
        Content += "</div>";
        //MessageBoxContainer

        // alert(SmartMSGboxCount);
        if (SmartMSGboxCount > 1) {
            $(".MessageBoxContainer").hide();
            $(".MessageBoxContainer").css("z-index", 99999);
        }

        $(".divMessageBox").append(Content);
        // Focus
        if (HasInput == 1) {
            $("#txt" + SmartMSGboxCount).focus();
        }
        if (HasTimer)
        {
            HasTimer = false;
            var interval = settings.interval;
            var date = getcustomdate(new Date());
            date = common.addminute(date, interval);
            var func = settings.onfinish;
            common.timer($('#fingerprint-timer'), date, func, settings.span, "<i class='fa fa-lg fa-qrcode fa-lg'></i>&nbsp;<span class='lng-timeup'>Time's up<span>!");
        }

        $('.botTempo').hover(function () {
            var ThisID = $(this).attr('id');
            // alert(ThisID);
            // $("#"+ThisID).css("background-color", settings.ActiveButton);
        }, function () {
            var ThisID = $(this).attr('id');
            //$("#"+ThisID).css("background-color", settings.NormalButton);
        });

        // Callback and button Pressed
        $(".botTempo").click(function () {
            // Closing Method
            var ThisID = $(this).attr('id');
            var MsgBoxID = ThisID.substr(ThisID.indexOf("-") + 1);
            var Press = $.trim($(this).text());

            var close = true;
            if (HasInput == 1) {
                if (typeof callback == "function") {
                    var IDNumber = MsgBoxID.replace("Msg", "");
                    var Value = $("#txt" + IDNumber).val();
                    if (callback)
                        callback(Press, Value);
                }
            } else {
                if (typeof callback == "function") {
                    if (callback)
                        callback(Press);
                }
            }
        });
    }

    CloseSmartBox = function (settings, callback) {
        if (SmartMSGboxCount > 0) {
            var MsgBoxID = "Msg-" + SmartMSGboxCount;

            $("#" + MsgBoxID).addClass("animated fadeOut fast");
            SmartMSGboxCount = SmartMSGboxCount - 1;

            if (SmartMSGboxCount == 0) {
                $("#MsgBoxBack").removeClass("fadeIn").addClass("fadeOut").delay(300).queue(function () {
                    ExistMsg = 0;
                    $(this).remove();
                });
            }
        }
    }
})(jQuery);