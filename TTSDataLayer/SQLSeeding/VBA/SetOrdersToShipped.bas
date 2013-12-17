Attribute VB_Name = "SetShippedDate"
' ==========================================================================
' Module      : SetShippedDate
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

Private Const msMODULE As String = "SetShippedDate"

Sub CallSetShippedDateSproc()
    Const sPROC As String = "CallSetShippedDateSproc"

    Dim bValid As Boolean
    Dim sVal   As String

    'On Error GoTo PROC_ERR
    Call Trace(tlIntense, msMODULE, sPROC, gsPROC_ENTER)

    Dim objConn As New ADODB.Connection
    Dim objCmd As New ADODB.Command
    Dim objParm1 As New ADODB.Parameter
    Dim objRs  As New ADODB.Recordset

    Dim wbBook As Workbook
    Dim wsSheet As Worksheet

    Set objConn = GetNewConnection
    Set objCmd = New ADODB.Command
    objCmd.CommandType = adCmdStoredProc
    objCmd.CommandText = "SetOrderShippedDate"
    objCmd.Parameters.Refresh
    Set wbBook = ActiveWorkbook
    Set wsSheet = ActiveSheet
    'Dim rnStoreOrderID As Excel.Range
    Dim webinarID As Integer

    Dim myRow    As Excel.Range
    myRow = ActiveCell.row

    Dim theDate As Date
    Dim idOrder As Integer

    theDate = Range(myRow, 3).Value2
    idOrder = Range(myRow, 8).Value2
    Worksheets("Archive").Unprotect
    Worksheets("Archive").Rows(2).Insert
    Worksheets("Archive").Range("A2").Value2 = theDate
    Worksheets("Archive").Range("A1").Value2 = idOrder

    Dim parm_idOrder As New ADODB.Parameter

    Dim optionId As Long
    Dim orderId As Long
    Set parm_idOrder = objCmd.CreateParameter("idOrder", adInteger, adParamInput)

    objCmd.Parameters.Append parm_idOrder
    objCmd.ActiveConnection = objConn
    parm_idOrder.Value = idOrder


    Set objRs = objCmd.Execute()

    'If Not objRs.EOF Then orderId = objRs.Fields("orderID").Value


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

'BeginNewConnection
Private Function GetNewConnection() As ADODB.Connection
    Const sPROC As String = "GetNewConnection"

    Dim bValid As Boolean
    Dim sVal   As String

    'On Error GoTo PROC_ERR
    'Call Trace(tlIntense, msMODULE, sPROC, gsPROC_ENTER)
    Dim oCn    As New ADODB.Connection
    Dim sCnStr As String

    sCnStr = "Driver={SQL Server};Server=25.149.190.135;Database=TTSWebinars2;UID=EventsOp;PWD=this Old Horse22;"
    'sCnStr = "Driver={SQL Server};Server=y1qylg6yd2.database.windows.net,1433;Database=CUWebinars;UID=Lab1;PWD=pJ6Cic0EFPC5"
    'sCnStr = "Provider=SQLOLEDB.1;Integrated Security=SSPI;" & _
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
