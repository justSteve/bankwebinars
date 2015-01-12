USE TTSWebinars2
GO

DELETE MembershipReboot.dbo.UserAccounts WHERE Tenant = 'bankwebinars' AND Email NOT IN ('affiliate@ttstrain.com')

DELETE BankWebinars.dbo.Affiliate WHERE idUserAff > 19
DELETE BankWebinars.dbo.[Order] WHERE idUser > 19
DELETE BankWebinars.dbo.WebUser WHERE email NOT IN ('affiliate@ttstrain.com') AND UserType <> 3


DECLARE @idwebinar INT

SET @idwebinar = ( SELECT TOP 1
                            idWebinar
                   FROM     dbo.Webinar
                   WHERE    status = 2
                   ORDER BY date
                 )
DECLARE @idUser INT
DECLARE @importCount INT

DECLARE migrate_cursor CURSOR
FOR
    SELECT  idUser
    FROM    dbo.Affiliate
    --WHERE   idUser > 19
    ORDER BY idUser
OPEN migrate_cursor


FETCH NEXT FROM migrate_cursor
INTO @idUser

WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @importCount = @importCount + 1
        SET NOCOUNT ON 
        SELECT  
                19 AS AffiliateID ,
                @idwebinar AS WebinarID ,
                209 AS idRegType ,
                u.firstName AS FirstName ,
                u.lastName AS LastName ,
                'na' AS Title ,
                ISNULL(u.provisionalInstitution, ( SELECT   name
                                                   FROM     dbo.Institution
                                                   WHERE    idInstitution = ( SELECT    idUserInstitution
                                                                              FROM      dbo.Users
                                                                              WHERE     idUser = @idUser
                                                                            )
                                                            AND idUser = @idUser
                                                 )) AS Institution ,
                ( SELECT    u.email
                  FROM      dbo.Users
                  WHERE     idUser = @idUser
                ) AS Email ,
                u.phone1 AS Phone ,
                u.mAddress AS Address ,
                'na' AS Address2 ,
                u.mCity AS City ,
                ISNULL(u.mState, 'na') AS State ,
                u.mZip AS Zip ,
                '' AS DiscountCode ,--r.idDiscount ,
                '' AS AdditionalLocations ,
                u.shippingFirstName ,
                u.shippingLastName ,
                u.shippingPhone ,
                u.shippingAddress ,
                u.shippingCity ,
                u.shippingState ,
                u.shippingZip ,
                0 ,
                '' ,
                '' ,
                0 ,
                GETDATE() ,
                2
        FROM    TTSWebinars2.dbo.Users u
        --INNER JOIN dbo.OrdersRows r ON r.idOrder = u.idOrder
        WHERE   u.idUser = @idUser
        ORDER BY LastName


        FETCH NEXT FROM migrate_cursor
   INTO @idUser
    END

CLOSE migrate_cursor
DEALLOCATE migrate_cursor

GO


