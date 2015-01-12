USE CUWebinars
GO
PRINT '-----------------2HOUR LIVE ONLY'
INSERT  dbo.RegType
        (idRegType, RegTypeExplain ,
          RegTypeLabel ,
          Price ,
          TaxExempt ,
          SortOrder ,
          SKU ,
          ShowLiveNotifications ,
          ShowRecordingNotifications ,
          ShowShippedNotifications ,
          Stage1CheckoutConfirmationMsg ,
          Stage2CheckoutConfirmationMsg ,
          Stage1EmailConfirmationMsg ,
          Stage2EmailConfirmationMsg
        )
VALUES  ( 205, ( SELECT  RegTypeExplain
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- RegTypeLabel - nvarchar(max)
          265 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    dbo.RegType
                    WHERE   idRegType = 1
                  ), '255_1', '265_205') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 1
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 1
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )

-----------------2HOUR ONDEMAND ONLY
INSERT  dbo.RegType
        (idRegType, RegTypeExplain ,
          RegTypeLabel ,
          Price ,
          TaxExempt ,
          SortOrder ,
          SKU ,
          ShowLiveNotifications ,
          ShowRecordingNotifications ,
          ShowShippedNotifications ,
          Stage1CheckoutConfirmationMsg ,
          Stage2CheckoutConfirmationMsg ,
          Stage1EmailConfirmationMsg ,
          Stage2EmailConfirmationMsg
        )
VALUES  ( 206, 'Interested in the topic but unable to attend all the scheduled events? Purchase the recorded version and receive OnDemand playback for 6 months following date of the event.' , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    dbo.RegType
            WHERE   idRegType = 2
          ) , -- RegTypeLabel - nvarchar(max)
          295 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    dbo.RegType
            WHERE   idRegType = 2
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    dbo.RegType
                    WHERE   idRegType = 2
                  ), '255_2', '295_206') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 2
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 2
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 2
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 2
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 2
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 2
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 2
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )


-----------------2HOUR LIVE ONLY
INSERT  dbo.RegType
        (idRegType, RegTypeExplain ,
          RegTypeLabel ,
          Price ,
          TaxExempt ,
          SortOrder ,
          SKU ,
          ShowLiveNotifications ,
          ShowRecordingNotifications ,
          ShowShippedNotifications ,
          Stage1CheckoutConfirmationMsg ,
          Stage2CheckoutConfirmationMsg ,
          Stage1EmailConfirmationMsg ,
          Stage2EmailConfirmationMsg
        )
VALUES  ( 207, ( SELECT  RegTypeExplain
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- RegTypeLabel - nvarchar(max)
          365 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    dbo.RegType
                    WHERE   idRegType = 3
                  ), '355_3', '365_207') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 3
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 3
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )

-----------------2HOUR CD-ROM and Hardcopy Handouts
INSERT  dbo.RegType
        (idRegType, RegTypeExplain ,
          RegTypeLabel ,
          Price ,
          TaxExempt ,
          SortOrder ,
          SKU ,
          ShowLiveNotifications ,
          ShowRecordingNotifications ,
          ShowShippedNotifications ,
          Stage1CheckoutConfirmationMsg ,
          Stage2CheckoutConfirmationMsg ,
          Stage1EmailConfirmationMsg ,
          Stage2EmailConfirmationMsg
        )
VALUES  ( 208, ( SELECT  RegTypeExplain
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- RegTypeLabel - nvarchar(max)
          325 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    dbo.RegType
                    WHERE   idRegType = 17
                  ), '295_17', '325_208') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 17
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 17
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )

INSERT  dbo.RegType
        (idRegType, RegTypeExplain ,
          RegTypeLabel ,
          Price ,
          TaxExempt ,
          SortOrder ,
          SKU ,
          ShowLiveNotifications ,
          ShowRecordingNotifications ,
          ShowShippedNotifications ,
          Stage1CheckoutConfirmationMsg ,
          Stage2CheckoutConfirmationMsg ,
          Stage1EmailConfirmationMsg ,
          Stage2EmailConfirmationMsg
        )
VALUES  ( 209, ( SELECT  RegTypeExplain
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- RegTypeLabel - nvarchar(max)
          395 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    dbo.RegType
                    WHERE   idRegType = 18
                  ), '395_18', '395_209') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 18
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    dbo.RegType
            WHERE   idRegType = 18
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )
------upgrades
INSERT  dbo.RegType
        (idRegType, RegTypeExplain ,
          RegTypeLabel ,
          Price ,
          TaxExempt ,
          SortOrder ,
          SKU ,
          ShowLiveNotifications ,
          ShowRecordingNotifications ,
          ShowShippedNotifications ,
          Stage1CheckoutConfirmationMsg ,
          Stage2CheckoutConfirmationMsg ,
          Stage1EmailConfirmationMsg ,
          Stage2EmailConfirmationMsg
        )
VALUES  ( 228, ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- RegTypeLabel - nvarchar(max)
          365 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- SortOrder - int
          'Upgrade_to_OnDemand_Weblinks_2Hr_365_228' , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 93
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )
		
INSERT  dbo.RegType
        (idRegType, RegTypeExplain ,
          RegTypeLabel ,
          Price ,
          TaxExempt ,
          SortOrder ,
          SKU ,
          ShowLiveNotifications ,
          ShowRecordingNotifications ,
          ShowShippedNotifications ,
          Stage1CheckoutConfirmationMsg ,
          Stage2CheckoutConfirmationMsg ,
          Stage1EmailConfirmationMsg ,
          Stage2EmailConfirmationMsg
        )
VALUES  ( 229, ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- RegTypeLabel - nvarchar(max)
          395 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 95
                  ), '395_95', '395_229') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 95
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )
