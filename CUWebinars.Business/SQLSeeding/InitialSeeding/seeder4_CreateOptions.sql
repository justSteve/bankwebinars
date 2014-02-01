USE BankWebinars
go

DELETE  BankWebinars.dbo.Topic
DELETE  BankWebinars.dbo.Options
DELETE  BankWebinars.dbo.OptionsGroups
DELETE  BankWebinars.dbo.OptionsGroupsXref
DELETE  BankWebinars.dbo.OptionsXref


SET NOCOUNT ON
PRINT '--==========--'
PRINT 'INSERT TOPICS'
PRINT '--==========--'

SET IDENTITY_INSERT dbo.Topic ON

INSERT  INTO [dbo].[Topic]
        ( idTopic ,
          [topicDesc] ,
          [TopicHTML] ,
          [SortOrder]
        )
        SELECT  id ,
                [TopicDescription] ,
                topicHTML ,
                sortOrder
        FROM    TTSWebinarsSeeder.dbo.Topic
	
SET IDENTITY_INSERT dbo.Topic OFF

PRINT '--==========--'
PRINT 'BEGINS OPTIONS HANDLING'
PRINT '--==========--'

SET IDENTITY_INSERT dbo.OptionsGroups ON

INSERT  INTO dbo.OptionsGroups
        ( [idOptionGroup] ,
          [OptionGroupDesc] ,
          [OptionType] ,
          [SortOrder]
        )
        SELECT  [idOptionGroup] ,
                [OptionGroupDesc] ,
                [OptionType] ,
                [SortOrder]
        FROM    TTSWebinarsSeeder.[dbo].[OptionsGroups]
  
SET IDENTITY_INSERT dbo.OptionsGroups OFF


PRINT '--==========--'
PRINT 'Begin UpdatedOptions migration'
PRINT '--==========--'

DECLARE my_Cursor CURSOR
FOR
SELECT  idOption
FROM    TTSWebinarsSeeder.dbo.Options
WHERE   idOption > 78


OPEN my_Cursor
DECLARE @id2InsertOption INT
--SET @id2InsertOption = 1
DECLARE @sku NVARCHAR(MAX)
DECLARE @Stage2EmailConfirmationMsg NVARCHAR(MAX)
DECLARE @Stage1EmailConfirmationMsg NVARCHAR(MAX)
DECLARE @Stage1CheckoutConfirmationMsg NVARCHAR(MAX)
DECLARE @Stage2CheckoutConfirmationMsg NVARCHAR(MAX)
DECLARE @ShowShippedNotifications NVARCHAR(MAX)
DECLARE @ShowRecordingNotifications NVARCHAR(MAX)
DECLARE @ShowLiveNotifications NVARCHAR(MAX)

FETCH NEXT FROM my_Cursor INTO @id2InsertOption
WHILE @@FETCH_STATUS = 0
    BEGIN
        SET IDENTITY_INSERT [Options] ON

        SET @sku = ( SELECT SKU
                     FROM   TTSWebinarsSeeder.dbo.Options
                     WHERE  idoption = @id2InsertOption
                   )
        SET @ShowLiveNotifications = ( SELECT   ShowLiveNotifications
                                       FROM     TTSWebinarsSeeder.dbo.Options
                                       WHERE    idoption = @id2InsertOption
                                     )
        SET @ShowRecordingNotifications = ( SELECT  ShowRecordingNotifications
                                            FROM    TTSWebinarsSeeder.dbo.Options
                                            WHERE   idoption = @id2InsertOption
                                          )
        SET @ShowShippedNotifications = ( SELECT    ShowShippedNotifications
                                          FROM      TTSWebinarsSeeder.dbo.Options
                                          WHERE     idoption = @id2InsertOption
                                        )
        SET @Stage1CheckoutConfirmationMsg = ( SELECT   Stage1CheckoutConfirmationMsg
                                               FROM     TTSWebinarsSeeder.dbo.Options
                                               WHERE    idoption = @id2InsertOption
                                             )
        SET @Stage2CheckoutConfirmationMsg = ( SELECT   Stage2CheckoutConfirmationMsg
                                               FROM     TTSWebinarsSeeder.dbo.Options
                                               WHERE    idoption = @id2InsertOption
                                             )
        SET @Stage1EmailConfirmationMsg = ( SELECT  Stage1EmailConfirmationMsg
                                            FROM    TTSWebinarsSeeder.dbo.Options
                                            WHERE   idoption = @id2InsertOption
                                          )
        SET @Stage2EmailConfirmationMsg = ( SELECT  Stage2EmailConfirmationMsg
                                            FROM    TTSWebinarsSeeder.dbo.Options
                                            WHERE   idoption = @id2InsertOption
                                          )



        INSERT  INTO [Options]
                ( idOption ,
                  [optionExplain] ,
                  [optionLabel] ,
                  [priceToAdd] ,
                  [SKU] ,
                  [percToAdd] ,
                  [sortOrder] ,
                  [Type] ,
                  [ShowLiveNotifications] ,
                  [ShowRecordingNotifications] ,
                  [ShowShippedNotifications] ,
                  [Stage1CheckoutConfirmationMsg] ,
                  [Stage2CheckoutConfirmationMsg] ,
                  [Stage1EmailConfirmationMsg] ,
                  [Stage2EmailConfirmationMsg] ,
                  [taxExempt] ,
                  [msgConfirm]
		        )
        VALUES  ( @id2InsertOption ,
                  ( SELECT  [optionExplain]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2InsertOption
                  ) ,
                  ( SELECT  [optionLabel]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2InsertOption
                  ) ,
                  ( SELECT  [priceToAdd]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2InsertOption
                  ) ,
                  ( SELECT  [SKU]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2InsertOption
                  ) ,
                  ( SELECT  [percToAdd]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2InsertOption
                  ) ,
                  ( SELECT  [sortOrder]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2InsertOption
                  ) ,
                  ( SELECT  [type]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2InsertOption
                  ) ,
                  @ShowLiveNotifications ,
                  @ShowRecordingNotifications ,
                  @ShowShippedNotifications ,
                  @Stage1CheckoutConfirmationMsg ,
                  @Stage2CheckoutConfirmationMsg ,
                  @Stage1EmailConfirmationMsg ,
                  @Stage2EmailConfirmationMsg ,
                  '' ,
                  ''
                )

        PRINT 'inserted: ' + CAST(@id2InsertOption AS VARCHAR) + ': [@Stage1CheckoutConfirmationMsg] = ' + @Stage1CheckoutConfirmationMsg

        FETCH NEXT FROM my_Cursor INTO @id2InsertOption
        SET IDENTITY_INSERT [Options]  OFF
    END
CLOSE my_Cursor
DEALLOCATE my_Cursor


INSERT  INTO BankWebinars.[dbo].[OptionsXref]
        ( idOptionGroup ,
          idOption                
        )
        SELECT  idOptionGroup ,
                idOption
        FROM    TTSWebinarsSeeder.dbo.OptionsXref

--PRINT 'inserted OptionsXref'

INSERT  INTO dbo.OptionsGroupsXref
        ( idWebinar ,
          idOptionGroup
        )
        SELECT  idWebinar ,
                idOptionGroup
        FROM    TTSWebinarsSeeder.dbo.OptionsGroupsXref