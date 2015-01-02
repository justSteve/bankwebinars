USE BankWebinars
GO
PRINT '-----------------5-PART SERIES LIVE ONLY'
INSERT  dbo.RegType
        ( idRegType ,
          RegTypeExplain ,
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
VALUES  ( 221 ,
          'Attend all sessions live with the opportunity to ask questions of the presenter.  You also receive handouts.' , -- REGTYPEEXPLAIN - NVARCHAR(MAX)
          'Live Sessions Only' , -- REGTYPELABEL - NVARCHAR(MAX)
          1145 , -- PRICE - FLOAT
          0 , -- TAXEXEMPT - BIT
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- SORTORDER - INT
          'Live_session_only_5part_1145_221' , -- SKU - NVARCHAR(MAX)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- SHOWLIVENOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- SHOWRECORDINGNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- SHOWSHIPPEDNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- STAGE1CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- STAGE2CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- STAGE1EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          )   -- STAGE2EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
        )


INSERT  dbo.RegType
        ( idRegType ,
          RegTypeExplain ,
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
VALUES  ( 222 ,
          'Interested in the topic but unable to attend all the scheduled events? Purchase the recorded version and receive ondemand playback for 6 months following date of the event.' , -- regtypeexplain - nvarchar(max)
          'OnDemand Recordings Only' , -- REGTYPELABEL - NVARCHAR(MAX)
          1385 , -- PRICE - FLOAT
          0 , -- TAXEXEMPT - BIT
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- SORTORDER - INT
          'OnDemand_1385_222' , -- SKU - NVARCHAR(MAX)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- SHOWLIVENOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- SHOWRECORDINGNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- SHOWSHIPPEDNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- STAGE1CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- STAGE2CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- STAGE1EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          )   -- STAGE2EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
        )



INSERT  dbo.RegType
        ( idRegType ,
          RegTypeExplain ,
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
VALUES  ( 223 ,
          'Attend the live events plus receive 6-months of ondemand playback and electronic handouts.' , -- REGTYPEEXPLAIN - NVARCHAR(MAX)
          'Live Plus Ondemand Weblinks' , -- REGTYPELABEL - NVARCHAR(MAX)
          1555 , -- PRICE - FLOAT
          0 , -- TAXEXEMPT - BIT
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- SORTORDER - INT
          'LIVE_PLUS_ONDEMAND_5PART_1555_223' , -- SKU - NVARCHAR(MAX)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- SHOWLIVENOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- SHOWRECORDINGNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- SHOWSHIPPEDNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- STAGE1CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- STAGE2CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- STAGE1EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          )   -- STAGE2EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
        )


INSERT  dbo.RegType
        ( idRegType ,
          RegTypeExplain ,
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
VALUES  ( 224 ,
          'CD-ROM plus hardcopy handouts. Also includes free 6-month OnDemand weblinks.  Does not include live sessions. ' , -- REGTYPEEXPLAIN - NVARCHAR(MAX)
          'CD-ROM and Hardcopy Handouts' , -- REGTYPELABEL - NVARCHAR(MAX)
          1385 , -- PRICE - FLOAT
          0 , -- TAXEXEMPT - BIT
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- SORTORDER - INT
          'CDROM_ONLY_5PART_1385_224' , -- SKU - NVARCHAR(MAX)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- SHOWLIVENOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- SHOWRECORDINGNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- SHOWSHIPPEDNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- STAGE1CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- STAGE2CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- STAGE1EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          )   -- STAGE2EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
        )

INSERT  dbo.RegType
        ( idRegType ,
          RegTypeExplain ,
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
VALUES  ( 225 ,
          'Includes it all! Live sessions; OnDemand weblinks; and CD-ROM plus hardcopy handouts.' , -- REGTYPEEXPLAIN - NVARCHAR(MAX)
          'Premier Package' , -- REGTYPELABEL - NVARCHAR(MAX)
          1745 , -- PRICE - FLOAT
          0 , -- TAXEXEMPT - BIT
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- SORTORDER - INT
          'PREMIER_5PART_1745_225' , -- SKU - NVARCHAR(MAX)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- SHOWLIVENOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- SHOWRECORDINGNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- SHOWSHIPPEDNOTIFICATIONS - NVARCHAR(MAX)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- STAGE1CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- STAGE2CHECKOUTCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- STAGE1EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          )   -- STAGE2EMAILCONFIRMATIONMSG - NVARCHAR(MAX)
        )


------upgrades
INSERT  dbo.RegType
        ( idRegType ,
          RegTypeExplain ,
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
VALUES  ( 234 ,
          ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- RegTypeLabel - nvarchar(max)
          1555 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 132
          ) , -- SortOrder - int
          'Upgrade_to_OnDemand_Weblinks_Series5_1555_234' , -- SKU - nvarchar(max)
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
        ( idRegType ,
          RegTypeExplain ,
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
VALUES  ( 235 ,
          ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- RegTypeLabel - nvarchar(max)
          1745 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 133
          ) , -- SortOrder - int
          'Upgrade_to_Premier_Series5_1745_235' , -- SKU - nvarchar(max)
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
PRINT '-----------------5-PART SERIES LIVE ONLY'
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