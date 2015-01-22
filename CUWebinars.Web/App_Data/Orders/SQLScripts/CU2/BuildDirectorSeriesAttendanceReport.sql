USE DirSeriesAttendanceImporter
GO
SELECT  DateInserted ,
        EventID ,
        EventTitle ,
        RegisteredOn ,
        FirstName ,
        LastName ,
        Email ,
        Bank ,
        TotalViews ,
        TotalDuration ,
        TimeIn ,
        TimeOut ,
        SessionDuration
FROM    dbo.DirSeriesAttend2
WHERE   DateInserted > '1/01/2015'
--AND DATEPART(MINUTE, SessionDuration) > 1
AND SessionDuration NOT LIKE '00:00%'
ORDER BY SessionDuration 