
USE [master]
go
ALTER DATABASE CUWebinars SET SINGLE_USER WITH ROLLBACK IMMEDIATE
BACKUP LOG [CUWebinarsClean] TO  DISK = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\Backup\CUWebinarsClean_LogBackup_2014-04-10_07-16-52.bak' WITH NOFORMAT, NOINIT,  NAME = N'CUWebinarsClean_LogBackup_2014-04-10_07-16-52', NOSKIP, NOREWIND, NOUNLOAD,  NORECOVERY ,  STATS = 5
RESTORE DATABASE [CUWebinars] FROM  DISK = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\Backup\CUWebinarsClean.bak' WITH  FILE = 1,  MOVE N'CUWebinarsClean_Data' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\CUWebinars',  NOUNLOAD,  STATS = 5
 go
 
 ALTER DATABASE CUWebinars SET MULTI_USER WITH ROLLBACK IMMEDIATE

go
USE [master]
go
ALTER DATABASE MembershipReboot SET SINGLE_USER WITH ROLLBACK IMMEDIATE
go
BACKUP LOG [MembershipRebootClean] TO  DISK = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\Backup\MembershipRebootClean_LogBackup_2014-04-10_07-22-35.bak' WITH NOFORMAT, NOINIT,  NAME = N'MembershipRebootClean_LogBackup_2014-04-10_07-22-35', NOSKIP, NOREWIND, NOUNLOAD,  NORECOVERY ,  STATS = 5
RESTORE DATABASE [MembershipReboot] FROM  DISK = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\Backup\MembershipRebootClean.bak' WITH  FILE = 2,  MOVE N'MembershipRebootClean_Data' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\MembershipReboot_Data',  MOVE N'MembershipRebootClean_Log' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\MembershipReboot_Log',  NOUNLOAD,  REPLACE,  STATS = 5

GO

 ALTER DATABASE MembershipReboot SET MULTI_USER WITH ROLLBACK IMMEDIATE







--USE cuwebinarsMigrator
--go

--    DROP PROC MigrateInstitution
--    PRINT 'droped: MigrateInstitution'
--    DROP PROC MigrateWebUser 
--    PRINT 'droped: MigrateWebUser'
--    DROP PROC MigrateAffiliate
--    PRINT 'droped: MigrateAffiliate'
--    DROP PROC MigrateRegTypes
--    PRINT 'droped: MigrateRegTypes'
--    DROP PROC [dbo].[MigrateRegTypesGroups]
--    PRINT 'droped: MigrateRegTypesGroups'
--    DROP PROC MigrateRegTypesXref 
--    PRINT 'droped: MigrateRegTypesXref'
--    DROP PROC MigratePresenter 
--    PRINT 'droped: MigratePresenter'
--    DROP PROC MigrateWebinar 
--    PRINT 'droped: MigrateWebinar'
--    DROP PROC MigrateRegTypesGroupsXref 
--    PRINT 'droped: MigrateOptionsGroupsXref'
--    DROP PROC MigrateTopic 
--    PRINT 'droped: MigrateTopic'
--    DROP PROC MigrateWebinarTopicXref 
--    PRINT 'droped: MigrateWebinarTopicXref'
--    DROP PROC MigrateWebinarFile 
--    PRINT 'droped: MigrateWebinarFile'
--    DROP PROC MigrateAddresses 
--    PRINT 'droped: MigrateAddresses'

USE [CUWebinarsMigrator]
GO
/****** Object:  StoredProcedure [dbo].[Migrate_Runner]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Migrate_Runner]
AS
BEGIN

    EXEC MigrateInstitution
    PRINT 'ends: MigrateInstitution'
    EXEC MigrateWebUser 
    PRINT 'ends: MigrateWebUser'
    EXEC MigrateAffiliate
    PRINT 'ends: MigrateAffiliate'
    EXEC MigrateRegTypes
    PRINT 'ends: MigrateRegTypes'
    EXEC [dbo].[MigrateRegTypesGroups]
    PRINT 'ends: MigrateRegTypesGroups'
    EXEC MigrateRegTypesXref 
    PRINT 'ends: MigrateRegTypesXref'
    EXEC MigratePresenter 
    PRINT 'ends: MigratePresenter'
    EXEC MigrateWebinar 
    PRINT 'ends: MigrateWebinar'
    EXEC MigrateRegTypesGroupsXref 
    PRINT 'ends: MigrateOptionsGroupsXref'
    EXEC MigrateTopic 
    PRINT 'ends: MigrateTopic'
    EXEC MigrateWebinarTopicXref 
    PRINT 'ends: MigrateWebinarTopicXref'
    EXEC MigrateWebinarFile 
    PRINT 'ends: MigrateWebinarFile'
    EXEC MigrateAddresses 
    PRINT 'ends: MigrateAddresses'
--EXEC MigrateOrders
--PRINT 'ends: MigrateOrders'
--EXEC MigrateOrderRow
--PRINT 'ends: MigrateOrderRow'
--EXEC dbo.MigrateAdditionalLocations
--PRINT 'ends: MigrateAdditionalLocations'
END


GO
/****** Object:  StoredProcedure [dbo].[MigrateAdditionalLocations]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[MigrateAdditionalLocations]
AS
BEGIN

INSERT  dbo.AdditionalLocations
        ( Email, FullName, idOrderRowOption )
VALUES  ( N'', -- Email - nvarchar(150)
          N'', -- FullName - nvarchar(100)
          0  -- idOrderRowOption - int
          )
end


GO
/****** Object:  StoredProcedure [dbo].[MigrateAddresses]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateAddresses]
AS
BEGIN

    PRINT '---------------------Starting MigrateAddresses'
	SET NOCOUNT ON 
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)
	
    SET IDENTITY_INSERT CUWebinars.dbo.[Address] on
    DECLARE my_Cursor CURSOR
    FOR
    SELECT  id
    FROM    CUWebinarsClean.dbo.Addresses
    OPEN my_Cursor

    SET @id2Insert = 1
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY

                INSERT  CUWebinars.dbo.[Address]
                        (Id, AddressType ,
                          Name ,
                          Phone ,
                          StreetAddress ,
                          StreetAddress2 ,
                          City ,
                          Zip ,
                          State ,
                          Country ,
                          idUser
                        )

                VALUES  ( @id2Insert ,
                          ( SELECT  AddressType
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  Name
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  Phone
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  StreetAddress
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  StreetAddress2
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  City
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  Zip
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  State
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  Country
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          ) ,
                          ( SELECT  idUser
                            FROM    CUWebinarsClean.dbo.Addresses
                            WHERE   id = @id2Insert
                          )
                        )

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error idAddresses: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor
    SET IDENTITY_INSERT CUWebinars.dbo.[Address] OFF

END




GO
/****** Object:  StoredProcedure [dbo].[MigrateAffiliate]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateAffiliate]
AS
BEGIN

    PRINT '---------------------Starting [MigrateAffiliate]'
	SET NOCOUNT ON 
	  DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idUserAff
    FROM    CUWebinarsClean.dbo.Affiliate

    OPEN my_Cursor

    SET @id2Insert = 1
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY
                INSERT  CUWebinars.dbo.Affiliate
                        ( idUserAff ,
                          CommissionModel ,
                          URL ,
                          WebBanner ,
                          WebFooter ,
                          EmailBanner ,
                          EmailFooter ,
                          ttsDomain ,
                          GAPass ,
                          supportEmail ,
                          DisplayTitle ,
                          BillingModel ,
                          Logo ,
                          ContactPerson ,
                          ContactPhone ,
                          ContactEmail ,
                          ContactFax ,
                          ContactAddress ,
                          TechEmail ,
                          TechPhone ,
                          TechName ,
                          EmailPromo
                        )
                VALUES  ( @id2Insert , -- idUserAff - int
                          ( SELECT  CommissionModel
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  URL
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  WebBanner
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  WebFooter
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  EmailBanner
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  EmailBanner
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  ttsDomain
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  GAPass
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  supportEmail
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  DisplayTitle
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  BillingModel
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  Logo
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  ContactPerson
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  ContactPhone
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  ContactEmail
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  ContactFax
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  ContactAddress
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  TechEmail
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  TechPhone
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  TechName
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          ) ,
                          ( SELECT  EmailPromo
                            FROM    CUWebinarsClean.dbo.Affiliate
                            WHERE   idUserAff = @id2Insert
                          )
                        )

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error AffiliateID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor


END



GO
/****** Object:  StoredProcedure [dbo].[MigrateDiscounts]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateDiscounts]
AS
BEGIN

INSERT  dbo.Discounts
        ( discountType ,
          code ,
          percentOff ,
          flatOff ,
          usesNumber ,
          dateValidFrom ,
          dateValidTo ,
          status ,
          dateBilled ,
          cost ,
          Notes
        )
VALUES  ( 0 , -- discountType - tinyint
          N'' , -- code - nvarchar(50)
          NULL , -- percentOff - decimal
          NULL , -- flatOff - decimal
          0 , -- usesNumber - int
          '2014-03-26 12:43:37' , -- dateValidFrom - datetime
          '2014-03-26 12:43:37' , -- dateValidTo - datetime
          N'' , -- status - nvarchar(50)
          '2014-03-26 12:43:37' , -- dateBilled - datetime
          NULL , -- cost - decimal
          N''  -- Notes - nvarchar(max)
        )


end


GO
/****** Object:  StoredProcedure [dbo].[MigrateInstitution]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateInstitution]
AS
BEGIN

    PRINT '---------------------Starting MigrateInstitution'
    SET NOCOUNT ON;
    SET IDENTITY_INSERT CUWebinars.[dbo].[Institution] ON 

    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idInstitution
    FROM    CUWebinarsClean.dbo.Institution

    OPEN my_Cursor

    SET @id2Insert = 1
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY
                INSERT  CUWebinars.dbo.Institution
                        ( idInstitution ,
                          InstitutionName ,
                          InstitutionType ,
                          domainName ,
                          RegIdentifier ,
                          Address ,
                          City ,
                          State ,
                          Zip
                        )
                VALUES  ( @id2Insert ,
                          ( SELECT  InstitutionName
                            FROM    CUWebinarsClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  InstitutionType
                            FROM    CUWebinarsClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  domainName
                            FROM    CUWebinarsClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  RegIdentifier
                            FROM    CUWebinarsClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  Address
                            FROM    CUWebinarsClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  City
                            FROM    CUWebinarsClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  State
                            FROM    CUWebinarsClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  Zip
                            FROM    CUWebinarsClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          )
                        )
            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error InstitutionID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor

SET IDENTITY_INSERT CUWebinars.[dbo].[Institution] off
END


GO
/****** Object:  StoredProcedure [dbo].[MigrateOrderRow]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MigrateOrderRow]
AS
BEGIN
    SET NOCOUNT ON
    PRINT '---------------------Starting MigrateOrderRow'
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idOrderRow
    FROM    CUWebinarsClean.dbo.OrderRow

    OPEN my_Cursor

    SET @id2Insert = 1
	
    SET IDENTITY_INSERT CUWebinars.dbo.OrderRow ON
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY

                INSERT  CUWebinars.dbo.OrderRow
                        ( idOrder ,
                          idWebinar ,
                          idRegType ,
                          UnitPrice ,
                          RowPrice ,
                          Royalty ,
                          ShipmentDate ,
                          AccessExpires ,
                          RowStatus ,
                          Discount_idDiscount
                        )
                VALUES  ( 0 , -- idOrder - int
                          0 , -- idWebinar - int
                          0 , -- idRegType - int
                          NULL , -- UnitPrice - decimal
                          NULL , -- RowPrice - decimal
                          NULL , -- Royalty - decimal
                          '2014-04-07 19:35:27' , -- ShipmentDate - datetime
                          '2014-04-07 19:35:27' , -- AccessExpires - datetime
                          0 , -- RowStatus - int
                          0  -- Discount_idDiscount - int
                        )
                        
            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error OrderRowsID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor



    SET IDENTITY_INSERT CUWebinars.dbo.OrderRow OFF
END

GO
/****** Object:  StoredProcedure [dbo].[MigrateOrders]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateOrders]
AS
BEGIN
    SET NOCOUNT ON
    PRINT '---------------------Starting MigrateOrders'
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idOrder
    FROM    CUWebinarsClean.dbo.Orders

    OPEN my_Cursor

    SET @id2Insert = 1
	
    SET IDENTITY_INSERT CUWebinars.dbo.Orders ON
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY

                INSERT  CUWebinars.dbo.Orders
                        (idOrder, idUser ,
                          idAffiliate ,
                          OrderDate ,
                          Total ,
                          FirstName ,
                          LastName ,
                          Institution ,
                          BillingPhone ,
                          BillingEmail ,
                          BillingAddress ,
                          BillingAddress2 ,
                          BillingCity ,
                          BillingState ,
                          BillingZip ,
                          ShippingFirstName ,
                          ShippingLastName ,
                          ShippingPhone ,
                          ShippingAddress ,
                          ShippingAddress2 ,
                          ShippingCity ,
                          ShippingState ,
                          ShippingZip ,
                          PaymentType ,
                          UserComments ,
                          AuditInfo ,
                          AffiliateComments ,
                          AdminComments ,
                          TaxExempt ,
                          InitiatedBy ,
                          PaidByCCNumber ,
                          Origin
                        )
                VALUES  (@id2Insert, ( SELECT  idUser
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  idAffiliate
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  OrderDate
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  Total
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  FirstName
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  LastName
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  Institution
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  BillingPhone
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  BillingEmail
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  BillingAddress
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  BillingAddress2
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  BillingCity
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  BillingState
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  BillingZip
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  ShippingFirstName
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  ShippingLastName
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  ShippingPhone
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  ShippingAddress
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  ShippingAddress2
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  ShippingCity
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  ShippingState
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  ShippingZip
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  PaymentType
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  UserComments
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  AuditInfo
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  AffiliateComments
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  AdminComments
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  TaxExempt
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  InitiatedBy
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  PaidByCCNumber
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          ) ,
                          ( SELECT  Origin
                            FROM    CUWebinarsClean.dbo.Orders
                            WHERE   idOrder = @id2Insert
                          )
                        )

		

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

				-- Roll back any active or uncommittable transactions before
				-- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error OrderID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor



    SET IDENTITY_INSERT CUWebinars.dbo.Orders OFF
END


GO
/****** Object:  StoredProcedure [dbo].[MigratePresenter]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigratePresenter]
AS
BEGIN
SET NOCOUNT ON 

    PRINT '---------------------Starting MigratePresenter'
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idUser
    FROM    CUWebinarsClean.dbo.Presenter

    OPEN my_Cursor

    SET @id2Insert = 1
	
    
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY
                INSERT  CUWebinars.dbo.Presenter
                        ( idUser ,
                          Biography ,
                          BiographyLong ,
                          PhotoFull ,
                          PhotoThumb
                        )
                VALUES  ( @id2Insert , -- idUser - int
                          ( SELECT  Biography
                            FROM    CUWebinarsClean.dbo.Presenter
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  BiographyLong
                            FROM    CUWebinarsClean.dbo.Presenter
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  PhotoFull
                            FROM    CUWebinarsClean.dbo.Presenter
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  PhotoThumb
                            FROM    CUWebinarsClean.dbo.Presenter
                            WHERE   idUser = @id2Insert
                          )
                        )


            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error PresenterID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor


END


GO
/****** Object:  StoredProcedure [dbo].[MigrateRegTypes]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MigrateRegTypes]
AS
BEGIN
    SET NOCOUNT ON 
    PRINT '---------------------Starting MigrateRegTypes'
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idOption
    FROM    CUWebinarsClean.dbo.Options

    OPEN my_Cursor

    SET @id2Insert = 1
	
    SET IDENTITY_INSERT CUWebinars.dbo.RegType ON
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY

                INSERT  CUWebinars.dbo.RegType
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
                VALUES  ( @id2Insert ,
                          ( SELECT  OptionExplain
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) , -- RegTypeExplain - nvarchar(max)
                          ( SELECT  OptionLabel
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) , -- RegTypeLabel - nvarchar(max)
                          ( SELECT  PriceToAdd
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) , -- Price - float
                          NULL , -- TaxExempt - bit
                          ( SELECT  SortOrder
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) ,   -- SortOrder - int
                          ( SELECT  SKU
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) , -- SKU - nvarchar(max)
                          ( SELECT  ShowLiveNotifications
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) , -- ShowLiveNotifications - nvarchar(max)
                          ( SELECT  ShowRecordingNotifications
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) ,-- ShowRecordingNotifications - nvarchar(max)
                          ( SELECT  ShowShippedNotifications
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) , -- ShowShippedNotifications - nvarchar(max)
                          ( SELECT  Stage1CheckoutConfirmationMsg
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) ,-- Stage1CheckoutConfirmationMsg - nvarchar(max)
                          ( SELECT  Stage2CheckoutConfirmationMsg
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) , -- Stage2CheckoutConfirmationMsg - nvarchar(max)
                          ( SELECT  Stage1EmailConfirmationMsg
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) , -- Stage1EmailConfirmationMsg - nvarchar(max)
                          ( SELECT  Stage2EmailConfirmationMsg
                            FROM    CUWebinarsClean.dbo.Options
                            WHERE   idOption = @id2Insert
                          ) -- Stage2EmailConfirmationMsg - nvarchar(max)
                        )

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error OptionsID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor



    SET IDENTITY_INSERT CUWebinars.dbo.RegType OFF
END


GO
/****** Object:  StoredProcedure [dbo].[MigrateRegTypesGroups]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MigrateRegTypesGroups]
AS
BEGIN
    SET NOCOUNT ON
    PRINT '---------------------Starting [MigrateRegTypesGroups]'
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idOptionGroup
    FROM    CUWebinarsClean.dbo.OptionsGroups

    OPEN my_Cursor

    SET @id2Insert = 1
	
    SET IDENTITY_INSERT CUWebinars.dbo.RegTypesGroups ON

    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY
                INSERT  CUWebinars.dbo.RegTypesGroups
                        ( idRegTypeGroup ,
                          RegTypeGroupDesc ,
                          SortOrder
                        )
                VALUES  ( @id2Insert ,
                          ( SELECT  OptionGroupDesc
                            FROM    CUWebinarsClean.dbo.OptionsGroups
                            WHERE   idOptionGroup = @id2Insert
                          ) ,
                          ( SELECT  SortOrder
                            FROM    CUWebinarsClean.dbo.OptionsGroups
                            WHERE   idOptionGroup = @id2Insert
                          )
                        )

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error OptionsGroups: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor



    SET IDENTITY_INSERT CUWebinars.dbo.RegTypesGroups OFF
END


GO
/****** Object:  StoredProcedure [dbo].[MigrateRegTypesGroupsXref]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateRegTypesGroupsXref]
AS
BEGIN
SET NOCOUNT ON
    PRINT '---------------------Starting [MigrateRegTypesGroupsXref]'
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)
	
    SET IDENTITY_INSERT CUWebinars.dbo.RegTypesGroupsXref ON
    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idWebinarOptionGroup
    FROM    CUWebinarsClean.dbo.OptionsGroupsXref
    WHERE   idWebinar IN ( SELECT   idWebinar
                           FROM     CUWebinarsClean.dbo.Webinar
                           WHERE    Status > 1 )

    OPEN my_Cursor

    SET @id2Insert = 1
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY


                INSERT  CUWebinars.dbo.RegTypesGroupsXref
                        ( idWebinarRegTypeGroup ,
                          idWebinar ,
                          idRegTypeGroup
                        )
                VALUES  ( @id2Insert ,
                          ( SELECT  idWebinar
                            FROM    CUWebinarsClean.dbo.OptionsGroupsXref
                            WHERE   idWebinarOptionGroup = @id2Insert
                          ) ,
                          ( SELECT  idOptionGroup
                            FROM    CUWebinarsClean.dbo.OptionsGroupsXref
                            WHERE   idWebinarOptionGroup = @id2Insert
                          )
                        )

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error idWebinarOptionGroup: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor
    SET IDENTITY_INSERT CUWebinars.dbo.RegTypesGroupsXref OFF

END


GO
/****** Object:  StoredProcedure [dbo].[MigrateRegTypesXref]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MigrateRegTypesXref]
AS
BEGIN

    PRINT '---------------------Starting [MigrateRegTypesXref]'
    SET NOCOUNT ON

    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idOptionsXref
    FROM    CUWebinarsClean.dbo.OptionsXref

    OPEN my_Cursor

    SET @id2Insert = 1
	
    SET IDENTITY_INSERT CUWebinars.dbo.RegTypesXref ON
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY

                INSERT  CUWebinars.dbo.RegTypesXref
                        ( idRegTypesXref ,
                          idRegTypeGroup ,
                          idRegType
                        )
                VALUES  ( @id2Insert ,
                          ( SELECT  idOptionGroup
                            FROM    CUWebinarsClean.dbo.OptionsXref
                            WHERE   idOptionsXref = @id2Insert
                          ) ,
                          ( SELECT  idOption
                            FROM    CUWebinarsClean.dbo.OptionsXref
                            WHERE   idOptionsXref = @id2Insert
                          )
                        )
            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error OptionsXRefID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor



    SET IDENTITY_INSERT CUWebinars.dbo.RegTypesXref OFF

END

GO
/****** Object:  StoredProcedure [dbo].[MigrateTopic]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateTopic]
AS
BEGIN

    PRINT '---------------------Starting MigrateOrders'
	SET NOCOUNT ON 

    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)
	
    SET IDENTITY_INSERT CUWebinars.dbo.Topic ON 
    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idTopic
    FROM    CUWebinarsClean.dbo.Topic

    OPEN my_Cursor

    SET @id2Insert = 1
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY

                INSERT  CUWebinars.dbo.Topic
                        ( idTopic ,
                          topicDesc ,
                          idParentTopic ,
                          topicHTML ,
                          sortOrder
                        )
                VALUES  ( @id2Insert ,
                          ( SELECT  topicDesc
                            FROM    CUWebinarsClean.dbo.Topic
                            WHERE   idTopic = @id2Insert
                          ) ,
                          ( SELECT  idParentTopic
                            FROM    CUWebinarsClean.dbo.Topic
                            WHERE   idTopic = @id2Insert
                          ) ,
                          ( SELECT  topicHTML
                            FROM    CUWebinarsClean.dbo.Topic
                            WHERE   idTopic = @id2Insert
                          ) ,
                          ( SELECT  sortOrder
                            FROM    CUWebinarsClean.dbo.Topic
                            WHERE   idTopic = @id2Insert
                          )
                        )
            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error idTopic: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor
    SET IDENTITY_INSERT CUWebinars.dbo.Topic OFF


END


GO
/****** Object:  StoredProcedure [dbo].[MigrateWebinar]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MigrateWebinar]
AS
BEGIN
    SET NOCOUNT ON 

    PRINT '---------------------Starting MigrateWebinar'
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idWebinar
    FROM    CUWebinarsClean.dbo.Webinar
    WHERE   idWebinar IN ( SELECT   idWebinar
                           FROM     CUWebinarsClean.dbo.Webinar
                           WHERE    Status > 1 )

    OPEN my_Cursor

    SET @id2Insert = 1
	
    SET IDENTITY_INSERT CUWebinars.dbo.Webinar ON
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY
                INSERT  CUWebinars.dbo.Webinar
                        ( idWebinar ,
                          Description ,
                          DescriptionLong ,
                          ImageUrl ,
                          SmallImageUrl ,
                          Status ,
                          Title ,
                          Date ,
                          LearnCaption ,
                          LearnBody ,
                          WhoAttend ,
                          Duration ,
                          RecordingUrl ,
                          idPresenter ,
                          ceu ,
                          ConnectionInfo ,
                          DateCreated ,
                          DateChanged ,
                          --idWebinarRegTypeGroup ,
                          WebinarKey ,
                          OrganizerKey ,
                          OrganizerOAuthKey
                        )
                VALUES  ( @id2Insert ,
                          ( SELECT  Description
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- Description - nvarchar(max)
                          ( SELECT  DescriptionLong
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- DescriptionLong - nvarchar(max)
                          ( SELECT  ImageUrl
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) ,-- ImageUrl - nvarchar(50)
                          ( SELECT  ImageUrl
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) ,-- SmallImageUrl - nvarchar(50)
                          ( SELECT  Status
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) ,-- Status - int
                          ( SELECT  Title
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) ,-- Title - nvarchar(125)
                          ( SELECT  Date
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- Date - datetime
                          ( SELECT  LearnCaption
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) ,-- LearnCaption - nvarchar(255)
                          ( SELECT  LearnBody
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- LearnBody - nvarchar(max)
                          ( SELECT  WhoAttend
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- WhoAttend - nvarchar(max)
                          ( SELECT  Duration
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- Duration - decimal
                          ( SELECT  RecordingUrl
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- RecordingUrl - nvarchar(300)
                          ( SELECT  idPresenter
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) ,-- idPresenter - int
                          ( SELECT  ceu
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) ,-- ceu - nvarchar(1000)
                          ( SELECT  ConnectionInfo
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- ConnectionInfo - nvarchar(max)
                          ( SELECT  DateCreated
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- DateCreated - datetime
                          ( SELECT  DateChanged
                            FROM    CUWebinarsClean.dbo.Webinar
                            WHERE   idWebinar = @id2Insert
                          ) , -- DateChanged - datetime
                          --0 , -- idWebinarRegTypeGroup - int
                          0 , -- WebinarKey - int
                          0 , -- OrganizerKey - int
                          N''  -- OrganizerOAuthKey - nvarchar(max)
                        )
            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error WebinarID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor



    SET IDENTITY_INSERT CUWebinars.dbo.Webinar OFF
END


GO
/****** Object:  StoredProcedure [dbo].[MigrateWebinarFile]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MigrateWebinarFile]
AS
BEGIN
    SET NOCOUNT ON 
    PRINT '---------------------Starting [MigrateWebinarFile]'
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idWebinarFile
    FROM    CUWebinarsClean.dbo.WebinarFile

    OPEN my_Cursor

    SET @id2Insert = 1
	
    SET IDENTITY_INSERT CUWebinars.dbo.WebinarFile ON
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY
                INSERT  CUWebinars.dbo.WebinarFile
                        ( idWebinarFile ,
                          idWebinar ,
                          fileLocation ,
                          fileDesc
                        )
                VALUES  ( @id2Insert ,
                          ( SELECT  idWebinar
                            FROM    CUWebinarsClean.dbo.WebinarFile
                            WHERE   idWebinarFile = @id2Insert
                          ) ,
                          ( SELECT  fileLocation
                            FROM    CUWebinarsClean.dbo.WebinarFile
                            WHERE   idWebinarFile = @id2Insert
                          ) ,
                          ( SELECT  fileDesc
                            FROM    CUWebinarsClean.dbo.WebinarFile
                            WHERE   idWebinarFile = @id2Insert
                          )
                        )

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error WebinarFileID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor



    SET IDENTITY_INSERT CUWebinars.dbo.WebinarFile OFF
END


GO
/****** Object:  StoredProcedure [dbo].[MigrateWebinarTopicXref]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateWebinarTopicXref]
AS
BEGIN

    PRINT '---------------------Starting [MigrateWebinarTopicXref]'
	SET NOCOUNT ON 
    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)
	
    SET IDENTITY_INSERT CUWebinars.dbo.WebinarTopicXref ON 
    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idWebinarTopicXref
    FROM    CUWebinarsClean.dbo.WebinarTopicXref
    WHERE   idWebinar IN ( SELECT   idWebinar
                           FROM     CUWebinarsClean.dbo.Webinar
                           WHERE    Status > 1 )

    OPEN my_Cursor

    SET @id2Insert = 1
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY

                INSERT  CUWebinars.dbo.WebinarTopicXref
                        ( idWebinarTopicXref ,
                          idWebinar ,
                          idTopic
                        )
                VALUES  ( @id2Insert ,
                          ( SELECT  idWebinar
                            FROM    CUWebinarsClean.dbo.WebinarTopicXref
                            WHERE   idWebinarTopicXref = @id2Insert
                          ) ,
                          ( SELECT  idTopic
                            FROM    CUWebinarsClean.dbo.WebinarTopicXref
                            WHERE   idWebinarTopicXref = @id2Insert
                          )
                        )

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error WebinarTopicXref: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor
    SET IDENTITY_INSERT CUWebinars.dbo.WebinarTopicXref OFF



END


GO
/****** Object:  StoredProcedure [dbo].[MigrateWebUser]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MigrateWebUser]
AS
BEGIN
    SET nocount ON;
	
    PRINT '---------------------Starting MigrateWebUser'
    SET NOCOUNT ON;

    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idUser
    FROM    CUWebinarsClean.dbo.WebUser

    OPEN my_Cursor

    SET @id2Insert = 1
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY
                INSERT  CUWebinars.dbo.WebUser
                        ( idUser ,
                          UserType ,
                          AcctStatus ,
                          DateCreated ,
                          FirstName ,
                          LastName ,
                          Initial ,
                          idUserInstitution ,
                          email ,
                          futureMail ,
                          generalComments ,
                          taxExempt ,
                          idSubscriptionDiscount ,
                          timeZone ,
                          Title
                        )
                VALUES  ( @id2Insert , -- idUser - int
                          ( SELECT  UserType
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  AcctStatus
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  DateCreated
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  FirstName
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  LastName
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  Initial
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  idUserInstitution
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  email
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  futureMail
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  generalComments
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  taxExempt
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  idSubscriptionDiscount
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  timeZone
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          ) ,
                          ( SELECT  Title
                            FROM    CUWebinarsClean.dbo.WebUser
                            WHERE   idUser = @id2Insert
                          )
                        )

            END TRY
            BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
                IF XACT_STATE() <> 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END

                EXECUTE dbo.uspLogError @Msg=@id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

                PRINT '--==========--'
                PRINT 'Error WebUserID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor


END


GO
/****** Object:  StoredProcedure [dbo].[uspLogError]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- uspLogError logs error information in the ErrorLog table about the 
-- error that caused execution to jump to the CATCH block of a 
-- TRY...CATCH construct. This should be executed from within the scope 
-- of a CATCH block otherwise it will return without inserting error 
-- information. 
CREATE Procedure [dbo].[uspLogError]
    @Msg VARCHAR(MAX) = NULL ,
    @ErrorLogID [int] = 0 OUTPUT -- contains the ErrorLogID of the row inserted
AS -- by uspLogError in the ErrorLog table
BEGIN
    SET NOCOUNT ON;

    -- Output parameter value of 0 indicates that error 
    -- information was not logged
    SET @ErrorLogID = 0;

    BEGIN TRY
        -- Return if there is no error information to log
        IF ERROR_NUMBER() IS NULL
            RETURN;
			
        -- Return if inside an uncommittable transaction.
        -- Data insertion/modification is not allowed when 
        -- a transaction is in an uncommittable state.
        IF XACT_STATE() = -1
            BEGIN
                PRINT 'Cannot log error since the current transaction is in an uncommittable state. '
                    + 'Rollback the transaction before executing uspLogError in order to successfully log error information.';
                RETURN;
            END

        SET @Msg = @Msg + ' : ' + ERROR_MESSAGE()
        INSERT  [dbo].[ErrorLog]
                ( [UserName] ,
                  [ErrorNumber] ,
                  [ErrorSeverity] ,
                  [ErrorState] ,
                  [ErrorProcedure] ,
                  [ErrorLine] ,
                  [ErrorMessage] ,
                  ErrorTime
                )
        VALUES  ( CONVERT(SYSNAME, CURRENT_USER) ,
                  ERROR_NUMBER() ,
                  ERROR_SEVERITY() ,
                  ERROR_STATE() ,
                  ERROR_PROCEDURE() ,
                  ERROR_LINE() ,
                  @Msg ,
                  GETDATE()
                );

        -- Pass back the ErrorLogID of the row inserted
        SET @ErrorLogID = @@IDENTITY;
    END TRY
    BEGIN CATCH
        PRINT 'An error occurred in stored procedure uspLogError: ';
        EXECUTE [dbo].[uspPrintError];
        RETURN -1;
    END CATCH
END;


GO
/****** Object:  StoredProcedure [dbo].[uspPrintError]    Script Date: 4/7/2014 4:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- uspPrintError prints error information about the error that caused 
-- execution to jump to the CATCH block of a TRY...CATCH construct. 
-- Should be executed from within the scope of a CATCH block otherwise 
-- it will return without printing any error information.
CREATE Procedure [dbo].[uspPrintError]
AS 
    BEGIN
        SET NOCOUNT ON;

    -- Print error information. 
        PRINT 'Error ' + CONVERT(VARCHAR(50), ERROR_NUMBER()) + ', Severity '
            + CONVERT(VARCHAR(5), ERROR_SEVERITY()) + ', State '
            + CONVERT(VARCHAR(5), ERROR_STATE()) + ', Procedure '
            + ISNULL(ERROR_PROCEDURE(), '-') + ', Line '
            + CONVERT(VARCHAR(5), ERROR_LINE());
        PRINT ERROR_MESSAGE();
    END;


GO
