USE TTSWebinars2
GO

SELECT  *
FROM    dbo.Discounts
WHERE   idDiscounts IN ( SELECT idSubscriptionDiscount
                         FROM   Users
                         WHERE  idSubscriptionDiscount IS NOT NULL )

USE BankWebinars 
GO
UPDATE dbo.WebUser SET idSubscriptionDiscount = 50, LastName = 'kdkdkdkddk'  WHERE idUser = 31660
SELECT * FROM dbo.WebUser WHERE idUser = 31660
SELECT * FROM dbo.Discount
SELECT * FROM dbo.[Order]