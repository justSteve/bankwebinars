USE BankWebinars
GO
PRINT '-----------------3-Part Series LIVE ONLY'
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
VALUES  ( 210, 'Attend all sessions live with the opportunity to ask questions of the presenter.  You also receive handouts.' , -- RegTypeExplain - nvarchar(max)
          'Live Session Only' , -- RegTypeLabel - nvarchar(max)
          699 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- SortOrder - int
          'Live_Session_Only_3Part_699_210' , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
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
VALUES  ( 211, 'Interested in the topic but unable to attend all the scheduled events? Purchase the recorded version and receive OnDemand playback for 6 months following date of the event.' , -- RegTypeExplain - nvarchar(max)
          'OnDemand Recording Only' , -- RegTypeLabel - nvarchar(max)
          699 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- SortOrder - int
          'OnDemand_Only_3Part_699_211' , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
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
VALUES  ( 212, 'Attend the live events plus receive 6-months of OnDemand playback and electronic handouts.' , -- RegTypeExplain - nvarchar(max)
          'Live Plus OnDemand Weblinks' , -- RegTypeLabel - nvarchar(max)
          895 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- SortOrder - int
          'Live_Plus_OnDemand_3Part_895_212' , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
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
VALUES  ( 213, 'CD-ROM plus Hardcopy Handouts. Also includes free 6-Month OnDemand Weblinks.  Does not include live sessions. ' , -- RegTypeExplain - nvarchar(max)
          'CD-ROM and Hardcopy Handouts' , -- RegTypeLabel - nvarchar(max)
          795 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- SortOrder - int
          'CDROM_Only_3Part_795_213' , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
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
VALUES  ( 214, 'Includes it all! Live Sessions; OnDemand Weblinks; and CD-Rom plus Hardcopy Handouts.' , -- RegTypeExplain - nvarchar(max)
          'Premier Package' , -- RegTypeLabel - nvarchar(max)
          995 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- SortOrder - int
          'Premier_3Part_995_214' , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
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
VALUES  ( 230, ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- RegTypeLabel - nvarchar(max)
          1249 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- SortOrder - int
          'Upgrade_to_OnDemand_Weblinks_Series3_1249_230' , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 116
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
VALUES  ( 231, ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- RegTypeLabel - nvarchar(max)
          1399 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 117
                  ), '1399_117', '1399_231') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 117
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )
		PRINT ''
		PRINT ''
		PRINT ''
		PRINT ''
		PRINT ''
		PRINT ''
		PRINT ''
		PRINT ''
		PRINT ''
		PRINT ''
PRINT '==================='
PRINT '-----------------3-PART SERIES LIVE ONLY'
PRINT 'finished '
        PRINT ''
        PRINT ''
        PRINT ''
        PRINT ''
        PRINT ''
        PRINT ''
        PRINT ''
        PRINT ''
        PRINT ''
        PRINT ''