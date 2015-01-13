UPDATE  dbo.Presenter
SET     BiographyLong = REPLACE(BiographyLong,
                                'https://ttseast.blob.core.windows.net',
                                'http://images.ttstrain.com') ,
        PhotoFull = REPLACE(PhotoFull, 'https://ttseast.blob.core.windows.net',
                            'http://images.ttstrain.com') ,
        PhotoThumb = REPLACE(PhotoThumb,
                             'https://ttseast.blob.core.windows.net',
                             'http://images.ttstrain.com')
							 WHERE idUser > 1


USE [BankWebinars]
GO

/****** Object:  StoredProcedure [dbo].[InsertGTWConnectionInfo]    Script Date: 1/12/2015 6:51:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      sjh
-- Create date: 5/14
-- Description: Updates webinar table with GTW connection info
-- =============================================
CREATE PROCEDURE [dbo].[InsertGTWConnectionInfo] 
    -- Add the parameters for the stored procedure here
    @idWebinar INT, 
    @Status INT ,
    @WebinarKey NVARCHAR(MAX) ,
    @CitrixURL NVARCHAR(MAX) ,
    @AccessPhone NVARCHAR(MAX) ,
    @AccessCodeAttendee NVARCHAR(MAX) ,
    @AccessCodePresenter NVARCHAR(MAX) ,
    @AccessCodeOrganizer NVARCHAR(MAX)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    UPDATE [dbo].[Webinar]
   SET [Status] = 7
      ,[WebinarKey] = @WebinarKey
      ,[OrganizerKey] = '901873'
      ,[OrganizerOAuthKey] = '8SSOAgpL2WMxa4iUyQUn1h9bGEmJ'
      ,[CitrixRegisterUrl] = @CitrixURL
      ,[AccessPhone] = @AccessPhone
      ,[AccessCodeAttendee] = @AccessCodeAttendee
      ,[AccessCodePresenter] = @AccessCodePresenter
      ,[AccessCodeOrganizer] = @AccessCodeOrganizer
      WHERE idWebinar = @idWebinar

END
GO


CREATE PROC [dbo].[OrdersByEmail] @email VARCHAR(300)
AS
SELECT  FirstName + ' ' + LastName ,
        Institution ,
        BillingEmail ,
        BillingPhone ,
        ( SELECT    RegTypeLabel
          FROM      dbo.RegType
          WHERE     idRegType = r.idRegType
        ) , r.idRegType,
        ( SELECT    title
          FROM      webinar
          WHERE     idWebinar = r.idWebinar
        ) ,
        r.idWebinar ,
        RegistrantKey ,
        o.OrderDate ,
        CASE o.OrderStatus
          WHEN 0 THEN 'Error'
          WHEN 1 THEN 'InProcess'
          WHEN 2 THEN 'Submitted'
          WHEN 3 THEN 'Billed'
          WHEN 4 THEN 'Paid'
          WHEN 5 THEN 'Abandoned'
          WHEN 6 THEN 'Canceled'
          WHEN 7 THEN 'AwaitingVerification'
          WHEN 255 THEN 'Unknown'
        END ,
        o.idOrder ,
        o.Total
FROM    dbo.[Order] o
        INNER JOIN dbo.OrderRow r ON r.idOrder = o.idOrder
WHERE   o.BillingEmail = @email
GO



GO

/****** Object:  StoredProcedure [dbo].[OrdersByEvent]    Script Date: 1/12/2015 6:55:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[OrdersByEvent] 
    -- Add the parameters for the stored procedure here
    @idWebinar INT = 0
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    SELECT  OrderDate ,
            FirstName ,
            LastName ,
            Institution ,
            BillingEmail ,
            ( SELECT    RegTypeLabel
              FROM      dbo.RegType
              WHERE     idRegType = r.idRegType
            ) ,
            r.idRegType ,
            CASE o.OrderStatus
              WHEN 0 THEN 'Error'
              WHEN 1 THEN 'InProcess'
              WHEN 2 THEN 'Submitted'
              WHEN 3 THEN 'Billed'
              WHEN 4 THEN 'Paid'
              WHEN 5 THEN 'Abandoned'
              WHEN 6 THEN 'Canceled'
              WHEN 7 THEN 'AwaitingVerification'
              WHEN 255 THEN 'Unknown'
            END ,
            o.idOrder ,
            o.Total
    FROM    dbo.[Order] o
            INNER JOIN dbo.OrderRow r ON r.idOrder = o.idOrder
    WHERE   r.idWebinar = @idWebinar

END
GO



GO

/****** Object:  StoredProcedure [dbo].[ProcessAdditionalLocationAddresses]    Script Date: 1/12/2015 6:55:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[ProcessAdditionalLocationAddresses]
    @sText VARCHAR(8000) ,
    @sDelim VARCHAR(20) = ',' ,
    @sSQL varchar(8000) OUTPUT
AS
DECLARE @alias varchar(5) ,
    @value varchar(20)

DECLARE GetSQL_Cursor CURSOR
FOR
SELECT  'T' + LTRIM(STR(idx)) ,
        value
FROM    dbo.fn_Split(@sText, @sDelim)
ORDER BY idx
OPEN GetSQL_Cursor
FETCH FROM GetSQL_Cursor INTO @alias, @value
SET @sSQL = 'INSERT TargetTable(IDs) '
--har(39) for single quotes within strings.
SET @sSQL = @sSQL + 'SELECT a.IDs FROM dbo.fn_GetIDs
        (' + CHAR(39) + @value + CHAR(39) + ') a '

FETCH NEXT FROM GetSQL_Cursor INTO @alias, @value
WHILE @@Fetch_Status = 0
    BEGIN
        SET @sSQL = @sSQL + 'JOIN dbo.GetIDs(' + CHAR(39) + @value + CHAR(39) + ') ' + @alias
        SET @sSQL = @sSQL + ' ON a.IDs = ' + @alias + '.IDs '
        FETCH NEXT FROM GetSQL_Cursor INTO @alias, @value
    END
CLOSE GetSQL_Cursor
DEALLOCATE GetSQL_Cursor
PRINT @sSQL
RETURN
GO



GO

/****** Object:  StoredProcedure [dbo].[UpdateWebinarToArchived]    Script Date: 1/12/2015 6:55:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:      sjh
-- Create date: 08/14
-- Description: Rolls over CUWebinars from Active to Recorded
-- =============================================
CREATE PROCEDURE [dbo].[UpdateWebinarToArchived] 
    -- Add the parameters for the stored procedure here
    @idWebinar int = 0
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    UPDATE dbo.Webinar SET Status = 4 WHERE GETDATE() > DATEADD(MONTH, 6, Date)

    
END
GO



GO

/****** Object:  StoredProcedure [dbo].[UpdateWebinarToRecorded]    Script Date: 1/12/2015 6:55:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      sjh
-- Create date: 08/14
-- Description: Rolls over CUWebinars from Active to Recorded
-- =============================================
CREATE PROCEDURE [dbo].[UpdateWebinarToRecorded] 
    -- Add the parameters for the stored procedure here
    @idWebinar int = 0, 
    @recording VARCHAR(50)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    UPDATE dbo.Webinar SET Status = 3, RecordingUrl = @recording WHERE idWebinar = @idWebinar

    UPDATE dbo.RegTypesGroupsXref SET idRegTypeGroup = 35 WHERE idWebinar = @idWebinar
    
END
GO


PRINT '-----------------Seeder '
PRINT 'finished '
SELECT TOP 1 date, LTRIM(RTRIM(Title)), idWebinar FROM BankWebinars.dbo.Webinar WHERE Status = 2 ORDER BY Date





