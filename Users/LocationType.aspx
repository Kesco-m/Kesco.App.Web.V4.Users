<%@ Page language="c#" Codebehind="LocationType.aspx.cs" AutoEventWireup="True" Inherits="Kesco.App.Web.Users.LocationType" %>
<%@ Register TagPrefix="v4dbselect" Namespace="Kesco.Lib.Web.DBSelect.V4" Assembly="DBSelect.V4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head runat="server">
    <title><%= Title %></title>
    <link rel="stylesheet" type="text/css" href="Kesco.Users.css"/>
    <script type="text/javascript" src="Kesco.LocationType.js?v=1"></script>
    <style type="text/css">
        html {overflow:hidden;}
    </style>
</head>
<body>
<div>
    <div class="marginD"><%= RenderHeader() %></div>
    <div class="marginL">
        <div id="EmployeePanel" class="predicate_block">
            <div class="label150"><%= Resx.GetString("Inv_lblEmployee") %>:</div>
            <v4dbselect:DBSEmployee ID="efEmployee" runat="server" Width="350px" IsAlwaysAdvancedSearch="True" CSSClass="aligned_control"/>
        </div>

        <div id="LocationFromPanel" class="predicate_block">
            <div class="label150"><%= Resx.GetString("Inv_lblLocation")+' '+Resx.GetString("lblLocationFrom") %>:</div>
            <v4dbselect:DBSLocation ID="efLocation_Old" runat="server" Width="350px" IsAlwaysAdvancedSearch="True" CSSClass="aligned_control"/>
        </div>

        <div id="LocationToPanel" class="predicate_block">
            <div class="label150"><%= Resx.GetString("Inv_lblLocation")+' '+Resx.GetString("lblLocationTo") %>:</div>
            <v4dbselect:DBSLocation ID="efLocation_New" runat="server" Width="350px" IsAlwaysAdvancedSearch="True" CSSClass="aligned_control"/>
        </div>
        
        <div id="Equipment" style="overflow: auto; width: 100%; height: 400px;">
            <%RenderEquipment(Response.Output);%>
        </div>

    </div>
</div>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $(window).resize(function () {
            resizeEquipment();
        });
        resizeEquipment();
    });

    function resizeEquipment() {
        var hr = $(window).height();
        if ($('#Equipment').height() != null)
            $('#Equipment').height(hr - 110);
    }</script>
</body>
</html>
