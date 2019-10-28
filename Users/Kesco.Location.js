$(document).ready(function() {
    $(window).resize(function() {
        resize();
    });
    resize();
});

function resize() {
    var hr = $(window).height();
    var wr = $(window).width();

    if ($('#dialog_divLocationType').height() != null) {
        $('#dialog_divLocationType').height(hr - 6);
        $('#dialog_divLocationType').width(wr - 6);

        $('#divLocationType').height(hr - 6);
        $('#divLocationType').width(wr - 6);
        $('#ifrLocType').height(hr - 35);
        $('#ifrLocType').width(wr - 6);
    }

}

var location_pageId, location_loc1, location_loc2, location_user;
location_LocType.form = null;
var location_ifrIsLoaded = false;
function location_LocType(titleForm, pageId, loc1, loc2, user) {
    if (titleForm && titleForm != "") title = titleForm;

    if (pageId && pageId != "" && loc1 && loc1 != "" && loc2 && loc2 != "" && user && user != "") {
        location_pageId = pageId;
        location_loc1 = loc1;
        location_loc2 = loc2;
        location_user = user;
    } else {
        location_pageId = "";
        location_loc1 = "";
        location_loc2 = 0;
        location_user = "";
    }

    var idContainer = "divLocationType";
    var width = 957; var height = 551;

    if (null == location_LocType.form) {
        var onOpen = function () {
            if (!location_ifrIsLoaded) {
                $("#ifrLocType").attr('src', "LocationType.aspx?loc1=" + location_loc1 + "&loc2=" + location_loc2 + "&user=" + location_user);
                location_ifrIsLoaded = true;
            }
        };
        var onClose = function () { location_Close(null, 0); };
        var buttons = null;

        var dialogPostion = { my: "left top", at: "left top", of: window  };
        location_LocType.form = v4_dialog(idContainer, $("#" + idContainer), title, width, height, onOpen, onClose, buttons, dialogPostion );
    }

    $("#divLocationType").dialog("option", "title", title);
    location_LocType.form.dialog("open");
}

function location_Close(ifrIdp, addFocus) {
    if (null == location_LocType.form) return;
    if (ifrIdp == null) {
        var location_idp = $("#ifrLocType")[0].contentWindow.idp;
        v4_closeIFrameSrc("ifrLocType", location_idp);
    }
    location_LocType.form.dialog("close");
    location_LocType.form = null;
    location_ifrIsLoaded = false;

    if (addFocus != "") {
        $('#' + addFocus).focus();
    }
    ResizeDialog(240, 650);
}


function setIframeHeight() {
    $('#ifrLocType').height($('#divLocationType').height());
};

function location_Records_Save() {
    cmdasync("cmd", "CloseForm");
}


function OnClearLoc() {
    if (document.all("kesLocation").value == "") {
        document.all("btnOK").style.display = "none";
        document.all("btnDel").style.display = "none";
    }
    else {
        document.all("btnOK").style.display = "block";
        document.all("btnDel").style.display = "block";
    }
}

function ResizeDialog(dh, dw) {
    var sw = window.screen.availWidth;
    var sh = window.screen.availHeight;
    var twosc = 0;

    if (parseInt(window.dialogLeft) > parseInt(sw)) twosc = parseInt(window.screen.availWidth);

    window.resizeTo(dw, dh);
    window.moveTo(Math.round((sw - dw) / 2 + twosc), Math.round((sh - dh) / 2) );
    resize();
}

/*

function SetLoc1() {
    frmXml.documentElement.setAttribute('loc1', document.all("kesLoc1").value);
    doSrvCmd('RefreshEquipment');
}

function SetLoc2() {
    frmXml.documentElement.setAttribute('loc2', document.all("kesLoc2").value);
    doSrvCmd('RefreshEquipment');
}

function init() {
    document.all("kesLoc1").AfterValueChanged = SetLoc1;
    document.all("kesLoc2").AfterValueChanged = SetLoc2;
    document.all('btn1').focus();
}
*/