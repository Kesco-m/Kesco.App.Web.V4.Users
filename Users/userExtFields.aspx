<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserExtFields.aspx.cs" Inherits="Kesco.App.Web.Users.UserExtFields"%>
<%@ Register TagPrefix="v4dbselect" Namespace="Kesco.Lib.Web.DBSelect.V4" Assembly="DBSelect.V4" %>
<%@ Register TagPrefix="v4control" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= Title %></title>
    <link rel="stylesheet" type="text/css" href="Kesco.Users.css"/>
    <script  type="text/javascript" src="Kesco.Users.js"></script>
</head>
<body style="overflow: hidden">
<div class="marginD"><%= RenderDocumentHeader() %></div>
<div id="UserPanel" class="v4formContainer" style="overflow: auto">
    <div class="marginL">
        <div class="predicate_block">
            <div class="inline_predicate_block" style="width: 320px;">
                <div id="InfoRusPanel" class="predicate_block">
                    <div style="display: inline-block; vertical-align: top; width: 300px;">
                        <%= Resx.GetString("Users_msgFieldAsNationalPassport") %> <%= AreaRegistration %>
                        <div id="divErrNoRequzit" class="errS">&nbsp;</div>
                    </div>
                </div>
                <div id="LastNamePanel" class="predicate_block">
                    <div class="label"><%= Resx.GetString("Users_lblSurName") %>:</div>
                    <v4control:TextBox ID="efRusLName" runat="server" Width="150px" NextControl="efRusFName" OnChanged="efRusLName_OnChanged" CSSClass="aligned_control"></v4control:TextBox>
                    <div id="divErrFIO" class="errS" style="display: none;"><%= Resx.GetString("Users_msgDoesNotMatchPerson") %></div>
                </div>
                <div id="FirstNamePanel" class="predicate_block">
                    <div class="label"><%= Resx.GetString("Users_lblName") %>:</div>
                    <v4control:TextBox ID="efRusFName" runat="server" Width="150px" NextControl="efRusMName" OnChanged="efRusFName_OnChanged" CSSClass="aligned_control"></v4control:TextBox>
                    <div id="divErrFIO2" class="errS" style="display: none;"><%= Resx.GetString("Users_msgDoesNotMatchPerson") %></div>
                </div>
                <div id="MiddleNamePanel" class="predicate_block">
                    <div class="label"><%= Resx.GetString("Users_lblMiddleName") %>:</div>
                    <v4control:TextBox ID="efRusMName" runat="server" Width="150px" NextControl="efEnLName" OnChanged="efRusMName_OnChanged" CSSClass="aligned_control"></v4control:TextBox>
                    <div id="divErrFIO3" class="errS" style="display: none;"><%= Resx.GetString("Users_msgDoesNotMatchPerson") %></div>
                </div>

            </div>
            <div class="inline_predicate_block" style="width: 270px;">
                <div id="InfoEnPanel" class="predicate_block">
                    <div style="display: inline-block; vertical-align: top; width: 270px;">
                        <%= Resx.GetString("Users_msgFieldInEnglish") %>
                    </div>
                    <div class="errS">&nbsp;</div>
                </div>
                <div id="LastNameEnPanel" class="predicate_block">
                    <div class="label">Last Name:</div>
                    <v4control:TextBox ID="efEnLName" runat="server" Width="150px" NextControl="efEnFName" CSSClass="aligned_control"></v4control:TextBox>
                    <div id="divErrFIOEn" class="errS" style="display: none;">&nbsp;</div>
                </div>
                <div id="FirstNameEnPanel" class="predicate_block">
                    <div class="label">First Name:</div>
                    <v4control:TextBox ID="efEnFName" runat="server" Width="150px" NextControl="efEnMName" CSSClass="aligned_control"></v4control:TextBox>
                    <div id="divErrFIOEn2" class="errS" style="display: none;">&nbsp;</div>
                </div>
                <div id="MiddleNameEnPanel" class="predicate_block">
                    <div class="label">Middle Name:</div>
                    <v4control:TextBox ID="efEnMName" runat="server" Width="150px" NextControl="efStatus" CSSClass="aligned_control"></v4control:TextBox>
                    <div id="divErrFIOEn3" class="errS" style="display: none;">&nbsp;</div>
                </div>

            </div>
        </div>

        <div id="AreaRegistrationPanel" class="predicate_block" runat="server" style="vertical-align: middle;">
            <div class="label140"><%= Resx.GetString("Users_lblCountryOfRegistration") %>:</div>
            <v4control:TextBox ID="efAreaRegistration" runat="server" IsReadOnly="True" CSSClass="aligned_control"/>
            <v4control:Link runat="server" ID="efAreaRegistrationLink" Style="color: red; display: none;"/>
        </div>
        <div id="BirthDatePanel" class="predicate_block" runat="server">
            <div class="label140"><%= Resx.GetString("Users_lblBirthDate") %>:</div>
            <v4control:TextBox ID="efBirthDate" runat="server" IsReadOnly="True" CSSClass="aligned_control"/><v4control:Link runat="server" ID="efBirthDateLink" Style="color: red; display: none;"/>
        </div>
        <div id="INNPanel" class="predicate_block" runat="server">
            <div class="label140"><%= Resx.GetString("Users_lblINN") %>:</div>
            <v4control:TextBox ID="efINN" runat="server" IsReadOnly="True" CSSClass="aligned_control"/><v4control:Link runat="server" ID="efINNLink" Style="color: red; display: none;"/>
        </div>
        <div id="PersonInfoErrorPanel" runat="server" style="display: none; margin-top: 5px;">
            <span style="color: red;"><%= Resx.GetString("Users_msgEmployeeNotSynchronized") %>!</span>
        </div>

        <div id="StatusPanel" class="predicate_block" runat="server">
            <div class="label140"><%= Resx.GetString("Users_lblCondition") %>:</div>
            <v4control:DropDownList ID="efStatus" runat="server" IsReadOnly="True" Width="200px" NextControl="efLogin" CSSClass="aligned_control" OnChanged="efStatus_OnChanged"></v4control:DropDownList>
        </div>
        <div id="GuidPanel" class="predicate_block" runat="server">
            <div class="label140">GUID:</div>
            <v4control:TextBox ID="efGuid" runat="server" IsReadOnly="True" CSSClass="aligned_control"></v4control:TextBox>
        </div>
        <div id="LoginPanel" class="predicate_block" runat="server">
            <div class="label140">Login:</div>
            <v4control:TextBox ID="efLogin" runat="server" NextControl="efAccountDisabled" Width="150px" OnChanged="efLogin_OnChanged" CSSClass="aligned_control"></v4control:TextBox>
            <v4control:CheckBox runat="server" id="efAccountDisabled" NextControl="efDisplayName" CSSClass="aligned_control" style="margin-left: 20px;"></v4control:CheckBox>
            <div class="inline_predicate_block_text">disabled</div>
            <div id="AccountExpiresPanel" class="inline_predicate_block_text" runat="server" style="display: none; margin-left: 20px; margin-top: -1px;">
                <div style="display: inline-block; vertical-align: middle;">Account expires:</div>
                <v4control:TextBox ID="efAccountExpires" runat="server" IsReadOnly="True" NextControl="efRusFName" CSSClass="aligned_control"></v4control:TextBox>
            </div>
            <v4control:Button runat="server" id="btnUnLock" Text="UnLock" OnClick="cmdasync('cmd','UnLock');" Style="margin-left: 20px;"/>
        </div>
        <div id="DisplayNamePanel" class="predicate_block" runat="server">
            <div class="label140">Display name:</div>
            <v4control:TextBox ID="efDisplayName" runat="server" NextControl="efLang" Width="150px" CSSClass="aligned_control"></v4control:TextBox>
        </div>
        <div id="PathPanel" class="predicate_block" runat="server">
            <div class="label140">Path:</div>
            <v4control:TextBox ID="efPath" runat="server" IsReadOnly="True" CSSClass="aligned_control"></v4control:TextBox>
        </div>
        <div id="EMailPanel" class="predicate_block" runat="server">
            <div class="label140">e-Mail:</div>
            <v4control:TextBox ID="efEMail" runat="server" IsReadOnly="True" CSSClass="aligned_control"></v4control:TextBox>
        </div>
        <div id="langPanel" class="predicate_block" runat="server">
            <div class="label140"><%= Resx.GetString("Users_lblLanguage") %>:</div>
            <v4control:DropDownList ID="efLang" Width="40px" IsReadOnly="True" runat="server" NextControl="efPersonalFolder" CSSClass="aligned_control" OnChanged="efLang_OnChanged"></v4control:DropDownList>
        </div>
        <div id="PersonalFolderPanel" class="predicate_block" runat="server">
            <div class="label140"><%= Resx.GetString("Users_lblPersonalFolder") %>:</div>
            <v4control:TextBox ID="efPersonalFolder" runat="server" NextControl="efOrganization" Width="420px" CSSClass="aligned_control"></v4control:TextBox>
        </div>
        <div id="OrganizationPanel" class="predicate_block" runat="server">
            <div class="label140"><%= Resx.GetString("Users_lblCustomerOrganization") %>:</div>
            <v4dbselect:DBSPerson ID="efOrganization" runat="server" Width="400px" IsAlwaysAdvancedSearch="True" NextControl="efNotes" AutoSetSingleValue="True" CSSClass="aligned_control" OnChanged="efOrganization_OnChanged"></v4dbselect:DBSPerson>
        </div>
        <div id="Div2" class="predicate_block" runat="server">
            <div class="label140"><%= Resx.GetString("Users_lblNotes") %>:</div>
            <v4control:TextBox ID="efNotes" runat="server" Width="420px" CSSClass="aligned_control"></v4control:TextBox>
        </div>
    </div>
    

</div>
<div class="footer">
    <v4control:Changed ID="efChanged" runat="server"/>
</div>
</body>
</html>