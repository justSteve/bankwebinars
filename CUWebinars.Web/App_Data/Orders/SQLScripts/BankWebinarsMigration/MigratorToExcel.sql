USE TTSWebinars2
GO

DECLARE @idwebinar INT
SET @idwebinar = 1720

SELECT  o.idOrder ,
        orderDate ,
        ( SELECT    title
          FROM      dbo.Webinar
          WHERE     idWebinar = @idwebinar
        ) AS Webinar ,
        @idwebinar AS ID ,
        CASE ( SELECT   registrationType
               FROM     dbo.OrdersRows
               WHERE    o.idOrder = idOrder
			   AND o.orderDate > '01/01/2015'
             )
          WHEN 1 THEN 205
          WHEN 16 THEN 206
          WHEN 3 THEN 207
          WHEN 17 THEN 208
          WHEN 18 THEN 209
		  --1hr
          WHEN 27 THEN 200
          WHEN 32 THEN 206
          WHEN 35 THEN 207
          WHEN 33 THEN 208
          WHEN 36 THEN 209
          ELSE (SELECT   registrationType
               FROM     dbo.OrdersRows
               WHERE    o.idOrder = idOrder)
        END AS RegistrationType ,
        o.firstName ,
        o.lastName ,
        ISNULL(o.customerInstitution, ( SELECT  name
                                        FROM    dbo.Institution
                                        WHERE   idInstitution = ( SELECT    idUserInstitution
                                                                  FROM      dbo.Users
                                                                  WHERE     idUser = o.idUser
                                                                )
                                                AND idUser = o.idUser
                                      )) AS Institution ,
        'na' AS Title ,
        o.email ,
        o.phone ,
        o.address AS Address ,
        'na' AS Address2 ,
        o.city ,
        ISNULL(o.state, 'na') AS State ,
        o.zip ,
        o.idAffiliate ,
        r.idDiscount ,
        CASE r.status
          WHEN 1 THEN 'Inprocess'
          WHEN 2 THEN 'Submitted'
          WHEN 3 THEN 'Billed'
          WHEN 4 THEN 'Paid'
          WHEN 5 THEN 'Abandoned'
          WHEN 6 THEN 'Canceled'
          WHEN 7 THEN 'AwaitingConfirmation'
          WHEN 255 THEN 'Unknown'
        END ,
        '' ,
        o.total ,
        o.shippingFirstName ,
        o.shippingLastName ,
        o.shippingPhone ,
        o.shippingAddress ,
        o.shippingCity ,
        o.shippingState ,
        o.shippingZip ,
        o.idUser ,
        ( (SELECT   ISNULL(additional_locations_emails, '')
           FROM     dbo.OrdersRowsOptions
           WHERE    r.idOrderRow = idOrderRow)
        ) AS AdditionalLocations
FROM    TTSWebinars2.dbo.Orders o
        INNER JOIN dbo.OrdersRows r ON r.idOrder = o.idOrder
WHERE   r.idWebinar = @idwebinar
        AND ( r.status > 1
              AND r.status < 5
            )
        --AND orderDate > '01/01/2015'
ORDER BY orderDate DESC

		 -----------------above code generates statement below------------

		 