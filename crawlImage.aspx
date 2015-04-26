<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="crawlImage.aspx.cs" Inherits="youknow.crawlImage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Untitled Page</title>
	<style type="text/css">
		.style1
		{
			text-align: center;
		}
		.style2
		{
			text-align: left;
		}
		.style3
		{
			text-align: right;
			width: 50%;
		}
	</style>    
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<table style="width: 100%;">
			<tr>
				<td colspan="2" style="text-align: center">
					<h1>
						Batch Runner Example</h1>
				</td>
			</tr>
		</table>
		<table style="width: 100%;" id="tblBatchParameters" runat="server">
			<tr>
				<td class="style3">
					Start Date:&nbsp;
				</td>
				<td class="style2">
					&nbsp;
					<asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td class="style3">
					End Date:
				</td>
				<td class="style2">
					&nbsp;
					<asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td class="style1" colspan="2">
					<asp:Button ID="btnRunBatch" runat="server" Text="Run Batch" 
						onclick="btnRunBatch_Click"></asp:Button>
				</td>
			</tr>
		</table>
		<table width="100%" id="tblBatchProgress" runat="server" visible="false">
			<tr>
				<td rowspan="2" align="left" valign="middle" class="style3">
					<img id="Img1" alt="Loading" src="/images/loading.gif" runat="server" style="width: 30px;
						height: 30px" />
				</td>
				<td>
					Job Running: <b>
						<asp:Label ID="lblPercentage" runat="server"></asp:Label>%</b> &nbsp;complete
				</td>
			</tr>
			<tr>
				<td>
					Estimated Minutes Remaining: <b>
						<asp:Label ID="lblTimeRemaining" runat="server"></asp:Label></b>
				</td>
			</tr>
            <tr>
				<td>
					Infomation: <b>
						<asp:Label ID="lblInfo" runat="server"></asp:Label></b>
				</td>
			</tr>
			<tr>
				<td>
				</td>
				<td>
					<asp:Button ID="btnStop" runat="server" Text="Stop" OnClick="btnStop_Click" OnClientClick="this.value='Canceling...';this.enable=false;" />
				</td>
			</tr>
		</table>
	</div>
	<asp:Timer ID="timerBatchRun" runat="server" ontick="timerBatchRun_Tick" 
		Enabled="false" Interval="4000">
	</asp:Timer>
	<asp:ScriptManager ID="scriptManger" runat="server">
	</asp:ScriptManager>
	</form>
    
    <div class="container">
        <input type="text" id="message" />
        <input type="button" id="sendmessage" value="Send" />
        <input type="hidden" id="displayname" />
        <ul id="discussion">
        </ul>
    </div>
    <!--Script references. -->
    <!--Reference the jQuery library. -->
    <script src="/Scripts/jquery-1.8.2.min.js" ></script>
    <!--Reference the SignalR library. -->
    
</body>
</html>
