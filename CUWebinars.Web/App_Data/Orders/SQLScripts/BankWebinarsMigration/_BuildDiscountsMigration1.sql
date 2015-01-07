
PRINT '--==========--'
PRINT 'BEGINS DISCOUNT HANDLING'
PRINT '--==========--'


SET NOCOUNT ON
DROP TABLE Migrator.dbo.discounts
SELECT  *
INTO    Migrator.dbo.Discounts
FROM    TTSWebinars2.dbo.Discounts 


DECLARE @discountType TINYINT ,
    @code VARCHAR(50) ,
    @percentOff DECIMAL(5, 2) ,
    @flatOff MONEY ,
    @usesNumber INT ,
    @dateValidFrom SMALLDATETIME ,
    @dateValidTo SMALLDATETIME ,
    @status NVARCHAR(50) ,
    @dateBilled SMALLDATETIME ,
    @cost MONEY ,
    @Notes NVARCHAR(MAX) ,
    @importCount INT ,
    @idDiscount INT
  
SET @importCount = 0  
PRINT '---===================---'
PRINT 'BEGINNING DISCOUNT CODE IMPORT'
PRINT '---===================---'
DECLARE migrate_cursor CURSOR
FOR
    SELECT --
            idDiscounts ,--@idDiscount
            discountType , -- @discountType
            code ,-- @code
            percentOff ,-- @percentOff
            flatOff ,
            usesNumber ,
            dateValidFrom ,
            dateValidTo ,
            status ,
            dateBilled ,
            cost ,
            notes
    FROM    TTSWebinars2.[dbo].Discounts

OPEN migrate_cursor


FETCH NEXT FROM migrate_cursor
INTO @idDiscount, @discountType, @code, @percentOff, @flatOff, @usesNumber,
    @dateValidFrom, @dateValidTo, @status, @dateBilled, @cost, @Notes 

WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @importCount = @importCount + 1

        --PRINT '---------> importing: ' + CAST (@importCount AS VARCHAR)
        SET IDENTITY_INSERT BankWebinars.dbo.Discount ON
        INSERT  BankWebinars.dbo.Discount
                ( idDiscount ,
                  DiscountType ,
                  DiscountCode ,
                  PercentOff ,
                  FlatOff ,
                  UsesCount ,
                  UsesRemain ,
                  DateValidFrom ,
                  DateValidTo ,
                  Status ,
                  DateBilled ,
                  Cost ,
                  Notes ,
                  renewalTerm
                )
        VALUES  ( @idDiscount ,
                  @discountType , -- DiscountType - int
                  @code , -- DiscountCode - nvarchar(50)
                  @percentOff , -- PercentOff - decimal
                  @flatOff , -- FlatOff - decimal
                  @usesNumber , -- UsesCount - int
                  @usesNumber , -- UsesRemain - int
                  @dateValidFrom , -- DateValidFrom - datetime
                  @dateValidTo , -- DateValidTo - datetime
                  @status , -- Status - nvarchar(50)
                  @dateBilled , -- DateBilled - datetime
                  @cost , -- Cost - decimal
                  @Notes , -- Notes - nvarchar(max)
                  @usesNumber  -- renewalTerm - int
                )
        SET IDENTITY_INSERT BankWebinars.dbo.Discount OFF

        FETCH NEXT FROM migrate_cursor
   INTO @idDiscount, @discountType, @code, @percentOff, @flatOff, @usesNumber,
            @dateValidFrom, @dateValidTo, @status, @dateBilled, @cost, @Notes 
    END

CLOSE migrate_cursor
DEALLOCATE migrate_cursor

--PRINT 'removes non-packages w/expired date'
--SELECT  'exec [dbo].[DeleteDiscount]	@discID =  ' ,
--        idDiscounts
--FROM    Discounts d
--        INNER JOIN Users u ON u.idSubscriptionDiscount = d.idDiscounts
--        INNER JOIN dbo.Orders o ON o.idUser = u.idUser
--WHERE   discountType <> 4
--        AND dateValidTo < GETDATE()
---- add special handling for current spreadsheet
----
--        AND o.idOrder NOT IN ( SELECT   OrderID
--                               FROM     Migrator.dbo.['Current Clients$'] )
----

--UPDATE  Discounts
--SET     dateBilled = dateValidFrom
--WHERE   dateBilled IS NULL
--PRINT 'removes packages w/no remaining uses'
--SELECT  'exec [dbo].[DeleteDiscount]	@discID =  ' ,
--        idDiscounts
--FROM    Discounts
--WHERE   discountType = 4
--        AND usesNumber = 0

----GO

SET NOCOUNT ON 

PRINT '--==========--'
PRINT 'ENDS DISCOUNT HANDLING'
PRINT '--==========--'

USE master
GO
