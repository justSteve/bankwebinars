USE CUWebinars
GO
PRINT '-----------------1HOUR LIVE ONLY'
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
VALUES  ( 200 ,
          ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- RegTypeLabel - nvarchar(max)
          165 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 97
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 97
                  ), '155', '165') , -- SKU - nvarchar(max)
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
VALUES  ( 201 ,
          'Interested in the topic but unable to attend all the scheduled events? Purchase the recorded version and receive OnDemand playback for 6 months following date of the event.' , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- RegTypeLabel - nvarchar(max)
          185 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 98
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 98
                  ), '155_98', '185_201') , -- SKU - nvarchar(max)
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
VALUES  ( 202 ,
          ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- RegTypeLabel - nvarchar(max)
          235 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 100
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 100
                  ), '225_100', '235_202') , -- SKU - nvarchar(max)
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
VALUES  ( 203 ,
          ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- RegTypeLabel - nvarchar(max)
          215 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 99
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 99
                  ), '195_99', '215_203') , -- SKU - nvarchar(max)
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
VALUES  ( 204 ,
          ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- RegTypeLabel - nvarchar(max)
          265 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 101
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 101
                  ), '255_101', '265_204') , -- SKU - nvarchar(max)
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
VALUES  ( 226 ,
          ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- RegTypeLabel - nvarchar(max)
          235 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 105
                  ), '225_105', '235_226') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 105
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
VALUES  ( 227 ,
          ( SELECT  RegTypeExplain
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- RegTypeExplain - nvarchar(max)
          ( SELECT  RegTypeLabel
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- RegTypeLabel - nvarchar(max)
          265 , -- Price - float
          0 , -- TaxExempt - bit
          ( SELECT  SortOrder
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- SortOrder - int
          REPLACE(( SELECT  SKU
                    FROM    CUWebinars.dbo.RegType
                    WHERE   idRegType = 107
                  ), '255_107', '265_227') , -- SKU - nvarchar(max)
          ( SELECT  ShowLiveNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- ShowLiveNotifications - nvarchar(max)
          ( SELECT  ShowRecordingNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- ShowRecordingNotifications - nvarchar(max)
          ( SELECT  ShowShippedNotifications
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- ShowShippedNotifications - nvarchar(max)
          ( SELECT  Stage1CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- Stage1CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2CheckoutConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
          ( SELECT  Stage1EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
          ( SELECT  Stage2EmailConfirmationMsg
            FROM    CUWebinars.dbo.RegType
            WHERE   idRegType = 107
          )   -- Stage2EmailConfirmationMsg - nvarchar(max)
        )

