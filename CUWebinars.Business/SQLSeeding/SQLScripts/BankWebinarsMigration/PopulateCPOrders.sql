UPDATE orders SET state = NULL WHERE state = ''
--80477
SELECT  o.idOrder ,
        orderDate ,
        'Compliance Perspectives' AS Webinar ,
        842 AS ID ,
        ( SELECT    registrationType
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder 
        ) AS RegistrationType,
        o.firstName ,
        o.lastName ,
        ISNULL(o.customerInstitution,
               ( SELECT name
                 FROM   dbo.Institution
                 WHERE  idInstitution = ( SELECT 
                                                    idUserInstitution
                                          FROM      dbo.Users 
                                          WHERE     idUser = o.idUser
                                        )
                        AND idUser = o.idUser
               )) AS Institution ,
        'na' AS Title ,
        o.email ,
        o.phone ,
        o.address AS Address,
        'na' AS Address2,
        o.city ,
        ISNULL(o.state, 'na') AS State,
        o.zip ,
        o.idAffiliate
FROM    TTSWebinars2.dbo.Orders o
        INNER JOIN dbo.OrdersRows r ON r.idOrder = o.idOrder
WHERE   r.idWebinar = 842


		 -----------------above code generates statement below------------

		 