USE BankWebinars
GO

/****** Object:  StoredProcedure [dbo].[GetRegistrationType]    Script Date: 12/10/2013 11:31:31 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		SJH
-- Create date: 12/13
-- Description:	Produce list of Options based on idWebinar
-- =============================================
CREATE PROCEDURE [dbo].[GetRegistrationType]
	-- Add the parameters for the stored procedure here
    @idRegType INT = 0
AS
BEGIN
    SET NOCOUNT ON-- added to prevent extra result sets from
	-- interfering with SELECT statements.
    SELECT  o.[idRegType] ,
            o.[OptionExplain] ,
            o.[OptionLabel] ,
            o.[PriceToAdd] ,
            o.[TaxExempt] ,
            o.[PercToAdd] ,
            o.[SortOrder] ,
            o.[Type] ,
            o.[MsgConfirm] ,
            o.[SKU] ,
            o.[ShowLiveNotifications] ,
            o.[ShowRecordingNotifications] ,
            o.[ShowShippedNotifications] ,
            o.[Stage1CheckoutConfirmationMsg] ,
            o.[Stage2CheckoutConfirmationMsg] ,
            o.[Stage1EmailConfirmationMsg] ,
            o.[Stage2EmailConfirmationMsg]
    FROM    dbo.Options o
    WHERE   o.idRegType = @idRegType
END
GO


/****** Object:  Table [dbo].[ErrorLog]    Script Date: 12/10/2013 11:29:09 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ErrorLog]
(
  [ErrorLogID] [int] IDENTITY(1, 1)
                     NOT NULL ,
  [ErrorTime] [datetime] NOT NULL ,
  [UserName] [sysname] NOT NULL ,
  [ErrorNumber] [int] NOT NULL ,
  [ErrorSeverity] [int] NULL ,
  [ErrorState] [int] NULL ,
  [ErrorProcedure] [nvarchar](126) NULL ,
  [ErrorLine] [int] NULL ,
  [ErrorMessage] [nvarchar](4000) NOT NULL ,
  CONSTRAINT [PK_ErrorLog_ErrorLogID] PRIMARY KEY CLUSTERED ( [ErrorLogID] ASC )
    WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON )
)

GO

ALTER TABLE [dbo].[ErrorLog] ADD  CONSTRAINT [DF_ErrorLog_ErrorTime]  DEFAULT (GETDATE()) FOR [ErrorTime]
GO
/****** Object:  StoredProcedure [dbo].[GetWebinarsOptions]    Script Date: 12/4/2013 2:32:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

USE BankWebinars
GO


CREATE PROCEDURE [dbo].[CreateCUOrder]
    @idUser INT , -- UserID
    @idWebinar INT ,
    @idRegType INT ,
    @idAffiliate INT = 19 ,
    @idDiscount INT = NULL ,
    @billStatus INT = 3
AS
BEGIN
    SET NOCOUNT ON

    INSERT  ErrorLog
            ( ErrorTime ,
              UserName ,
              ErrorNumber ,
              ErrorSeverity ,
              ErrorState ,
              ErrorProcedure ,
              ErrorLine ,
              ErrorMessage
	        )
    VALUES  ( GETDATE() , -- ErrorTime - datetime
              CONVERT(SYSNAME, CURRENT_USER) ,--'tracing' , -- UserName - sysname
              0 , -- ErrorNumber - int
              0 , -- ErrorSeverity - int
              0 , -- ErrorState - int
              N'[InsertOrder] logging the call' , -- ErrorProcedure - nvarchar(126)
              0 , -- ErrorLine - int
              CAST(@idUser AS VARCHAR) + ', ' + CAST(@idWebinar AS VARCHAR) + ', ' + CAST(@idRegType AS VARCHAR)
	        )
    DECLARE @ErrorLogID INT
    DECLARE @percentOff DECIMAL
    SET @percentOff = ( SELECT  percentOff
                        FROM    Discounts
                        WHERE   idDiscount = @idDiscount
                      )

    DECLARE @FlatOff DECIMAL

    SET @FlatOff = ( SELECT flatOff
                     FROM   Discounts
                     WHERE  idDiscount = @idDiscount
                   )
    BEGIN TRY
        INSERT  [Orders]
                ( idUser ,
                  orderDate ,
                  Total ,
                  FirstName ,
                  lastName ,
                  customerInstitution ,
                  phone ,
                  Email ,
                  [address] , -- 9 deep
                      --[address2] ,
                  city ,
                  [state] ,
                  zip ,
                  --shippingFirstName ,
                  shippingLastName ,
                  shippingAddress ,
                      --shippingAddress2 ,
                  shippingCity ,
                  shippingState ,
                  shippingZip ,
                  auditInfo ,
                  initiatedBy ,
                  --shippingPhone ,
                  idAffiliate ,
                  paymentType ,
                  taxExempt ,
                  --Email ,
                  Origin
                )
        VALUES  ( @idUser ,
                  GETDATE() ,  --orderDate                  
                  ( SELECT  PriceToAdd
                    FROM    dbo.Options
                    WHERE   idRegType = @idRegType
                  ) ,
                  ( SELECT  firstName
                    FROM    WebUser
                    WHERE   idUser = @idUser
                  ) ,
                  ( SELECT  lastName
                    FROM    WebUser
                    WHERE   idUser = @idUser
                  ) ,
                  ( SELECT  InstitutionName
                    FROM    dbo.Institution
                    WHERE   idInstitution = ( SELECT    idUserInstitution
                                              FROM      dbo.WebUser
                                              WHERE     idUser = @idUser
                                            )
                  ) ,
                  ( SELECT  Phone
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Billing'
                  ) ,
                  ( SELECT  email
                    FROM    WebUser
                    WHERE   idUser = @idUser
                  )--email
                  ,
                  ( SELECT  [StreetAddress]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Billing'
                  ) ,--address  9 deep
                      --( SELECT  [StreetAddress2]
                      --  FROM    dbo.[Addresses]
                      --  WHERE   [WebUser_idUser]= @idUser
                      --          AND AddressType = 'Billing'
                      --) ,--address2
                  ( SELECT  [City]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Billing'
                  ) --city
                  ,
                  ( SELECT  [State]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Billing'
                  ) ,--state
                  ( SELECT  [Zip]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Billing'
                  )   --zip
                  ,
                  ( SELECT  [Name]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Shipping'
                  ) ,--address
                  ( SELECT  [StreetAddress]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Shipping'
                  ) ,--address
                      --( SELECT  [StreetAddress2]
                      --  FROM    dbo.[Addresses]
                      --  WHERE   [WebUser_idUser]= @idUser
                      --          AND AddressType = 'Shipping'
                      --) ,--address2
                  ( SELECT  [City]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Shipping'
                  ) --city
                  ,
                  ( SELECT  [State]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Shipping'
                  ) ,--state
                  ( SELECT  [Zip]
                    FROM    dbo.[Addresses]
                    WHERE   [WebUser_idUser] = @idUser
                            AND AddressType = 'Shipping'
                  ) ,   --zip
                  'sproc insertion' , --auditInfo
                  4 , --initiatedBy                      
                  19 ,--@AffiliateId ,
                  '5' , -- paymentType
                  0 ,  --taxExempt
                  --'' ,
                  8
                )
  
        DECLARE @newOrderID INT
        SET @newOrderID = SCOPE_IDENTITY()
		
            
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
    END CATCH; 

    BEGIN TRY
        INSERT  OrderRow
                ( idOrder ,
                  idWebinar ,
                  unitPrice ,
                  rowPrice ,
                  idDiscount ,
                  discountPercentOff ,
                  discountFlatOff ,
                  alternateEmail ,
                  registrationType ,
                  status ,
                  Royalty ,
                  isAdHocRecording
                )
        VALUES  ( @newOrderID ,
                  @idWebinar ,
                  ( SELECT  priceToAdd
                    FROM    Options
                    WHERE   idRegType = @idRegType
                  ) ,
                  ( SELECT  priceToAdd
                    FROM    Options
                    WHERE   idRegType = @idRegType
                  ) ,
                  @idDiscount ,
                  ISNULL(@percentOff, 0) ,
                  ISNULL(@flatOff, 0) ,
                  ' ' ,
                  @idRegType ,
                  2 ,
                  0 ,
                  ''
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
    END CATCH; 
END
BEGIN
    SELECT  @newOrderID AS orderId
END
go

USE [TTSWebinars]
GO

/****** Object:  StoredProcedure [dbo].[GetWebinarsOptions]    Script Date: 12/4/2013 2:32:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		SJH
-- Create date: 12/13
-- Description:	Produce list of Options based on idWebinar
-- =============================================
CREATE PROCEDURE [dbo].[GetWebinarsOptions]
	-- Add the parameters for the stored procedure here
    @idWebinar INT = 0
AS
BEGIN
    SET NOCOUNT ON-- added to prevent extra result sets from
	-- interfering with SELECT statements.
    SET NOCOUNT ON;
    SELECT  o.[idRegType] ,
            o.[OptionExplain] ,
            o.[OptionLabel] ,
            o.[PriceToAdd] ,
            o.[TaxExempt] ,
            o.[PercToAdd] ,
            o.[SortOrder] ,
            o.[Type] ,
            o.[MsgConfirm] ,
            o.[SKU] ,
            o.[ShowLiveNotifications] ,
            o.[ShowRecordingNotifications] ,
            o.[ShowShippedNotifications] ,
            o.[Stage1CheckoutConfirmationMsg] ,
            o.[Stage2CheckoutConfirmationMsg] ,
            o.[Stage1EmailConfirmationMsg] ,
            o.[Stage2EmailConfirmationMsg]

        --oRef.idRegTypesXref ,
        --oRef.idRegTypeGroup ,
        --ogRef.idWebinarOptionGroup
    FROM    dbo.Options o
            INNER JOIN dbo.OptionsXref oRef ON oRef.idRegType = o.idRegType
            INNER JOIN dbo.OptionsGroupsXref ogRef ON ogRef.idRegTypeGroup = oRef.idRegTypeGroup
            INNER JOIN dbo.Webinar w ON w.idWebinar = ogRef.idWebinar
    WHERE   w.idWebinar = @idWebinar
END

GO

USE [TTSWebinars]
GO

/****** Object:  StoredProcedure [dbo].[uspLogError]    Script Date: 12/4/2013 2:32:39 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- uspLogError logs error information in the ErrorLog table about the 
-- error that caused execution to jump to the CATCH block of a 
-- TRY...CATCH construct. This should be executed from within the scope 
-- of a CATCH block otherwise it will return without inserting error 
-- information. 
CREATE PROCEDURE [dbo].[uspLogError]
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

        INSERT  [dbo].[ErrorLog]
                ( [UserName] ,
                  [ErrorNumber] ,
                  [ErrorSeverity] ,
                  [ErrorState] ,
                  [ErrorProcedure] ,
                  [ErrorLine] ,
                  [ErrorMessage]
                )
        VALUES  ( CONVERT(SYSNAME, CURRENT_USER) ,
                  ERROR_NUMBER() ,
                  ERROR_SEVERITY() ,
                  ERROR_STATE() ,
                  ERROR_PROCEDURE() ,
                  ERROR_LINE() ,
                  ERROR_MESSAGE()
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

USE [TTSWebinars]
GO

/****** Object:  StoredProcedure [dbo].[uspPrintError]    Script Date: 12/4/2013 2:32:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- uspPrintError prints error information about the error that caused 
-- execution to jump to the CATCH block of a TRY...CATCH construct. 
-- Should be executed from within the scope of a CATCH block otherwise 
-- it will return without printing any error information.
CREATE PROCEDURE [dbo].[uspPrintError]
AS
BEGIN
    SET NOCOUNT ON;

    -- Print error information. 
    PRINT 'Error ' + CONVERT(VARCHAR(50), ERROR_NUMBER()) + ', Severity ' + CONVERT(VARCHAR(5), ERROR_SEVERITY()) + ', State '
        + CONVERT(VARCHAR(5), ERROR_STATE()) + ', Procedure ' + ISNULL(ERROR_PROCEDURE(), '-') + ', Line ' + CONVERT(VARCHAR(5), ERROR_LINE());
    PRINT ERROR_MESSAGE();
END;


GO
USE [TTSWebinars]
GO

/****** Object:  Table [dbo].[ErrorLog]    Script Date: 12/4/2013 2:33:12 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
