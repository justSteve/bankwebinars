
DELETE  BankWebinars.dbo.Discount
WHERE   idDiscount IN ( SELECT  idDiscount
                        FROM    BankWebinars.dbo.Discount
                        WHERE   DiscountCode NOT LIKE 'cp%' )
PRINT '--==========--'
PRINT 'BEGINS DISCOUNT HANDLING'
PRINT '--==========--'

SET NOCOUNT on

DECLARE @discountType INT ,
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
    SELECT DISTINCT --
            [Promo Code]
    FROM    Migrator.dbo.SubscriptionsOut
    WHERE   Package IN ( 'Bronze', 'Gold', 'Platinum', 'Platinum/Silver', 'Silver')
	AND [Remaining Credits] > 0

OPEN migrate_cursor


FETCH NEXT FROM migrate_cursor
INTO @code

WHILE @@FETCH_STATUS = 0
    BEGIN
	PRINT @code
SET @idDiscount = (SELECT idDiscounts FROM TTSWebinars2.dbo.Discounts WHERE code = @code )
 set  @discountType = 4
 set  @code = (select  [Promo Code] from Migrator.dbo.SubscriptionsOut where [Promo Code] = (select code from TTSWebinars2.dbo.discounts where idDiscounts = @idDiscount))
 set  @percentOff = 100
 set  @flatOff = 0
 set  @usesNumber = (select  Credits from Migrator.dbo.SubscriptionsOut where [Promo Code] = (select code from TTSWebinars2.dbo.discounts where idDiscounts = @idDiscount))
 set  @dateValidFrom = DATEADD(YEAR, -1, GETDATE()) 
 set  @dateValidTo = DATEADD(YEAR, 1, GETDATE()) 
 set  @status = 'Active'
 set 
    @dateBilled = DATEADD(YEAR, -1, GETDATE()) 
 set  @cost = 0
 set  @Notes = (SELECT FirstName + ' ' + LastName + ' ' + Company FROM Migrator.dbo.SubscriptionsOut WHERE [Promo Code] = @code)


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
                  4 , -- DiscountType - int
                  @code , -- DiscountCode - nvarchar(50)
                  @percentOff , -- PercentOff - decimal
                  @flatOff , -- FlatOff - decimal
                  @usesNumber , -- UsesCount - int
                  (SELECT [Remaining Credits] FROM Migrator.dbo.SubscriptionsOut WHERE [Promo Code] = @code) , -- UsesRemain - int
                  @dateValidFrom , -- DateValidFrom - datetime
                  @dateValidTo , -- DateValidTo - datetime
                  @status , -- Status - nvarchar(50)
                  @dateBilled , -- DateBilled - datetime
                  @cost , -- Cost - decimal
                  @Notes , -- Notes - nvarchar(max)
                  0  -- renewalTerm - int
                )
        SET IDENTITY_INSERT BankWebinars.dbo.Discount OFF

        FETCH NEXT FROM migrate_cursor
   INTO @code

    END

CLOSE migrate_cursor
DEALLOCATE migrate_cursor


PRINT '--==========--'
PRINT 'ENDS DISCOUNT HANDLING'
PRINT '--==========--'
SET NOCOUNT off
SELECT * FROM BankWebinars.dbo.Discount  WHERE DiscountCode NOT LIKE 'cp%'