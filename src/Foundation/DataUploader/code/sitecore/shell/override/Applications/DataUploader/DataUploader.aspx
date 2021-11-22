<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataUploader.aspx.cs" Inherits="FWD.Foundation.DataUploader.sitecore.DataUploader" %>

<%@ OutputCache Duration="3600" VaryByParam="None" %>
<%@ Register Assembly="Sitecore.Kernel" Namespace="Sitecore.Web.UI.WebControls" TagPrefix="cc1" %>
<%@ Register Assembly="Sitecore.Client" Namespace="Sitecore.Shell.Applications.Install.Controls" TagPrefix="cc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Data Uploader</title>
    <style>
        body {
            font-family: 'Open Sans', Arial, 'Microsoft YaHei', sans-serif;
        }

        input[type=file] {
            -webkit-appearance: textfield;
            position: relative;
            -webkit-box-sizing: border-box;
        }

        .custom-file-input::-webkit-file-upload-button {
            width: 0;
            padding: 0;
            margin: 0;
            margin-right: 5px;
            -webkit-appearance: none;
            border: none;
        }
        /*.custom-file-input::-webkit-file-upload-button {
            background-color: orange;
            color: white;
            padding: 1em;
            border: none;
            -webkit-appearance: none;
        }*/

        x::-webkit-file-upload-button, input[type=file]:after {
            content: ' ';
            display: inline-block;
            left: 100%;
            margin-left: 3px;
            position: relative;
            -webkit-appearance: button;
            padding-top: 8px;
        }

        .box {
            background: white;
            padding-top: unset;
            margin-left: auto;
            margin-right: auto;
            width: 45%;
            box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
        }

        .txtlabel {
            font-weight: 500;
            padding: 10px;
            width: 100%;
            max-width: 151px;
            display: inline-block;
        }

        .custom-file-input {
            border: 2px solid #e3e3e3;
            height: 36px;
            margin: 3px;
            width: 50%;
        }

        select {
            border: 2px solid #e3e3e3;
            padding: 3px;
            width: 30%;
            min-height: 36px;
        }

        input[type="text"] {
            min-height: 30px;
            border: 2px solid #e3e3e3;
        }

        .btnupload {
            height: 43px;
            width: 90px;
            font-size: 15px;
            font-weight: 900;
            background: orange;
            border: none;
            color: white;
        }

        .listBox {
            border-style: none;
            border-width: 0px;
            border: none;
            font-size: 12pt;
            font-family: Verdana;
        }

        .header {
            background: #2b2b2b;
            color: #fff;
            padding: 14px 0 14px 13px;
            line-height: 28px;
            font-size: 18px;
            text-align: left;
        }

            .header > h4 {
                padding: 5px 0px 5px 9px;
            }

        .resultTable {
            height: 150px;
            overflow-y: auto;
            margin-top: 20px;
        }

        .langList {
            display: inline-block;
        }

        .input-section {
            padding: 4px;
        }

        .btnExport {
            background-color: orange;
            color: white;
            border-style: none;
            padding: 10px;
            font-weight: 900;
            font-size: 15px;
        }

        .linkstyle {
            text-decoration-line: none;
            font-size: large;
        }

        .lblFeature {
            text-align: center;
            color: red;
        }

        .overlay {
            z-index: 999999;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            position: fixed;
            background-color: rgb(39 33 39 / 68%);
        }

        .overlay__inner {
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            position: absolute;
        }

        .overlay__content {
            left: 50%;
            position: absolute;
            top: 50%;
            transform: translate(-50%, -50%);
        }

        .spinner {
            width: 75px;
            height: 75px;
            display: inline-block;
            border-width: 5px;
            border-color: rgba(255, 255, 255, 0.05);
            border-top-color: orange;
            animation: spin 1s infinite linear;
            border-radius: 100%;
            border-style: solid;
        }

        @keyframes spin {
            100% {
                transform: rotate(360deg);
            }
        }

        .dialogFooter {
            position: relative;
            background-color: #f0f0f0;
            border-top: 1px solid #e3e3e3;
            padding: 8px 15px 9px;
            height: 56px;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            line-height: 34px;
        }

        .btn, input[type="submit"] {
            color: black;
            display: inline-block;
            margin-bottom: 0;
            font-weight: normal;
            text-align: center;
            vertical-align: middle;
            cursor: pointer;
            border: 1px solid #bdbdbd;
            white-space: nowrap;
            font-size: 12px;
            -webkit-user-select: none;
            -moz-user-select: none;
            -ms-user-select: none;
            user-select: none;
            min-width: 80px;
            height: 36px;
            text-shadow: none;
            margin-left: 10px;
            background-repeat: repeat-x;
            -webkit-box-shadow: inset 0 1px #ffffff;
            box-shadow: inset 0 1px #ffffff;
            text-shadow: none;
            background-color: #d9d9d9;
            background-image: linear-gradient(to bottom, #f0f0f0 0%, #d9d9d9 100%);
            -moz-border-radius: 6px;
            -webkit-border-radius: 6px;
            border-radius: 6px;
        }

            .btn.buttonPrimary {
                background-repeat: repeat-x;
                border-color: #207da2;
                -webkit-box-shadow: inset 0 1px #5dbadf;
                box-shadow: inset 0 1px #5dbadf;
                color: #fff;
                background-color: #207da2;
                background-image: linear-gradient(to bottom, #289bc8 0%, #207da2 100%);
                -moz-border-radius: 6px;
                -webkit-border-radius: 6px;
                border-radius: 6px;
                height: 36px;
            }

        .padding10 {
            padding: 10px;
        }

        .langStyle {
            display: block;
            padding-left: 29%;
        }
    </style>
</head>
<body style="position: absolute; width: 98%; background-color: #F0F0F0;" runat="server">
    <asp:Panel CssClass="overlay" runat="server" ID="Spinner" Style="display: none;">
        <div class="overlay__inner">
            <div class="overlay__content"><span class="spinner"></span></div>
        </div>
    </asp:Panel>
    <input type="hidden" id="txtIsdisable" runat="server" />
    <div class="box">
        <div class="header">
            <div style="">Data Upload Utility </div>

        </div>
        <form id="form1" runat="server" enctype="multipart/form-data">

            <div style="padding: 15px 0px;">
                <div class="input-section">
                    <asp:Label ID="lblMarketSite" runat="server" CssClass="txtlabel"></asp:Label>
                    <asp:DropDownList ID="SiteNode" runat="server" AutoPostBack="True" OnSelectedIndexChanged="SiteNode_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <div class="input-section">
                    <asp:Label ID="lblLanguage" runat="server" CssClass="txtlabel">
                    </asp:Label>
                    <asp:CheckBoxList ID="LanguageCheckBoxList" runat="server" CssClass="langList" AutoPostBack="True" OnSelectedIndexChanged="LanguageCheckBoxList_SelectedIndexChanged">
                    </asp:CheckBoxList>
                    <asp:Label ID="lblLang" runat="server" CssClass="langStyle"></asp:Label>
                </div>
                <div class="input-section">
                    <asp:Label ID="lblDefaultLang" runat="server" CssClass="txtlabel"></asp:Label>
                    <asp:DropDownList ID="DefaultLanguage" runat="server"></asp:DropDownList>
                </div>
                <div class="input-section">
                    <asp:Label ID="lblType" runat="server" CssClass="txtlabel"></asp:Label>
                    <asp:DropDownList ID="FolderType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="TemplateFolder_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <div class="input-section">
                    <asp:Label ID="lblParentFolder" runat="server" CssClass="txtlabel"></asp:Label>
                    <asp:DropDownList ID="ParentNode" runat="server" OnSelectedIndexChanged="ParentNode_SelectedIndexChanged">
                        <asp:ListItem Text="Select Parent Node" Value="-"></asp:ListItem>
                    </asp:DropDownList>
                    Or
                <asp:TextBox ID="txtParentName" runat="server" placeholder="Enter Parent Name" title="Enter Parent Name if you want to create your own folder under selected Parent Node" />
                </div>

                <div class="input-section">
                    <asp:Label ID="lblTemplate" runat="server" CssClass="txtlabel"></asp:Label>
                    <asp:DropDownList ID="TemplateItem" runat="server">
                        <asp:ListItem Text="Select Template" Value="-"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="input-section">
                    <asp:Label ID="lblCreateSubFolder" runat="server" CssClass="txtlabel">
                    </asp:Label>
                    <asp:CheckBox ID="createSubfolder" runat="server" Text="" Checked="True" />
                </div>
                <div class="input-section">
                    <asp:Label ID="lblAlphabets" runat="server" CssClass="txtlabel">
                    </asp:Label>
                    <asp:DropDownList ID="Alphabets" runat="server">
                        <asp:ListItem>1</asp:ListItem>
                        <asp:ListItem Selected="True">2</asp:ListItem>
                        <asp:ListItem>3</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="input-section">
                    <asp:Label ID="lblSelectFile" runat="server" CssClass="txtlabel"></asp:Label>
                    <asp:FileUpload ID="FileUpload" runat="server" CssClass="custom-file-input" />

                </div>



            </div>
            <div class="input-section padding10">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
                <asp:Label ID="lblUploadMessage" runat="server"></asp:Label>
            </div>
            <div style="text-align: right;" class="dialogFooter">
                <asp:Button ID="btnValidate" runat="server" OnClick="btnValidate_Click" CssClass="btn buttonPrimary" OnClientClick="EnableExport();" />
                <a runat="server" id="lnkGoTO" target="_blank" class="linkstyle btn"></a>
                <asp:Button ID="btnExport" CssClass="btn" runat="server" OnClientClick='exportTableToExcel("resultTable","Data_Upload_Log")' Visible="False" />

            </div>

        </form>
        <div id="lblFeatureDiv" hidden>
            <h2 id="lblFeature" class="lblFeature"></h2>
        </div>
    </div>

    <div class="resultTable" style="display: none;">
        <asp:Table ID="resultTable" runat="server" Width="80%" BackColor="White" CellPadding="1" CellSpacing="1" GridLines="Both" Height="150px" HorizontalAlign="Center" Visible="False">
            <asp:TableRow>
                <asp:TableHeaderCell>Name</asp:TableHeaderCell>
                <asp:TableHeaderCell>ID</asp:TableHeaderCell>
                <asp:TableHeaderCell>Status</asp:TableHeaderCell>
            </asp:TableRow>
        </asp:Table>

    </div>
    <script>
        var isDisable = document.getElementById("txtIsdisable").value;
        if (isDisable.toLowerCase() == "true") {
            document.getElementById("form1").style.display = "none";
            document.getElementById("lblFeatureDiv").hidden = false;
            document.getElementById("lblFeature").innerText = "Feature is Disabled"
        }
        function EnableExport() {
            document.getElementById("Spinner").style.display = "block";
            document.getElementById("btnExport").style.display = "inline-block";



        }
        function exportTableToExcel(tableID, filename = '') {
            var downloadLink;
            var dataType = 'application/vnd.ms-excel';
            var tableSelect = document.getElementById(tableID);
            var tableHTML = tableSelect.outerHTML.replace(/ /g, '%20');

            // Specify file name
            filename = filename ? filename + '.xls' : 'excel_data.xls';

            // Create download link element
            downloadLink = document.createElement("a");

            document.body.appendChild(downloadLink);

            if (navigator.msSaveOrOpenBlob) {
                var blob = new Blob(['\ufeff', tableHTML], {
                    type: dataType
                });
                navigator.msSaveOrOpenBlob(blob, filename);
            } else {
                // Create a link to the file
                downloadLink.href = 'data:' + dataType + ', ' + tableHTML;

                // Setting the file name
                downloadLink.download = filename;

                //triggering the function
                downloadLink.click();
            }
        }
    </script>
</body>
</html>
