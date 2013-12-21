USE TTSWebinars
go

SET NOCOUNT ON
DECLARE @id2Insert INT


SET IDENTITY_INSERT dbo.Topic ON

INSERT  INTO [dbo].[Topic]
        (idTopic, [topicDesc] ,
          [TopicHTML] ,
          [SortOrder]
        )
        SELECT id, [TopicDescription],
                topicHTML ,
                sortOrder
        FROM    TTSWebinarsSeeder.dbo.Topic
	
SET IDENTITY_INSERT dbo.Topic OFF

PRINT '--==========--'
PRINT 'BEGINS OPTIONS HANDLING'
PRINT '--==========--'

SET IDENTITY_INSERT dbo.OptionsGroups ON
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 32 ,
          'BW_13_PreEvent_2hr' ,
          'T' ,
          1
        )
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 33 ,
          'BW_13_PostEvent_2hr' ,
          'T' ,
          2
        )
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 34 ,
          'BW_13_PreEvent_1hr' ,
          'T' ,
          3
        )
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 35 ,
          'BW_13_PostEvent_1hr' ,
          'T' ,
          4
        )
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 36 ,
          'BW_13_Subscription' ,
          'T' ,
          5
        )
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 37 ,
          'BW_13_PreEvent_Series3' ,
          'T' ,
          6
        )
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 38 ,
          'BW_13_PostEvent_Series3' ,
          'T' ,
          7
        )
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 39 ,
          'BW_13_PreEvent_Series4' ,
          'T' ,
          7
        )
INSERT  INTO [dbo].[OptionsGroups]
        ( idOptionGroup ,
          [optionGroupDesc] ,
          [optionType] ,
          [sortOrder]
        )
VALUES  ( 40 ,
          'BW_13_PostEvent_Series4' ,
          'T' ,
          7
        )

SET IDENTITY_INSERT dbo.OptionsGroups OFF


PRINT '--==========--'
PRINT 'Begin UpdatedOptions migration'
PRINT '--==========--'

DECLARE my_Cursor CURSOR
FOR
    SELECT  idOption
    FROM    TTSWebinarsSeeder.dbo.Options
    WHERE   idOption > 78
--SKU is null

OPEN my_Cursor
--DECLARE @id2Insert INT
SET @id2Insert = 1
DECLARE @sku NVARCHAR(MAX)
DECLARE @Stage2EmailConfirmationMsg NVARCHAR(MAX)
DECLARE @Stage1EmailConfirmationMsg NVARCHAR(MAX)
DECLARE @Stage1CheckoutConfirmationMsg NVARCHAR(MAX)
DECLARE @Stage2CheckoutConfirmationMsg NVARCHAR(MAX)
DECLARE @ShowShippedNotifications NVARCHAR(MAX)
DECLARE @ShowRecordingNotifications NVARCHAR(MAX)
DECLARE @ShowLiveNotifications NVARCHAR(MAX)

FETCH NEXT FROM my_Cursor INTO @id2Insert
WHILE @@FETCH_STATUS = 0 
    BEGIN
        SET IDENTITY_INSERT [Options] ON

        SET @sku = ( SELECT SKU
                     FROM   TTSWebinarsSeeder.dbo.Options
                     WHERE  idoption = @id2Insert
                   )
        SET @ShowLiveNotifications = ( SELECT   ShowLiveNotifications
                                       FROM     TTSWebinarsSeeder.dbo.Options
                                       WHERE    idoption = @id2Insert
                                     )
        SET @ShowRecordingNotifications = ( SELECT  ShowRecordingNotifications
                                            FROM    TTSWebinarsSeeder.dbo.Options
                                            WHERE   idoption = @id2Insert
                                          )
        SET @ShowShippedNotifications = ( SELECT    ShowShippedNotifications
                                          FROM      TTSWebinarsSeeder.dbo.Options
                                          WHERE     idoption = @id2Insert
                                        )
        SET @Stage1CheckoutConfirmationMsg = ( SELECT   Stage1CheckoutConfirmationMsg
                                               FROM     TTSWebinarsSeeder.dbo.Options
                                               WHERE    idoption = @id2Insert
                                             )
        SET @Stage2CheckoutConfirmationMsg = ( SELECT   Stage2CheckoutConfirmationMsg
                                               FROM     TTSWebinarsSeeder.dbo.Options
                                               WHERE    idoption = @id2Insert
                                             )
        SET @Stage1EmailConfirmationMsg = ( SELECT  Stage1EmailConfirmationMsg
                                            FROM    TTSWebinarsSeeder.dbo.Options
                                            WHERE   idoption = @id2Insert
                                          )
        SET @Stage2EmailConfirmationMsg = ( SELECT  Stage2EmailConfirmationMsg
                                            FROM    TTSWebinarsSeeder.dbo.Options
                                            WHERE   idoption = @id2Insert
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
        VALUES  ( @id2Insert ,
                  ( SELECT  [optionExplain]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2Insert
                  ) ,
                  ( SELECT  [optionLabel]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2Insert
                  ) ,
                  ( SELECT  [priceToAdd]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2Insert
                  ) ,
                  ( SELECT  [SKU]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2Insert
                  ) ,
                  ( SELECT  [percToAdd]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2Insert
                  ) ,
                  ( SELECT  [sortOrder]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2Insert
                  ) ,
                  ( SELECT  [type]
                    FROM    TTSWebinarsSeeder.dbo.Options
                    WHERE   idoption = @id2Insert
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

        --PRINT 'inserted: ' + CAST(@id2Insert AS VARCHAR)
        --    + '[@Stage1CheckoutConfirmationMsg] = '
        --    + @Stage1CheckoutConfirmationMsg

        FETCH NEXT FROM my_Cursor INTO @id2Insert
        SET IDENTITY_INSERT [Options]  OFF
    END
CLOSE my_Cursor
DEALLOCATE my_Cursor


INSERT  INTO [dbo].[OptionsXref]
        ( idOptionGroup ,
          idOption                
        )
        SELECT  idOptionGroup ,
                idOption
        FROM    TTSWebinarsSeeder.dbo.OptionsXref

--PRINT 'inserted OptionsXref'

