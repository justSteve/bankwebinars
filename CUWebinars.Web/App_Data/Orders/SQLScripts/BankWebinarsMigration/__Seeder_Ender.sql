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
USE BankWebinars
GO
 CREATE PROCEDURE updateRegGroups AS begin
UPDATE  dbo.[Order]
SET     idAffiliate = 383
WHERE   idAffiliate = 395

UPDATE  dbo.[Order]
SET     idAffiliate = 62
WHERE   idAffiliate = 379
	
--SELECT * from TTSWebinars2_.dbo.OptionsGroups WHERE  idWebinarRegTypeGroup = 27
--SELECT * FROM dbo.RegTypesGroupsXref WHERE idRegTypeGroup = 13

UPDATE dbo.Webinar SET recordingUrl = '' WHERE idWebinar IN (SELECT idWebinar FROM	dbo.Webinar WHERE date > GETDATE() AND recordingUrl != '')



--change scheduled webinars to recorded
UPDATE  Webinar
SET     status = 3 --Recorded
WHERE   date < GETDATE()
        AND status = 2
  --Scheduled

  --change recorded to archived
UPDATE  Webinar
SET     status = 4 --ARCHIVED
WHERE   ( DATEADD(MONTH, -6, GETDATE()) > [date]
          AND status = 3
        ) 	
	 -- move compliance pers.
        OR ( [date] < DATEADD(MONTH, -1, GETDATE())
             AND status = 3
           )
        AND title LIKE '%Compliance Perspectives%'
  
  
  --SELECT * FROM TTSWebinars2.dbo.OptionsGroups

--legacy OPTIONS GROUPS
UPDATE  RegTypesGroupsXref
SET      idRegTypeGroup = 46
WHERE    idRegTypeGroup = 45
        AND idWebinar IN ( SELECT   w.idWebinar
                           FROM     Webinar w
                           WHERE    w.status = 3
                                    AND w.duration = 1 )

UPDATE  RegTypesGroupsXref
SET      idRegTypeGroup = 44
WHERE    idRegTypeGroup = 43
        AND idWebinar IN ( SELECT   w.idWebinar
                           FROM     Webinar w
                           WHERE    w.status = 3
                                    AND w.duration = 2 )

END 
GO

EXEC updateRegGroups

DELETE FROM MembershipReboot.dbo.UserAccounts WHERE email IN ('MigrateSteve@ttstrain.com' ,'MigrateSteve1@ttstrain.com' ,'MigrateSteve2@ttstrain.com','MigrateSteve3@ttstrain.com','steve@ttstrain.com','steve1@ttstrain.com','steve2@ttstrain.com','steve21@ttstrain.com')

EXEC dbo.InsertGTWConnectionInfo @idWebinar = 1719, -- int
    @Status = 0, -- int
    @WebinarKey = N'127193450', -- nvarchar(max)
    @CitrixURL = N'https://attendee.gotowebinar.com/register/100000000065846026', -- nvarchar(max)
    @AccessPhone = N'877 309 2074', -- nvarchar(max)
    @AccessCodeAttendee = N'283-313-444', -- nvarchar(max)
    @AccessCodePresenter = N'626-564-899', -- nvarchar(max)
    @AccessCodeOrganizer = N'421-322-267' -- nvarchar(max)

UPDATE BankWebinars.dbo.Webinar SET OrganizerKey = '922930', OrganizerOAuthKey  = 'A93DZ8hhX8JAqKNVAm04uZLDJckD'
UPDATE BankWebinars.dbo.Webinar SET OrganizerKey = '643333405148919814', OrganizerOAuthKey  = 'bcDzWKEHfnJAWb9FQGCRcAwuqf25'

PRINT '-----------------Seeder '
PRINT 'finished '
SELECT TOP 1 date, LTRIM(RTRIM(Title)), idWebinar FROM BankWebinars.dbo.Webinar WHERE Status = 2 ORDER BY Date





