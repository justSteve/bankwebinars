USE TTSWebinars2
GO
--UPDATE dbo.users SET userType =2  WHERE idUser = 33194
--UPDATE dbo.users SET userType =3  WHERE idUser = 33192
--SELECT * FROM dbo.Users WHERE idUser = 33194

--SELECT * FROM dbo.Options WHERE idOption = (SELECT registrationType FROM dbo.OrdersRows WHERE idOrder = 71270)
--DELETE FROM dbo.Orders WHERE idOrder = 74973  
--UPDATE dbo.OrdersRows SET rowPrice = 1749, unitPrice = 1749, WHERE idOrder = 71270  
--UPDATE dbo.Orders SET total = 1749 WHERE idOrder = 21274
--SELECT r.* FROM dbo.Orders o INNER JOIN dbo.OrdersRows r ON r.idOrder = o.idOrder WHERE o.idOrder = 21274  
SELECT * FROM dbo.Webinar WHERE status = 2 AND duration = 1
--SELECT FirstName, LastName, 'na', 'TTS', REPLACE(email, 'admin', ''), * FROM dbo.USERS WHERE UserType = 4 and email LIKE '%ttstrain.com'
DECLARE @idwebinar INT
SET @idwebinar = 1736


--SELECT * INTO '+ GEtdate() 

SELECT  ( SELECT    o.idAffiliate
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS AffiliateID ,
        ( SELECT    idWebinar
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS WebinarID ,
		r.registrationType AS idRegType,
        o.firstName AS FirstName ,
        o.lastName AS LastName ,
        '' AS Title ,
        ISNULL(o.customerInstitution, ( SELECT  name
                                        FROM    dbo.Institution
                                        WHERE   idInstitution = ( SELECT    idUserInstitution
                                                                  FROM      dbo.Users
                                                                  WHERE     idUser = o.idUser
                                                                )
                                                AND idUser = o.idUser
                                      )) AS Institution ,
        --REPLACE(o.email, '@', '@NN') AS Email ,
		email AS Email,
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
        o.shippingZip,
		o.total,
		r.shipmentDate,
		o.storeCommentsPriv,
		o.idOrder,
		o.orderDate,
		r.status
FROM    TTSWebinars2.dbo.Orders o
        INNER JOIN dbo.OrdersRows r ON r.idOrder = o.idOrder
WHERE   r.idWebinar = @idwebinar
        AND ( r.status < 5
              AND r.status > 1
            )

ORDER BY orderDate desc
GO


        --( SELECT    CASE ( SELECT   optionLabel
        --                   FROM     dbo.Options
        --                   WHERE    idOption = dbo.OrdersRows.registrationType
        --                            AND o.idOrder = idOrder
        --                 )
        --              WHEN 'Live Session Only' THEN 'Live Plus Five'
        --              WHEN 'CD-ROM and Hardcopy Handouts' THEN 'CD-ROM and Hardcopy Handouts'
        --              WHEN 'OnDemand Recording Only' THEN 'OnDemand Recording Only'
        --              WHEN '6-Month OnDemand Weblink' THEN 'OnDemand Recording Only'
        --              WHEN 'Live plus OnDemand Weblinks' THEN 'Live Plus Six'
        --              WHEN 'Premier Package' THEN 'Premier Package'
        --            END
        --  FROM      dbo.OrdersRows
        --  WHERE     o.idOrder = idOrder
        --) AS RegistrationType ,