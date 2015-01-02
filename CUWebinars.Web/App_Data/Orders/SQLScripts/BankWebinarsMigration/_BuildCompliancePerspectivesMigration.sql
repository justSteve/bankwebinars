
PRINT '--==========--'
PRINT '----UPDATE COMPLIANCE PERSPECTIVES SUBSCRIPTIONS BY IMPORTING SHEET THEN RUNNING'
PRINT '----GENERATED DELETE CODES'

PRINT '--==========--'

USE [TTSWebinars2]
GO

UPDATE  Discounts
SET     discountType = 4
WHERE   discountType = 3
UPDATE  Discounts
SET     discountType = 3
WHERE   code IN ( 'FIB0113', 'BBCSB0313', 'GRCB0313', 'CCB0413' )



SET NOCOUNT ON
--DELETE  FROM [TTSWebinars2].dbo.Discounts
--WHERE   code LIKE 'CP_%'
--        AND code NOT LIKE 'CP2010'
USE TTSWebinars2
GO

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
    @importCount INT,
	@idDiscount int
  
SET @importCount = 0  
PRINT '---===================---'
PRINT 'BEGINNING COMPLIANCE SUBSCRIPTIONS IMPORT'
PRINT '---===================---'
DECLARE migrate_cursor CURSOR
FOR
    SELECT --
            '5' ,
            'CP_' + CAST([OrderID] AS VARCHAR) ,
            100 ,
            0.0 ,
            [Renewal Term] ,
            DATEADD(mm, -[Renewal Term], [Subscription Expires]) , -- good from
            [Subscription Expires] , -- Expires 
            'Active' ,
            DATEADD(mm, -[Renewal Term], [Subscription Expires])  ,
            ( SELECT    total
              FROM      [TTSWebinars2].dbo.Orders o
              WHERE     o.idOrder = OrderID
            ) ,
            'Institution: '
            + ( SELECT  name
                FROM    [TTSWebinars2].dbo.Institution
                WHERE   idInstitution = ( SELECT    idUserInstitution
                                          FROM      [TTSWebinars2].dbo.Users
                                          WHERE     idUser = ( SELECT
                                                              idUser
                                                              FROM
                                                              [TTSWebinars2].dbo.Orders
                                                              WHERE
                                                              idOrder = [OrderID]
                                                             )
                                        )
              )
    FROM    [Migrator].[dbo].['Current Clients$']
    WHERE   [Subscription Expires] IS NOT NULL
OPEN migrate_cursor


FETCH NEXT FROM migrate_cursor
INTO @discountType, @code, @percentOff, @flatOff, @usesNumber, @dateValidFrom,
    @dateValidTo, @status, @dateBilled, @cost, @Notes 

WHILE @@FETCH_STATUS = 0
    BEGIN
SET @importCount = @importCount + 1

PRINT '---------> importing: ' + CAST (@importCount AS VARCHAR)
        INSERT  INTO BankWebinars.dbo.Discount
                ( discountType ,
                  DiscountCode ,
                  percentOff ,
                  flatOff ,
                  UsesCount ,
                  dateValidFrom ,
                  dateValidTo ,
                  [status] ,
                  dateBilled ,
                  cost ,
                  notes, renewalTerm, UsesRemain
                )
        VALUES  ( @discountType ,
                  @code ,
                  @percentOff ,
                  @flatOff ,
                  @usesNumber ,
                  @dateValidFrom ,
                  @dateValidTo ,
                  @status ,
                  @dateBilled ,
                  @cost ,
                  @Notes, @usesNumber, 0
                )

        FETCH NEXT FROM migrate_cursor
   INTO @discountType, @code, @percentOff, @flatOff, @usesNumber,
            @dateValidFrom, @dateValidTo, @status, @dateBilled, @cost, @Notes 
    END

CLOSE migrate_cursor
DEALLOCATE migrate_cursor
-- select * from Discounts where idDiscounts = 105
-- select * from OrdersRows where idDiscount = 105
----select * from OrdersRows r inner join Discounts d on r.idDiscount = d.idDiscounts
SET NOCOUNT OFF

----where d.code = 'GARNCOMP_USER1'
--  select * from [TTSWebinars2].dbo.Discounts where code like 'CP_%'
PRINT 'excute statements that populate CompPers Subscriptions'
PRINT 'following statements generated via C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\PopulateCPSubscriptions.sql'
SET NOCOUNT ON 
SET NOCOUNT ON
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_19983') where idOrder = 19983
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21253') where idOrder = 21253
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21254') where idOrder = 21254
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21268') where idOrder = 21268
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21274') where idOrder = 21274
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21278') where idOrder = 21278
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21282') where idOrder = 21282
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_22572') where idOrder = 22572
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_22648') where idOrder = 22648
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_22890') where idOrder = 22890
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_23162') where idOrder = 23162
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_23223') where idOrder = 23223
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_23242') where idOrder = 23242
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_32174') where idOrder = 32174
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_32939') where idOrder = 32939
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_33088') where idOrder = 33088
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_33491') where idOrder = 33491
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_33492') where idOrder = 33492
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_33817') where idOrder = 33817
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_35266') where idOrder = 35266
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_35460') where idOrder = 35460
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_35799') where idOrder = 35799
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_36172') where idOrder = 36172
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_43493') where idOrder = 43493
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_44807') where idOrder = 44807
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_45529') where idOrder = 45529
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_45967') where idOrder = 45967
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_46026') where idOrder = 46026
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_46196') where idOrder = 46196
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_49068') where idOrder = 49068
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50032') where idOrder = 50032
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50079') where idOrder = 50079
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50519') where idOrder = 50519
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50730') where idOrder = 50730
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50761') where idOrder = 50761
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50919') where idOrder = 50919
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_51186') where idOrder = 51186
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_51705') where idOrder = 51705
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_52860') where idOrder = 52860
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_56893') where idOrder = 56893
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59756') where idOrder = 59756
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59797') where idOrder = 59797
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59801') where idOrder = 59801
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59820') where idOrder = 59820
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59973') where idOrder = 59973
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_61049') where idOrder = 61049
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_62315') where idOrder = 62315
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_64834') where idOrder = 64834
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_65634') where idOrder = 65634
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_65907') where idOrder = 65907
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_66106') where idOrder = 66106
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_68033') where idOrder = 68033
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_70083') where idOrder = 70083
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_71226') where idOrder = 71226
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_71435') where idOrder = 71435
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_73600') where idOrder = 73600
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_73601') where idOrder = 73601
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_73625') where idOrder = 73625
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_73878') where idOrder = 73878
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_74813') where idOrder = 74813

SET NOCOUNT ON 

PRINT '--==========--'
PRINT 'ENDS DISCOUNT HANDLING'
PRINT '--==========--'

USE master
GO
