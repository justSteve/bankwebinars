
--CREATE TABLE BWMigrator.[dbo].[Options]
--(
--[idOption] [int] ,
--[OptionExplain] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[OptionLabel] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[PriceToAdd] [float] NULL,
--[TaxExempt] [bit] NULL,
--[PercToAdd] [float] NULL,
--[SortOrder] [int] NULL,
--[Type] [nvarchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
--[MsgConfirm] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[SKU] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[ShowLiveNotifications] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[ShowRecordingNotifications] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[ShowShippedNotifications] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[Stage1CheckoutConfirmationMsg] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[Stage2CheckoutConfirmationMsg] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[Stage1EmailConfirmationMsg] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
--[Stage2EmailConfirmationMsg] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
--GO
--INSERT BWMigrator.dbo.Options
--        ( idOption ,
--          OptionExplain ,
--          OptionLabel ,
--          PriceToAdd ,
--          TaxExempt ,
--          PercToAdd ,
--          SortOrder ,
--          Type ,
--          MsgConfirm ,
--          SKU ,
--          ShowLiveNotifications ,
--          ShowRecordingNotifications ,
--          ShowShippedNotifications ,
--          Stage1CheckoutConfirmationMsg ,
--          Stage2CheckoutConfirmationMsg ,
--          Stage1EmailConfirmationMsg ,
--          Stage2EmailConfirmationMsg
--        )
--select idOption ,
--          OptionExplain ,
--          OptionLabel ,
--          PriceToAdd ,
--          TaxExempt ,
--          PercToAdd ,
--          SortOrder ,
--          Type ,
--          MsgConfirm ,
--          SKU ,
--          ShowLiveNotifications ,
--          ShowRecordingNotifications ,
--          ShowShippedNotifications ,
--          Stage1CheckoutConfirmationMsg ,
--          Stage2CheckoutConfirmationMsg ,
--          Stage1EmailConfirmationMsg ,
--          Stage2EmailConfirmationMsg
--        FROM TTSWebinars2_testing.dbo.Options


	
    SET IDENTITY_INSERT  BWClean.dbo.Options ON
    INSERT  BWClean.dbo.Options
            ( idOption ,
              OptionExplain ,
              OptionLabel ,
              PriceToAdd ,
              TaxExempt ,
              PercToAdd ,
              SortOrder ,
              Type ,
              MsgConfirm ,
              SKU ,
              ShowLiveNotifications ,
              ShowRecordingNotifications ,
              ShowShippedNotifications ,
              Stage1CheckoutConfirmationMsg ,
              Stage2CheckoutConfirmationMsg ,
              Stage1EmailConfirmationMsg ,
              Stage2EmailConfirmationMsg
            )
            SELECT  o.idOption ,
                    OptionExplain ,
                    OptionLabel ,
                    PriceToAdd ,
                    TaxExempt ,
                    PercToAdd ,
                    o.SortOrder ,
                    Type ,
                    MsgConfirm ,
                    SKU ,
                    ShowLiveNotifications ,
                    ShowRecordingNotifications ,
                    ShowShippedNotifications ,
                    Stage1CheckoutConfirmationMsg ,
                    Stage2CheckoutConfirmationMsg ,
                    Stage1EmailConfirmationMsg ,
                    Stage2EmailConfirmationMsg
            FROM    BWMigrator.dbo.Options o
                    INNER JOIN BWMigrator.dbo.OptionsXref ox ON ox.idOption = o.idOption
                    INNER JOIN BWMigrator.dbo.OptionsGroups og ON og.idOptionGroup = ox.idOptionGroup
            WHERE   o.optionLabel NOT LIKE 'Additional Location(s)'
                    --AND og.idOptionGroup IN (15,23,24,25,26,27,28,29,30,32,33)
		 ORDER BY o.idOption
    SET IDENTITY_INSERT  BWClean.dbo.Options OFF


	SELECT * FROM BWMigrator.dbo.Options