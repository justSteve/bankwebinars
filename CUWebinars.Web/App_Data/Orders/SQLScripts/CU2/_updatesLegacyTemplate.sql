USE TTSWebinars2
GO

SELECT * FROM dbo.Template WHERE body LIKE '%attendee%'

UPDATE dbo.Template SET body = 
'

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Total Training Solutions Mail - Webinar Reminder: #Webinar.Title#</title></head>
<body style="font-family: Verdana, Arial, Helvetica, sans-serif">
    <table width="790px" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td height="1px" width="440">&nbsp;</td>
            <td width="230">
                <p align="center">&nbsp;</p>
            </td>
            <td width="120">&nbsp;</td>
        </tr>
        <tr>
            <td width="440" height="73px">
                <img src="http://www.ttstrain.com/images/emailHeaderLeft.png" alt="headGraphic" />
            </td>
            <td colspan="2">
                <img src="http://www.ttstrain.com/images/connectionHead.png" alt="Connection Information" width="350" height="73" />
            </td>
        </tr>
        <tr>
            <td style="background-color: #111111; color: #FFFFCC; padding: 10px;" colspan="2"><b>Event Summary</b></td>
            <td style="background-color: #CCCCCC; width=120PX"></td>
        </tr>
        <tr>
            <td colspan="3">
                <table width="550px" border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="150px" style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller; color: #000000">&nbsp;</td>
                        <td width="400px" style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Title:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.Title#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Presenter:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.Presenter.FullName#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Date:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#FormatDate(Webinar.Date)#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Time:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#FormatTimeWithDuration(this)#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="background-color: #111111; color: #FFFFCC; padding: 10px;" colspan="2"><b>Connection Information</b></td>
            <td style="background-color: #CCCCCC; width=120PX"></td>
        </tr>
        <tr>
            <td colspan="3">
                <table width="550px" border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="150px" style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td width="400px" style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Webinar ID:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.HostPropertyValues.Webinar ID#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Password:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.HostPropertyValues.Password#</td>
                    </tr>
                    <tr>
                        <td colspan="3" style="background-color: #CCCCCC; padding: 9px; line-height: 11px;" align="center" valign="top"><font size="-1">Audio is now available via Voice over IP (VoIP) directly to your audio-enabled workstation. Alternatively, you may use this information for connecting to the teleconference with your telephone</font>.</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Teleconference:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.HostPropertyValues.Teleconference#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Access Code:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.HostPropertyValues.Access Code#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">URL:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;"><a target="joinWebinar" 
						href="https://attendee.gotowebinar.com/register/5632519157031214850">https://attendee.gotowebinar.com/register/5632519157031214850</a></td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller"></td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="background-color: #111111; color: #FFFFCC; padding: 10px;" colspan="2"><b>Webinar Materials</b></td>
            <td style="background-color: #CCCCCC; width=120PX"></td>
        </tr>
        <tr>
            <td colspan="3">
                <table width="550px" border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="150px" style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td width="400px" style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Handouts:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#WebinarFilesList(Webinar.Files)#</td>
                    </tr>
                    <tr>
                        <td width="150px" style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td width="400px" style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="background-color: #111111; color: #FFFFCC; padding: 10px;" colspan="2"><b>Detailed Connection Instructions</b></td>
            <td style="background-color: #CCCCCC; width=120PX"></td>
        </tr>
    </table>
    <table width="770" border="0" cellspacing="10" cellpadding="10">
        <tr>
            <td colspan="2">
                <p>15-20 minutes before the session is scheduled to begin click this link: <a href="https://attendee.gotowebinar.com/register/5632519157031214850" target="_blank"><b>Join Webinar</b></a>. A separate browser window will load (as seen in image below) that will connect you to the webinar. This current page that you are viewing now provides the connection details and remains open while you log into your webinar.</p>
            </td>
        </tr>
        <tr>
            <td>
                <p align="center">Enter the webinar ID: <strong>#Webinar.HostPropertyValues.Webinar ID#</strong><br />
                    Enter your email address.<br />
                    Click &#39;&#39;<strong>Continue</strong>&#39;&#39;.</p>
            </td>
            <td>
                <p align="center">
                    <img src="http://www.ttstrain.com/images/How2Connect1.png" alt="http://www.ttstrain.com/images/How2Connect1.png" /></p>
            </td>
        </tr>
        <tr>
            <td bgcolor="#CCCCCC" class="label">
                <div align="center">At the next screen, enter your name and organization.  Click on &#39;<strong>&#39;Join Webinar in Progress</strong>&#39;&#39;.  If you see the standard Windows security warning about installing software, verify the source is GoToWebinar and then click <em>Yes</em> or <em>Always</em>.</div>
            </td>
            <td align="right">
                <img src="http://www.bankwebinars.com/Content/images/gtwScreen1.gif" alt="http://www.ttstrain.com/images/gtwScreen1.gif" width="393" height="168" /></td>
        </tr>
        <tr>
            <td class="label" colspan="2">&nbsp;<br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <img src="http://www.bankwebinars.com/Content/images/gtwScreen2.gif" alt="http://www.bankwebinars.com/Content/gtwScreen2.gif" width="307" height="147" />
            </td>
            <td valign="middle" bgcolor="#CCCCCC">
                <p align="center">You will then be prompted for the (case sensitive) password: #Webinar.HostPropertyValues.Password# </p>
            </td>
        </tr>
        <tr>
            <td class="label" colspan="2">&nbsp;<br />
            </td>
        </tr>
        <tr>
            <td bgcolor="#CCCCCC" class="label">
                <div align="center">Once connected to the webinar you can choose your audio source - VoIP or conventional telephone. More and more people prefer to simply use their workstation&#39;s sound card and speakers, however, if you need to use your phone you can dial (toll-free) <strong>#Webinar.HostPropertyValues.Teleconference#</strong>. When prompted, enter: <strong>#Webinar.HostPropertyValues.Access Code#</strong></div>
            </td>
            <td>
                <div align="center">
                    <img src="http://www.ttstrain.com/images/voip.jpg" alt="http://www.ttstrain.com/images/voip.jpg" /></div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <p>&nbsp;</p>
                <p><font face="Arial, san-serif">Enjoy the webinar<!</font> <font face="Arial, san-serif">
                    <br />
                    Tech Support - 800-831-0678 ext 6</font></p>
                <p>Details about this webinar can be reviewed at: <a href="http://www.bankwebinars.com/Webinar/Details/#Webinar.ID#" target="_blank">http://www.bankwebinars.com/Webinar/Details/#Webinar.ID#</a></p>
            </td>
        </tr>
    </table>
    <div align="center" style="background-color: #000000; color: #CCCCCC; font-size: smaller">
        <br />
        Powered by Total Training Solutions Inc. | (800) 831-0678 | www.TTSTrain.com<br />
        &nbsp;</div>
</body>
</html>
'
WHERE idTemplate = 7

UPDATE dbo.Template SET body = 
'
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Total Training Solutions Mail - Webinar Reminder: #Webinar.Title#</title></head>
<body style="font-family: Verdana, Arial, Helvetica, sans-serif">
    <table width="790px" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td height="1px" width="440">&nbsp;</td>
            <td width="230">
                <p align="center">&nbsp;</p>
            </td>
            <td width="120">&nbsp;</td>
        </tr>
        <tr>
            <td width="440" height="73px">
                <img src="http://www.ttstrain.com/images/emailHeaderLeft.png" alt="headGraphic" />
            </td>
            <td colspan="2">
                <img src="http://www.ttstrain.com/images/connectionHead.png" alt="Connection Information" width="350" height="73" />
            </td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <blockquote><font size="-1">
                    <br />
                    NOTE: Our records indicate that you have not yet confirmed your newly created account with BankWebinars.com. <a href="mailto:confirm@bankwebinars.com">Please click here to send a blank email to BankWebinars.com so we can confirm your address.</a><br />
                </font></blockquote>
            </td>
        </tr>
        <tr>
            <td style="background-color: #111111; color: #FFFFCC; padding: 10px;" colspan="2"><b>Event Summary</b></td>
            <td style="background-color: #CCCCCC; width=120PX"></td>
        </tr>
        <tr>
            <td colspan="3">
                <table width="550px" border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="150px" style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller; color: #000000">&nbsp;</td>
                        <td width="400px" style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Title:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.Title#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Presenter:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.Presenter.FullName#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Date:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#FormatDate(Webinar.Date)#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Time:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#FormatTimeWithDuration(this)#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="background-color: #111111; color: #FFFFCC; padding: 10px;" colspan="2"><b>Connection Information</b></td>
            <td style="background-color: #CCCCCC; width=120PX"></td>
        </tr>
        <tr>
            <td colspan="3">
                <table width="550px" border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="150px" style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td width="400px" style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Webinar ID:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.HostPropertyValues.Webinar ID#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Password:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.HostPropertyValues.Password#</td>
                    </tr>
                    <tr>
                        <td colspan="3" style="background-color: #CCCCCC; padding: 9px; line-height: 11px;" align="center" valign="top"><font size="-1">Audio is now available via Voice over IP (VoIP) directly to your audio-enabled workstation. Alternatively, you may use this information for connecting to the teleconference with your telephone</font>.</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Teleconference:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.HostPropertyValues.Teleconference#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Access Code:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#Webinar.HostPropertyValues.Access Code#</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">URL:&nbsp;</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;"><a href="https://attendee.gotowebinar.com/register/5632519157031214850">www.joingotomeeting.com</a></td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="background-color: #111111; color: #FFFFCC; padding: 10px;" colspan="2"><b>Webinar Materials</b></td>
            <td style="background-color: #CCCCCC; width=120PX"></td>
        </tr>
        <tr>
            <td colspan="3">
                <table width="550px" border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="150px" style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td width="400px" style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">Handouts:</td>
                        <td style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">#WebinarFilesList(Webinar.Files)#</td>
                    </tr>
                    <tr>
                        <td width="150px" style="text-align: right; background-color: #CCCCCC; padding-right: 6px; font-size: smaller">&nbsp;</td>
                        <td width="400px" style="text-align: left; background-color: #B4D1EC; padding-left: 6px;">&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="background-color: #111111; color: #FFFFCC; padding: 10px;" colspan="2"><b>Detailed Connection Instructions</b></td>
            <td style="background-color: #CCCCCC; width=120PX"></td>
        </tr>
    </table>
    <table width="770" border="0" cellspacing="10" cellpadding="10">
        <tr>
            <td colspan="2">
                <p>15-20 minutes before the session is scheduled to begin click this link: 
				<a href="https://attendee.gotowebinar.com/register/5632519157031214850" target="_blank"><b>Join Webinar</b></a>. A separate browser window will load (as seen in image below) that will connect you to the webinar. This current page that you are viewing now provides the connection details and remains open while you log into your webinar.</p>
            </td>
        </tr>
        <tr>
            <td>
                <p align="center">Enter the webinar ID: <strong>#Webinar.HostPropertyValues.Webinar ID#</strong><br />
                    Enter your email address.<br />
                    Click &#39;&#39;<strong>Continue</strong>&#39;&#39;.</p>
            </td>
            <td>
                <p align="center">
                    <img src="http://www.ttstrain.com/images/How2Connect1.png" alt="http://www.ttstrain.com/images/How2Connect1.png" /></p>
            </td>
        </tr>
        <tr>
            <td bgcolor="#CCCCCC" class="label">
                <div align="center">At the next screen, enter your name and organization.  Click on &#39;<strong>&#39;Join Webinar in Progress</strong>&#39;&#39;.  If you see the standard Windows security warning about installing software, verify the source is GoToWebinar and then click <em>Yes</em> or <em>Always</em>.</div>
            </td>
            <td align="right">
                <img src="http://www.ttstrain.com/images/gtwScreen1.gif" alt="http://www.ttstrain.com/images/gtwScreen1.gif" width="393" height="168" /></td>
        </tr>
        <tr>
            <td class="label" colspan="2">&nbsp;<br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <img src="http://www.ttstrain.com/images/gtwScreen2.gif" alt="http://www.ttstrain.com/images/gtwScreen2.gif" width="307" height="147" />
            </td>
            <td valign="middle" bgcolor="#CCCCCC">
                <p align="center">You will then be prompted for the (case sensitive) password: #Webinar.HostPropertyValues.Password# </p>
            </td>
        </tr>
        <tr>
            <td class="label" colspan="2">&nbsp;<br />
            </td>
        </tr>
        <tr>
            <td bgcolor="#CCCCCC" class="label">
                <div align="center">Once connected to the webinar you can choose your audio source - VoIP or conventional telephone. More and more people prefer to simply use their workstation&#39;s sound card and speakers, however, if you need to use your phone you can dial (toll-free) <strong>#Webinar.HostPropertyValues.Teleconference#</strong>. When prompted, enter: <strong>#Webinar.HostPropertyValues.Access Code#</strong></div>
            </td>
            <td>
                <div align="center">
                    <img src="http://www.ttstrain.com/images/voip.jpg" alt="http://www.ttstrain.com/images/voip.jpg" /></div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <p>&nbsp;</p>
                <p><font face="Arial, san-serif">Enjoy the webinar<font size="-1">!</font></font> <font face="Arial, san-serif">
                    <br />
                    Tech Support - 800-831-0678 ext 6</font></p>
                <p>Details about this webinar can be reviewed at: <a href="http://www.bankwebinars.com/Webinar/Details/#Webinar.ID#" target="_blank">http://www.bankwebinars.com/Webinar/Details/#Webinar.ID#</a></p>
            </td>
        </tr>
    </table>
    <div align="center" style="background-color: #000000; color: #CCCCCC; font-size: smaller">
        <br />
        Powered by Total Training Solutions Inc. | (800) 831-0678 | www.TTSTrain.com<br />
        &nbsp;</div>
</body>
</html>

'


WHERE idTemplate = 8