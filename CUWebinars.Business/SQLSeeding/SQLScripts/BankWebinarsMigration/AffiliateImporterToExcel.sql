USE TTSWebinars2
GO

SELECT 
        ( SELECT    idWebinar
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS WebinarID ,
        ( SELECT    ( SELECT    optionLabel
                      FROM      dbo.Options
                      WHERE     idOption = dbo.OrdersRows.registrationType
                                AND o.idOrder = idOrder
                    )
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS RegistrationType ,
        o.firstName AS FirstName ,
        o.lastName AS LastName ,
        'na' AS Title ,
        ISNULL(o.customerInstitution,
               ( SELECT name
                 FROM   dbo.Institution
                 WHERE  idInstitution = ( SELECT    idUserInstitution
                                          FROM      dbo.Users
                                          WHERE     idUser = o.idUser
                                        )
                        AND idUser = o.idUser
               )) AS Institution ,
        REPLACE(o.email, '@', '@_') AS Email ,
        o.phone AS Phone ,
        o.address AS Address ,
        'na' AS Address2 ,
        o.city AS City ,
        ISNULL(o.state, 'na') AS State ,
        o.zip AS Zip ,
        ( SELECT    o.idAffiliate
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS AffiliateID ,
        '' AS DiscountCode ,--r.idDiscount ,
        ( (SELECT   ISNULL(additional_locations_emails, '')
           FROM     dbo.OrdersRowsOptions
           WHERE    r.idOrderRow = idOrderRow)
        ) AS AdditionalLocations ,
        o.shippingFirstName ,
        o.shippingLastName ,
        o.shippingPhone ,
        o.shippingAddress ,
        o.shippingCity ,
        o.shippingState ,
        o.shippingZip
FROM    TTSWebinars2.dbo.Orders o
        INNER JOIN dbo.OrdersRows r ON r.idOrder = o.idOrder
WHERE   r.idWebinar = 842


GO


USE BankWebinars
GO

DROP PROC FindRegTypeID
