Attribute VB_Name = "CUWebinarImporter"
' ==========================================================================
' Module      : CUWebinarImporter
' Type        : Module
' Description : Demo code for debugging tools
' --------------------------------------------------------------------------
' Procedures  : XXX
' ==========================================================================

' -----------------------------------
' Option statements
' -----------------------------------

Option Explicit

' -----------------------------------
' Constant declarations
' -----------------------------------
' Module Level
' ----------------

Private Const msMODULE As String = "CUWebinarsOrdersImporter"
Function CallCreateWebUserAccountAPI() As Long
    Const sPROC As String = "CallCreateWebUserAccountAPI"

    Dim bValid As Boolean
    Dim sVal   As String

    'On Error GoTo PROC_ERR
    Call Trace(tlIntense, msMODULE, sPROC, gsPROC_ENTER)

    Dim stSQL  As String
    Dim wbBook As Workbook
    Dim wsSheet As Worksheet
    Dim rnUserID As Excel.Range
    Dim rnOrderID As Excel.Range


    Set wbBook = ActiveWorkbook
    Set wsSheet = ActiveSheet
    Dim rnStoreUserID As Range

    Dim NumRows As Integer
    Dim x      As Integer
    Dim userId As Integer

    'userId = 30000

    ' Set numrows = number of rows of data.
    ' Select cell A3, *first line of data*.
    'Range("A3").Select
    ' Set Do loop to stop when an empty cell is reached.
    'Do Until IsEmpty(ActiveCell)
    'userId = userId + 1

    Set rnStoreUserID = wsSheet.Cells(Selection.Row, 1)

    Dim parm_UserType As String
    Dim parm_ActStatus As String
    Dim parm_fr As String
    Dim parm_ls As String
    Dim parm_inst As String
    Dim parm_title As String
    Dim parm_phne As String
    Dim parm_str1 As String
    Dim parm_city As String
    Dim parm_st As String
    Dim parm_zip As String
    Dim parm_email As String


    parm_ActStatus = "A"
    parm_fr = wsSheet.Cells(Selection.Row, 6)
    parm_ls = wsSheet.Cells(Selection.Row, 7)
    parm_inst = wsSheet.Cells(Selection.Row, 8)
    parm_title = wsSheet.Cells(Selection.Row, 9)
    parm_phne = wsSheet.Cells(Selection.Row, 11)
    parm_str1 = wsSheet.Cells(Selection.Row, 12)
    parm_city = wsSheet.Cells(Selection.Row, 13)
    parm_st = wsSheet.Cells(Selection.Row, 14)
    parm_zip = wsSheet.Cells(Selection.Row, 15)
    parm_email = wsSheet.Cells(Selection.Row, 10)


    Dim queryString As String
    queryString = "FirstName=" + parm_fr + "&LastName=" + parm_ls + "&Email=" + parm_email + "&Institution=" + parm_inst + "&AddressType=Billing&City=" + parm_city + "&Country=US&Name=" + parm_fr + " " + parm_ls + "&Phone=" + parm_phne + "&State=" + parm_st + "&StreetAddress=" + parm_str1 + "&StreetAddress2=x&Zip=" + parm_zip + "&Password=" + parm_ls + ".....&ConfirmPassword=" + parm_ls + ".....&Title=" + parm_title    '+ "&idWebUser=" + CStr(userId)
    Dim objHTTP As New MSXML2.XMLHTTP
    Dim URL    As String
    'URL = " http://localhost:21746/api/acctapi?" + queryString
    URL = "http://localhost:3538/account/get/?" + queryString
    URL = URL & "&currentTime=" & Now

    objHTTP.Open "GET", URL, False
    objHTTP.send

    'rnStoreUserID.Value2 =

    Debug.Print objHTTP.Status
    Debug.Print objHTTP.responseText

    CallCreateWebUserAccountAPI = objHTTP.responseText

    'ActiveCell.Offset(1, 0).Select

    'Loop

    ' ----------------------------------------------------------------------

PROC_EXIT:

    Call Trace(tlIntense, msMODULE, sPROC, gsPROC_EXIT)
    On Error GoTo 0

    Exit Function

    ' ----------------------------------------------------------------------

PROC_ERR:

    If ErrorHandler(msMODULE, sPROC, False, ThisWorkbook.Name) Then
        Stop
        Resume
    Else
        Resume PROC_EXIT
    End If

End Function


Sub CreateCUOrder()
    Const sPROC As String = "CreateCUOrder"

    Dim bValid As Boolean
    Dim sVal   As String

    'On Error GoTo PROC_ERR
    Call Trace(tlIntense, msMODULE, sPROC, gsPROC_ENTER)

    Dim objConn As New ADODB.Connection
    Dim objCmd As New ADODB.Command
    Dim objParm1 As New ADODB.Parameter
    Dim objRs  As New ADODB.Recordset

    Dim stSQL  As String
    Dim wbBook As Workbook
    Dim wsSheet As Worksheet
    Dim lnStoreUserID As Integer

    Set objConn = GetNewConnection
    Set objCmd = New ADODB.Command
    objCmd.CommandType = adCmdStoredProc
    objCmd.CommandText = "CreateCUOrder"
    objCmd.Parameters.Refresh

    Range("B3").Select
    Set wbBook = ActiveWorkbook
    Set wsSheet = ActiveSheet
    'Dim rnStoreOrderID As Excel.Range
    Dim webinarID As Integer

    Dim duration As Long
    webinarID = CInt(wsSheet.Name)
    duration = GetDuration(webinarID)
    Dim parm_UserID As New ADODB.Parameter
    Dim parm_WebinarID As New ADODB.Parameter
    Dim parm_OptionId As New ADODB.Parameter

    Dim optionId As Long
    Dim orderId As Long
    Set parm_UserID = objCmd.CreateParameter("idUser", adInteger, adParamInput)
    Set parm_WebinarID = objCmd.CreateParameter("idWebinar", adInteger, adParamInput)
    Set parm_OptionId = objCmd.CreateParameter("idOption", adInteger, adParamInput)

    objCmd.Parameters.Append parm_UserID
    objCmd.Parameters.Append parm_WebinarID
    objCmd.Parameters.Append parm_OptionId

    objCmd.ActiveConnection = objConn
    ' Set Do loop to stop when an empty cell is reached.
    Do Until IsEmpty(ActiveCell)

        lnStoreUserID = CallCreateWebUserAccountAPI

        Select Case wsSheet.Cells(Selection.Row, 4)
            Case "Live Session Only"
                If duration = 1 Then
                    optionId = 97
                Else
                    optionId = 84
                End If

            Case "OnDemand Recording Only"
                If duration = 1 Then
                    optionId = 98
                Else
                    optionId = 85
                End If

            Case "CD-ROM and Hardcopy Handouts"
                If duration = 1 Then
                    optionId = 99
                Else
                    optionId = 86
                End If

            Case "Live Plus OnDemand Weblinks"
                If duration = 1 Then
                    optionId = 100
                Else
                    optionId = 87
                End If

            Case "Premier Package"
                If duration = 1 Then
                    optionId = 101
                Else
                    optionId = 88
                End If

            Case Else
                optionId = 0
        End Select

        'parm_UserID.Value = wsSheet.Cells(Selection.Row, 1)
        parm_UserID.Value = lnStoreUserID
        parm_WebinarID.Value = webinarID
        parm_OptionId.Value = optionId

        Set objRs = objCmd.Execute()

        If Not objRs.EOF Then orderId = objRs.Fields("orderID").Value

        wsSheet.Cells(Selection.Row, 2).Value = orderId
        'rnStoreOrderID.Value2 = orderId
        ActiveCell.Offset(1, 0).Select

    Loop
    'Cleaning up.
    objRs.Close
    objConn.Close
    Set objRs = Nothing
    Set objConn = Nothing

    ' ----------------------------------------------------------------------

PROC_EXIT:

    Call Trace(tlIntense, msMODULE, sPROC, gsPROC_EXIT)
    On Error GoTo 0

    Exit Sub

    ' ----------------------------------------------------------------------

PROC_ERR:

    If ErrorHandler(msMODULE, sPROC, False, ThisWorkbook.Name) Then
        Stop
        Resume
    Else
        Resume PROC_EXIT
    End If

End Sub

Function GetDuration(webinarID)
    Const sPROC As String = "GetDuration"

    Dim bValid As Boolean
    Dim sVal   As String

    On Error GoTo PROC_ERR
    Call Trace(tlIntense, msMODULE, sPROC, gsPROC_ENTER)

    Dim objConn As New ADODB.Connection
    Dim objCmd As New ADODB.Command
    Dim objParm1 As New ADODB.Parameter
    Dim objRs  As New ADODB.Recordset

    '
    Dim sSQL   As String
    sSQL = "select duration as duration from webinar where idWebinar = " & webinarID
    objCmd.CommandText = sSQL
    objCmd.CommandType = adCmdText

    '
    Set objConn = GetNewConnection
    objCmd.ActiveConnection = objConn
    objRs.Open sSQL, objConn, adOpenForwardOnly, adLockOptimistic
    Dim duration As Long

    If Not objRs.EOF Then duration = objRs.Fields("duration").Value

    GetDuration = duration
    'clean up
    objRs.Close
    objConn.Close
    Set objRs = Nothing
    Set objConn = Nothing
    Set objCmd = Nothing
    Set objParm1 = Nothing
    'Exit Function

    ' ----------------------------------------------------------------------

PROC_EXIT:

    Call Trace(tlIntense, msMODULE, sPROC, gsPROC_EXIT)
    On Error GoTo 0

    Exit Function

    ' ----------------------------------------------------------------------

PROC_ERR:
    'clean up
    If objRs.State = adStateOpen Then
        objRs.Close
    End If

    If objConn.State = adStateOpen Then
        objConn.Close
    End If

    Set objRs = Nothing
    Set objConn = Nothing
    Set objCmd = Nothing
    Set objParm1 = Nothing


    If ErrorHandler(msMODULE, sPROC, False, ThisWorkbook.Name) Then
        Stop
        Resume
    Else
        Resume PROC_EXIT
    End If

End Function

'BeginNewConnection
Private Function GetNewConnection() As ADODB.Connection
    Const sPROC As String = "GetNewConnection"

    Dim bValid As Boolean
    Dim sVal   As String

    'On Error GoTo PROC_ERR
    'Call Trace(tlIntense, msMODULE, sPROC, gsPROC_ENTER)
    Dim oCn    As New ADODB.Connection
    Dim sCnStr As String

    'sCnStr = "Driver={SQL Server};Server=y1qylg6yd2.database.windows.net,1433;Database=CUWebinars;UID=Lab1;PWD=pJ6Cic0EFPC5"
    sCnStr = "Provider=SQLOLEDB.1;Integrated Security=SSPI;" & _
             "Persist Security Info=False;" & _
             "Initial Catalog=TTSWebinars;" & _
             "Data Source=(local)"

    oCn.Open sCnStr

    If oCn.State = adStateOpen Then
        Set GetNewConnection = oCn
    End If

End Function
'EndNewConnection



Sub SendEmail()
'Dim objHTTP As New MSXML2.XMLHTTP
'Set objHTTP = CreateObject("WinHttp.WinHttpRequest.5.1")
'Dim objHTTP As New MSXML2.XMLHTTP
'Dim URL As String
'URL = " http://localhost:21746/api/acctapi"
'objHTTP.setRequestHeader
'objHTTP.Open "GET", URL, False
'objHTTP.send ("{""key"":null,""from"":""me@me.com"",""to"":null,""cc"":null,""bcc"":null,""date"":null,""subject"":""My Subject"",""body"":null,""attachments"":null}")
'Debug.Print objHTTP.Status
'Debug.Print objHTTP.ResponseText

End Sub
