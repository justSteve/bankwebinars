
UPDATE  dbo.Presenter
SET     BiographyLong = REPLACE(BiographyLong, 'https://ttseast.blob.core.windows.net', 'http://images.ttstrain.com') ,
        PhotoFull = REPLACE(PhotoFull, 'https://ttseast.blob.core.windows.net', 'http://images.ttstrain.com') ,
        PhotoThumb = REPLACE(PhotoThumb, 'https://ttseast.blob.core.windows.net', 'http://images.ttstrain.com')
WHERE   idUser > 1


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
    @idWebinar INT ,
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
        UPDATE  [dbo].[Webinar]
        SET     [Status] = 7 ,
                [WebinarKey] = @WebinarKey ,
                [CitrixRegisterUrl] = @CitrixURL ,
                [AccessPhone] = @AccessPhone ,
                [AccessCodeAttendee] = @AccessCodeAttendee ,
                [AccessCodePresenter] = @AccessCodePresenter ,
                [AccessCodeOrganizer] = @AccessCodeOrganizer
        WHERE   idWebinar = @idWebinar

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
            ) ,
            r.idRegType ,
            ( SELECT    Title
              FROM      Webinar
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
    @sSQL VARCHAR(8000) OUTPUT
AS
    DECLARE @alias VARCHAR(5) ,
        @value VARCHAR(20)

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
    @idWebinar INT = 0
AS
    BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
        SET NOCOUNT ON;

        UPDATE  dbo.Webinar
        SET     Status = 4
        WHERE   GETDATE() > DATEADD(MONTH, 6, Date)

    
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
    @idWebinar INT = 0 ,
    @recording VARCHAR(50)
AS
    BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
        SET NOCOUNT ON;

        UPDATE  dbo.Webinar
        SET     Status = 3 ,
                RecordingUrl = @recording
        WHERE   idWebinar = @idWebinar

        UPDATE  dbo.RegTypesGroupsXref
        SET     idRegTypeGroup = 35
        WHERE   idWebinar = @idWebinar
    
    END
GO
USE BankWebinars
GO

DROP PROCEDURE updateRegGroups 
GO

CREATE PROCEDURE [dbo].[updateRegGroups]
AS
    BEGIN
        UPDATE  dbo.[Order]
        SET     idAffiliate = 383
        WHERE   idAffiliate = 395

        UPDATE  dbo.[Order]
        SET     idAffiliate = 62
        WHERE   idAffiliate = 379
	
--SELECT * from TTSWebinars2_.dbo.OptionsGroups WHERE  idWebinarRegTypeGroup = 27
--SELECT * FROM dbo.RegTypesGroupsXref WHERE idRegTypeGroup = 13

        UPDATE  dbo.Webinar
        SET     RecordingUrl = ''
        WHERE   idWebinar IN ( SELECT   idWebinar
                               FROM     dbo.Webinar
                               WHERE    Date > DATEADD(hh, -6, GETDATE())
                                        AND RecordingUrl != '' )



--change inprocess webinars to recorded
        UPDATE  Webinar
        SET     Status = 3 --Recorded
        WHERE   Date < DATEADD(hh, -6, GETDATE())
                AND Status = 7


--change scheduled webinars to recorded
        UPDATE  Webinar
        SET     Status = 3 --Recorded
        WHERE   Date < DATEADD(hh, -6, GETDATE())
                AND Status = 2

  --Scheduled

  --change recorded to archived
        UPDATE  Webinar
        SET     Status = 4 --ARCHIVED
        WHERE   ( DATEADD(MONTH, -6, DATEADD(hh, -6, GETDATE())) > [Date]
                  AND Status = 3
                ) 	
	 -- move compliance pers.
                OR ( [Date] < DATEADD(MONTH, -1, DATEADD(hh, -6, GETDATE()))
                     AND Status = 3
                   )
                AND Title LIKE '%Compliance Perspectives%'
  
  
  --SELECT * FROM TTSWebinars2.dbo.OptionsGroups

--legacy OPTIONS GROUPS
        UPDATE  RegTypesGroupsXref
        SET     idRegTypeGroup = 46
        WHERE   idRegTypeGroup = 45
                AND idWebinar IN ( SELECT   w.idWebinar
                                   FROM     Webinar w
                                   WHERE    w.Status = 3
                                            AND w.Duration = 1 )

        UPDATE  RegTypesGroupsXref
        SET     idRegTypeGroup = 44
        WHERE   idRegTypeGroup = 43
                AND idWebinar IN ( SELECT   w.idWebinar
                                   FROM     Webinar w
                                   WHERE    w.Status = 3
                                            AND w.Duration = 2 )

    END
GO
EXEC updateRegGroups


DELETE  FROM MembershipReboot.dbo.UserAccounts
WHERE   Email IN ( 'MigrateSteve@ttstrain.com', 'MigrateSteve1@ttstrain.com', 'MigrateSteve2@ttstrain.com',
                   'MigrateSteve3@ttstrain.com', 'steve@ttstrain.com', 'steve1@ttstrain.com', 'steve2@ttstrain.com',
                   'steve21@ttstrain.com' )

EXEC dbo.InsertGTWConnectionInfo @idWebinar = 1721, -- int
    @Status = 0, -- int
    @WebinarKey = N'116311563', -- nvarchar(max)
    @CitrixURL = N'https://attendee.gotowebinar.com/register/760641408465449217', -- nvarchar(max)
    @AccessPhone = N'1 866 952 8437', -- nvarchar(max)
    @AccessCodeAttendee = N'876-270-345', -- nvarchar(max)
    @AccessCodePresenter = N'149-015-753', -- nvarchar(max)
    @AccessCodeOrganizer = N'946-957-809'
 -- nvarchar(max)

EXEC dbo.InsertGTWConnectionInfo @idWebinar = 1721, -- int
    @Status = 0, -- int
    @WebinarKey = N'104748459', -- nvarchar(max)
    @CitrixURL = N' https://attendee.gotowebinar.com/register/9148449479400801282', -- nvarchar(max)
    @AccessPhone = N'1 866 901 6455', -- nvarchar(max)
    @AccessCodeAttendee = N'310-370-039', -- nvarchar(max)
    @AccessCodePresenter = N'393-790-834', -- nvarchar(max)
    @AccessCodeOrganizer = N'320-925-813'
 -- nvarchar(max)

UPDATE  dbo.Institution
SET     domainName = 'ttstrain.com'
WHERE   idInstitution = 3549
UPDATE  BankWebinars.dbo.Webinar
SET     OrganizerKey = '922930' ,
        OrganizerOAuthKey = 'A93DZ8hhX8JAqKNVAm04uZLDJckD'

UPDATE BankWebinars.dbo.Webinar SET OrganizerKey = '643333405148919814', OrganizerOAuthKey  = 'TJuriiJgiola6qAM0YC0GrcEYuHt'
UPDATE  dbo.RegType
SET     RegTypeExplain = REPLACE(RegTypeExplain, 'll have opportunity', 'll have an opportunity')

UPDATE  dbo.Presenter
SET     PhotoFull = '' ,
        PhotoThumb = '' ,
        BiographyLong = '<p><img class="alignleft" src="https://images.ttstrain.com/images/presenters/shelton.jpg" alt="Photo of Honey Shelton" />Honey Shelton brings the best of both worlds to her speaking and training engagements. She has 25 years of experience as a training and quality improvement consultant for banks and banking associations across the country. Her banking background includes spending three years as Executive Vice President/Chief Retail Banking Officer with First Victoria National Bank. </p><p>Nationally recognized as an outstanding speaker, over a half million bankers have participated in programs Honey has presented. Her depth of knowledge, enthusiasm, and compelling personality has left her lasting mark on InterAction Training, the firm she founded in 1983. </p><p>As a graduate of the School of Bank Marketing from the University of Colorado, she realizes the value of quality education. Honey invests time as a faculty member for banking schools around the country. She is a repeat presenter for most of the state banking associations. </p><p>Honey continues in her own personal pursuit of excellence and is a member of the American Society of Training and Development (ASTD). Currently she is pursuing a self study program on Six Sigma. She has obtained certification in Reality Therapy from the William Glasser Institute as well as certification from the Training and Development Program at Texas A &amp; M. </p>'
WHERE   idUser = 25

PRINT '-----------------Seeder '
PRINT 'finished '
SELECT TOP 1
        Date ,
        LTRIM(RTRIM(Title)) ,
        idWebinar
FROM    BankWebinars.dbo.Webinar
WHERE   Status = 2
ORDER BY Date

IF EXISTS (SELECT * FROM dbo.Webinar WHERE idWebinar NOT IN (SELECT idWebinar FROM dbo.AdditionalLocationsLookupPrice))
BEGIN 
PRINT '-----------------------------'
PRINT 'WARNING - ADDITIONAL LOCATIONS PRICES MISSING'
PRINT '-----------------------------'
PRINT '-----------------------------'
SELECT * FROM dbo.Webinar WHERE idWebinar NOT IN (SELECT idWebinar FROM dbo.AdditionalLocationsLookupPrice)
PRINT '-----------------------------'
PRINT 'WARNING - ADDITIONAL LOCATIONS PRICES MISSING'
PRINT '-----------------------------'
PRINT '-----------------------------'
END
UPDATE dbo.Discount SET DiscountType = 4 WHERE idDiscount > 0
UPDATE dbo.Discount SET DiscountType = 3 WHERE DiscountCode LIKE 'CP_%'
--SELECT  *
--FROM    dbo.Webinar
--WHERE   idWebinar NOT IN ( SELECT   idWebinar
--                           FROM     dbo.WebinarTopicXref )

--SELECT  *
--FROM    dbo.WebinarTopicXref
--WHERE   idWebinar NOT IN ( SELECT   idWebinar
--                           FROM     dbo.Webinar )

--SELECT  *
--FROM    dbo.RegTypesGroupsXref
--WHERE   idWebinar NOT IN ( SELECT   idWebinar
--                           FROM     dbo.Webinar )

--SELECT  *
--FROM    dbo.WebUser
--WHERE   UserType = 3
--        AND idUser NOT IN ( SELECT  idUser
--                            FROM    dbo.Presenter )
--        AND idUser NOT IN ( SELECT  idPresenter
--                            FROM    dbo.Webinar )

--SELECT  *
--FROM    dbo.WebUser
--WHERE   UserType = 2
--        AND idUser NOT IN ( SELECT  idUser
--                            FROM    dbo.Affiliate )
        