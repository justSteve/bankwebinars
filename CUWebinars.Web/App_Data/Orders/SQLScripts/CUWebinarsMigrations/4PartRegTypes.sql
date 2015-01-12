USE CUWebinars
GO
PRINT '-----------------4-Part Series LIVE ONLY'
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
VALUES  ( 216, 'Attend all sessions live with the opportunity to ask questions of the presenter.  You also receive handouts.' , -- RegTypeExplain - nvarchar(max)
          'Live Sessions Only' , -- RegTypeLabel - nvarchar(max)
          899 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- SortOrder - int
          'Live_Session_Only_4Part_899_216' , -- SKU - nvarchar(max)
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
VALUES  ( 217, 'Interested in the topic but unable to attend all the scheduled events? Purchase the recorded version and receive OnDemand playback for 6 months following date of the event.' , -- RegTypeExplain - nvarchar(max)
          'OnDemand Recordings Only' , -- RegTypeLabel - nvarchar(max)
          899 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- SortOrder - int
          'OnDemand_Only_4Part_899_217' , -- SKU - nvarchar(max)
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
VALUES  ( 218, 'Attend the live events plus receive 6-months of OnDemand playback and electronic handouts.' , -- RegTypeExplain - nvarchar(max)
          'Live Plus OnDemand Weblinks' , -- RegTypeLabel - nvarchar(max)
          1249 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- SortOrder - int
          'Live_Plus_OnDemand_4Part_1249_218' , -- SKU - nvarchar(max)
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
VALUES  ( 219, 'CD-ROM plus Hardcopy Handouts. Also includes free 6-Month OnDemand Weblinks.  Does not include live sessions. ' , -- RegTypeExplain - nvarchar(max)
          'CD-ROM and Hardcopy Handouts' , -- RegTypeLabel - nvarchar(max)
          979 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- SortOrder - int
          'CDROM_Only_4Part_979_219' , -- SKU - nvarchar(max)
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
VALUES  ( 220, 'Includes it all! Live Sessions; OnDemand Weblinks; and CD-Rom plus Hardcopy Handouts.' , -- RegTypeExplain - nvarchar(max)
          'Premier Package' , -- RegTypeLabel - nvarchar(max)
          1399 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- SortOrder - int
          'Premier_4Part_1399_220' , -- SKU - nvarchar(max)
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
VALUES  ( 232, ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- RegTypeLabel - nvarchar(max)
          1249 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 132
                  ), '1249_132', '1249_232') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
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
VALUES  ( 233, ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- RegTypeLabel - nvarchar(max)
          1399 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 133
                  ), '1399_133', '1399_233') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
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
PRINT '-----------------4-PART SERIES LIVE ONLY'
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