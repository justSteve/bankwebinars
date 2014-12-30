USE TTSWebinars2
GO
DECLARE @idwebinar INT
SET @idwebinar = 1720
SELECT  ( SELECT    o.idAffiliate
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS AffiliateID ,
        ( SELECT    idWebinar
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS WebinarID ,
        ( SELECT    CASE ( SELECT   optionLabel
                           FROM     dbo.Options
                           WHERE    idOption = dbo.OrdersRows.registrationType
                                    AND o.idOrder = idOrder
                         )
                      WHEN 'Live Session Only' THEN 'Live Plus Five'
                      WHEN 'CD-ROM and Hardcopy Handouts' THEN 'CD-ROM and Hardcopy Handouts'
                      WHEN 'OnDemand Recording Only' THEN 'OnDemand Recording Only'
                      WHEN '6-Month OnDemand Weblink' THEN 'OnDemand Recording Only'
                      WHEN 'Live plus OnDemand Weblinks' THEN ' Live Plus Six'
                      WHEN 'Premier Package' THEN 'Premier Package'
                    END
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS RegistrationType ,
        o.firstName AS FirstName ,
        o.lastName AS LastName ,
        'na' AS Title ,
        ISNULL(o.customerInstitution, ( SELECT  name
                                        FROM    dbo.Institution
                                        WHERE   idInstitution = ( SELECT    idUserInstitution
                                                                  FROM      dbo.Users
                                                                  WHERE     idUser = o.idUser
                                                                )
                                                AND idUser = o.idUser
                                      )) AS Institution ,
        REPLACE(o.email, '@', '@NN') AS Email ,
        o.phone AS Phone ,
        o.address AS Address ,
        'na' AS Address2 ,
        o.city AS City ,
        ISNULL(o.state, 'na') AS State ,
        o.zip AS Zip ,
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
WHERE   r.idWebinar = @idwebinar
        AND ( r.status < 5
              AND r.status > 1
            )
ORDER BY LastName
GO


