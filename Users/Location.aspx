<%@ Page language="c#" Codebehind="Location.aspx.cs" AutoEventWireup="True" Inherits="Kesco.App.Web.Users.Location" %>
<%@ Register TagPrefix="v4control" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>
<%@ Register TagPrefix="v4dbselect" Namespace="Kesco.Lib.Web.DBSelect.V4" Assembly="DBSelect.V4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= Title %></title>
    <link rel="stylesheet" type="text/css" href="Kesco.Users.css"/>
    <script type="text/javascript" src="Kesco.Location.js?v=1"></script>
    <style type="text/css">
        html {overflow:hidden;}
    </style>
</head>
<body id="LocationForm">
<form id="mvcDialogResult" action="<%= Request["callbackUrl"] %>" method="post" >			
    <input type="hidden" name="escaped" value="0" />
    <input type="hidden" name="control" value="" />
    <input type="hidden" name="callbackkey" value="" />
    <input type="hidden" name="multiReturn" value="" />
    <input type="hidden" name="value" value="" />
</form>
<script language="javascript" type="text/javascript">
    function returnMvcDialogResult(value, mvc4, doEscape, control, callbackkey, multiReturn) {
        var form = document.getElementById('mvcDialogResult');
        if (!form) return;
				
        var val = JSON.stringify(value);

        form.elements["value"].value = doEscape ? escape(val) : val;

        form.elements["control"].value = control;
        form.elements["callbackkey"].value = callbackkey;
        form.elements["multiReturn"].value = multiReturn;

        form.elements["escaped"].value = doEscape? "1" : "0";
        form.submit(doEscape);
        if (mvc4) closeWindow(); 
    }

    function closeWindow() {
        var ver = parseFloat(navigator.appVersion.split('MSIE')[1]);
        if (parent.window != null) {
            if (ver < 7) {
                parent.window.opener = this;
            } else {
                parent.window.open('', '_parent', '');
            }
            parent.window.close();
        } else {
            if (ver < 7) {
                window.opener = this;
            } else {
                window.open('', '_parent', '');
            }
            window.close();
        }
    }
</script>	

<div class="v4formContainer">
    <div class="marginD"><%= RenderHeader() %></div>
    <div class="marginL">
        <div id="ShipperStorePanel" class="predicate_block">
            <div class="label"><%= Resx.GetString("Inv_lblLocation") %>:</div>
            <v4dbselect:DBSLocation id="efWorkPlace" runat="server" Width="491px" IsAlwaysAdvancedSearch="True" CSSClass="aligned_control"/>
        </div>
        <div class="footer">
            <v4control:Changed ID="efChanged" runat="server"/>
        </div>
    </div>
</div>

<div id="divLocationType" style="display: none; padding: 2px 0 0 0; overflow:hidden;">
    <div class="v4DivTable" id="divProgressBar" style="display: none; height: 100%; position: absolute; width: 100%;">
        <div class="v4DivTableRow">
            <div class="v4DivTableCell">
                <img src="/styles/ProgressBar.gif" alt="wait"/><br/><%= Resx.GetString("lblWait") %>...
            </div>
        </div>
    </div>
    <div id="divFrame" style="overflow:hidden;">
        <iframe id="ifrLocType" style="width: 100%; height: 400px;" onload="setIframeHeight();" style="overflow:hidden;"></iframe>
    </div>
</div>

</body>
</HTML>
