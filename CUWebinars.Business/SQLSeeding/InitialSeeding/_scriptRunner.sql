USE BankWebinars
GO


:setvar path "C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding"
go
:r $(path)\seeder1_ini.SQL
GO
--PRINT '_________________________________________________________________________________________begin presenters'
--go
--:r $(path)\seeder2_ImportPresenters.SQL
--GO
--PRINT '_________________________________________________________________________________________restore presenters'
--go
--:r $(path)\seeder2_RestorePresenters.SQL

--PRINT '_________________________________________________________________________________________restore presenters'
--go
--:r $(path)\seeder3_RestoreAffiliates.SQL
--GO
--PRINT '_________________________________________________________________________________________begin affiliates'
--go
--:r $(path)\seeder3_ImportAffiliates.SQL
--go
PRINT '_________________________________________________________________________________________begin options'
go
:r $(path)\seeder4_CreateOptions.SQL
UPDATE [dbo].[Options]
   SET [OptionExplain] = (select OptionExplain FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[OptionLabel] = (select OptionLabel FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[PriceToAdd] = (select PriceToAdd FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[TaxExempt] = (select TaxExempt FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[PercToAdd] = (select PercToAdd FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[SortOrder] = (select SortOrder FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[Type] = (select Type FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[MsgConfirm] = (select MsgConfirm FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[SKU] = (select SKU FROM TTSWebinars2.dbo.Options WHERE idRegType = 1)
      ,[ShowLiveNotifications] = 'Yes'
      ,[ShowRecordingNotifications] = 'No'
      ,[ShowShippedNotifications] = 'No'
      ,[Stage1CheckoutConfirmationMsg] = 'This order is for the live session only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) we''ll notify you by email.'
      ,[Stage2CheckoutConfirmationMsg] = 'This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.'
      ,[Stage1EmailConfirmationMsg] = 'The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) you''ll receive another email with complete connection instructions.'
      ,[Stage2EmailConfirmationMsg] = 'Connection information is summerized below and is also available at http://www.BankWebinars.com.'
 WHERE dbo.Options.idRegType = 84
GO

print '_________________________________________________________________________________________begin webinars'
--go
:r $(path)\seeder5_ImportWebinars.SQL
go
--print '_________________________________________________________________________________________begin CUWebinars'
--:r $(path)\seeder5_ImportWebinars__CU.SQL
--go
--PRINT '_________________________________________________________________________________________finish'
:r $(path)\seeder6_post.sql
GO
SET NOCOUNT OFF
USE BankWebinars
go
SELECT * FROM dbo.Affiliate
SELECT * FROM dbo.WebUser ORDER BY idUser desc
SELECT * FROM dbo.Addresses
--SELECT * FROM dbo.OptionsGroups
SELECT * FROM dbo.Presenter 

SELECT w.idWebinar ,* FROM dbo.Webinar w INNER JOIN dbo.OptionsGroupsXref ogx ON ogx.idWebinar = w.idWebinar
INNER JOIN dbo.OptionsGroups og ON og.idRegTypeGroup = ogx.idRegTypeGroup
INNER JOIN dbo.OptionsXref ox ON ox.idRegTypeGroup = og.idRegTypeGroup INNER JOIN dbo.Options o ON o.idRegType = ox.idRegType

WHERE w.idWebinar = 803

--SELECT idWebinar FROM dbo.HostPropertyValue
----USE master 
------go

----IF NOT EXISTS ( SELECT  *
----            FROM    sys.objects
--            WHERE   object_id = OBJECT_ID(N'[dbo].[CreateCUOrder]')
--                    AND type IN ( N'U' ) )

--CREATE PROCEDURE [dbo].[CreateCUOrder]
--    @idUser INT , -- UserID
--    @idWebinar INT ,
--    @idRegType INT ,
--    @idAffiliate INT = 19 ,
--    @idDiscount INT = NULL ,
--    @billStatus INT = 3
--AS
--BEGIN
--    SET NOCOUNT ON

--    INSERT  BankWebinars.dbo.ErrorLog
--            ( ErrorTime ,
--              UserName ,
--              ErrorNumber ,
--              ErrorSeverity ,
--              ErrorState ,
--              ErrorProcedure ,
--              ErrorLine ,
--              ErrorMessage
--	        )
--    VALUES  ( GETDATE() , -- ErrorTime - datetime
--               CONVERT(SYSNAME, CURRENT_USER) ,--'tracing' , -- UserName - sysname
--              0 , -- ErrorNumber - int
--              0 , -- ErrorSeverity - int
--              0 , -- ErrorState - int
--              N'[InsertOrder] logging the call' , -- ErrorProcedure - nvarchar(126)
--              0 , -- ErrorLine - int
--              CAST(@idUser AS VARCHAR) + ', ' + CAST(@idWebinar AS VARCHAR) + ', ' + CAST(@idRegType AS VARCHAR)
--	        )
--    DECLARE @ErrorLogID INT
--    DECLARE @percentOff DECIMAL
--    SET @percentOff = ( SELECT  percentOff
--                        FROM    Discounts
--                        WHERE   idDiscount = @idDiscount
--                      )

--    DECLARE @FlatOff DECIMAL

--    SET @FlatOff = ( SELECT flatOff
--                     FROM   Discounts
--                     WHERE  idDiscount = @idDiscount
--                   )
--    BEGIN TRY
--        INSERT  [Orders]
--                ( idUser ,
--                  orderDate ,
--                  Total ,
--                  FirstName ,
--                  lastName ,
--                  customerInstitution ,
--                  phone ,
--                  Email ,
--                  [address] , -- 9 deep
--                      --[address2] ,
--                  city ,
--                  [state] ,
--                  zip ,
--                  --shippingFirstName ,
--                  shippingLastName ,
--                  shippingAddress ,
--                      --shippingAddress2 ,
--                  shippingCity ,
--                  shippingState ,
--                  shippingZip ,
--                  auditInfo ,
--                  initiatedBy ,
--                  --shippingPhone ,
--                  idAffiliate ,
--                  paymentType ,
--                  taxExempt ,
--                  --Email ,
--                  Origin
--                )
--        VALUES  ( @idUser ,
--                  GETDATE() ,  --orderDate                  
--                  ( SELECT  PriceToAdd
--                    FROM    dbo.Options
--                    WHERE   idRegType = @idRegType
--                  ) ,
--                  ( SELECT  firstName
--                    FROM    WebUser
--                    WHERE   idUser = @idUser
--                  ) ,
--                  ( SELECT  lastName
--                    FROM    WebUser
--                    WHERE   idUser = @idUser
--                  ) ,
--                  ( SELECT  InstitutionName
--                    FROM    dbo.Institution
--                    WHERE   idInstitution = ( SELECT    idUserInstitution
--                                              FROM      dbo.WebUser
--                                              WHERE     idUser = @idUser
--                                            )
--                  ) 
--                  ,
--                  ( SELECT  Phone
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Billing'
--                  ) ,
--                  ( SELECT  email
--                    FROM    WebUser
--                    WHERE   idUser = @idUser
--                  )--email
--                  ,
--                  ( SELECT  [StreetAddress]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Billing'
--                  ) ,--address  9 deep
--                      --( SELECT  [StreetAddress2]
--                      --  FROM    dbo.[Addresses]
--                      --  WHERE   [WebUser_idUser]= @idUser
--                      --          AND AddressType = 'Billing'
--                      --) ,--address2
--                  ( SELECT  [City]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Billing'
--                  ) --city
--                  ,
--                  ( SELECT  [State]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Billing'
--                  ) ,--state
--                  ( SELECT  [Zip]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Billing'
--                  )   --zip
--                  ,
--                  ( SELECT  [Name]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Shipping'
--                  ) ,--address
--                  ( SELECT  [StreetAddress]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Shipping'
--                  ) ,--address
--                      --( SELECT  [StreetAddress2]
--                      --  FROM    dbo.[Addresses]
--                      --  WHERE   [WebUser_idUser]= @idUser
--                      --          AND AddressType = 'Shipping'
--                      --) ,--address2
--                  ( SELECT  [City]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Shipping'
--                  ) --city
--                  ,
--                  ( SELECT  [State]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Shipping'
--                  ) ,--state
--                  ( SELECT  [Zip]
--                    FROM    dbo.[Addresses]
--                    WHERE   [WebUser_idUser] = @idUser
--                            AND AddressType = 'Shipping'
--                  ) ,   --zip
--                  'sproc insertion' , --auditInfo
--                  4 , --initiatedBy                      
--                  19 ,--@AffiliateId ,
--                  '5' , -- paymentType
--                  0 ,  --taxExempt
--                  --'' ,
--                  8
--                )
  
--        DECLARE @newOrderID INT
--        SET @newOrderID = SCOPE_IDENTITY()
		
            
--    END TRY
--    BEGIN CATCH
--    -- Call procedure to print error information.
--        EXECUTE dbo.uspPrintError;

--    -- Roll back any active or uncommittable transactions before
--    -- inserting information in the ErrorLog.
--        IF XACT_STATE() <> 0
--            BEGIN
--                ROLLBACK TRANSACTION;
--            END

--        EXECUTE dbo.uspLogError @ErrorLogID = @ErrorLogID OUTPUT;
--    END CATCH; 

--    BEGIN TRY
--        INSERT  OrderRow
--                ( idOrder ,
--                  idWebinar ,
--                  unitPrice ,
--                  rowPrice ,
--                  idDiscount ,
--                  discountPercentOff ,
--                  discountFlatOff ,
--                  alternateEmail ,
--                  registrationType ,
--                  status ,
--                  Royalty ,
--                  isAdHocRecording
--                )
--        VALUES  ( @newOrderID ,
--                  @idWebinar ,
--                  ( SELECT  priceToAdd
--                    FROM    Options
--                    WHERE   idRegType = @idRegType
--                  ) ,
--                  ( SELECT  priceToAdd
--                    FROM    Options
--                    WHERE   idRegType = @idRegType
--                  ) ,
--                  @idDiscount ,
--                  ISNULL(@percentOff, 0) ,
--                  ISNULL(@flatOff, 0) ,
--                  ' ' ,
--                  @idRegType ,
--                  2 ,
--                  0 ,
--                  ''
--                )
--    END TRY
--    BEGIN CATCH
--    -- Call procedure to print error information.
--        EXECUTE dbo.uspPrintError;

--    -- Roll back any active or uncommittable transactions before
--    -- inserting information in the ErrorLog.
--        IF XACT_STATE() <> 0
--            BEGIN
--                ROLLBACK TRANSACTION;
--            END

--        EXECUTE dbo.uspLogError @ErrorLogID = @ErrorLogID OUTPUT;
--    END CATCH; 
--END
--BEGIN
--    SELECT  @newOrderID AS orderId
--END

--GO

