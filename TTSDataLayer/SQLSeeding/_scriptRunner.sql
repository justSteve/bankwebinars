USE TTSWebinars
GO


:setvar path "C:\Users\Steve\Source\Repos\CUWebinars\TTSDataLayer\SQLSeeding"
go
:r $(path)\seeder1_ini.SQL
go
PRINT '_________________________________________________________________________________________begin presenters'
go
:r $(path)\seeder2_ImportPresenters.SQL
GO
PRINT '_________________________________________________________________________________________begin affiliates'
go
:r $(path)\seeder3_ImportAffiliates.SQL
go
PRINT '_________________________________________________________________________________________begin options'
go
:r $(path)\seeder4_CreateOptions.SQL
print '_________________________________________________________________________________________begin webinars'
go
:r $(path)\seeder5_ImportWebinars.SQL
go
print '_________________________________________________________________________________________begin CUWebinars'
:r $(path)\seeder5_ImportWebinars__CU.SQL
go
PRINT '_________________________________________________________________________________________finish'
:r $(path)\seeder6_post.sql
GO
SET NOCOUNT OFF
USE TTSWebinars
go
SELECT * FROM dbo.Affiliate
SELECT * FROM dbo.WebUser ORDER BY idUser desc
SELECT * FROM dbo.Addresses
SELECT * FROM dbo.OptionsGroups
SELECT * FROM dbo.Presenter 

SELECT w.idWebinar ,* FROM dbo.Webinar w INNER JOIN dbo.OptionsGroupsXref ogx ON ogx.idWebinar = w.idWebinar
INNER JOIN dbo.OptionsGroups og ON og.idOptionGroup = ogx.idOptionGroup
INNER JOIN dbo.OptionsXref ox ON ox.idOptionGroup = og.idOptionGroup INNER JOIN dbo.Options o ON o.idOption = ox.idOption
INNER JOIN dbo.HostPropertyValue hp ON hp.idWebinar = w.idWebinar
INNER JOIN dbo.HostProperty h ON h.idHostProperty = hp.idHostProperty

WHERE w.idWebinar = 803

SELECT idWebinar FROM dbo.HostPropertyValue
--USE master 
----go

--IF NOT EXISTS ( SELECT  *
--            FROM    sys.objects
--            WHERE   object_id = OBJECT_ID(N'[dbo].[CreateCUOrder]')
--                    AND type IN ( N'U' ) )

--CREATE PROCEDURE [dbo].[CreateCUOrder]
--    @idUser INT , -- UserID
--    @idWebinar INT ,
--    @idOption INT ,
--    @idAffiliate INT = 19 ,
--    @idDiscount INT = NULL ,
--    @billStatus INT = 3
--AS
--BEGIN
--    SET NOCOUNT ON

--    INSERT  TTSWebinars.dbo.ErrorLog
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
--              CAST(@idUser AS VARCHAR) + ', ' + CAST(@idWebinar AS VARCHAR) + ', ' + CAST(@idOption AS VARCHAR)
--	        )
--    DECLARE @ErrorLogID INT
--    DECLARE @percentOff DECIMAL
--    SET @percentOff = ( SELECT  percentOff
--                        FROM    Discounts
--                        WHERE   idDiscounts = @idDiscount
--                      )

--    DECLARE @FlatOff DECIMAL

--    SET @FlatOff = ( SELECT flatOff
--                     FROM   Discounts
--                     WHERE  idDiscounts = @idDiscount
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
--                    WHERE   idOption = @idOption
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
--                    WHERE   idOption = @idOption
--                  ) ,
--                  ( SELECT  priceToAdd
--                    FROM    Options
--                    WHERE   idOption = @idOption
--                  ) ,
--                  @idDiscount ,
--                  ISNULL(@percentOff, 0) ,
--                  ISNULL(@flatOff, 0) ,
--                  ' ' ,
--                  @idOption ,
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

