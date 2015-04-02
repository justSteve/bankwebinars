USE TTSWebinars2
GO

--SELECT (SELECT optionGroupDesc FROM dbo.OptionsGroups WHERE idOptionGroup = o.idoptiongroup),  * FROM dbo.OptionsGroupsXref o WHERE idWebinar IN (SELECT idWebinar FROM dbo.Webinar WHERE status =2) 
--SELECT TOP 500 * FROM dbo.Orders o INNER JOIN	dbo.OrdersRows r ON r.idOrder = o.idOrder
--WHERE o.idAffiliate = 62
------AND r.status > 1 AND r.status < 5
----AND r.idWebinar = 1745
--ORDER BY o.orderDate desc


SELECT --(SELECT idSubscriptionDiscount FROM USERs WHERE idUser = o.idUser ), 
( SELECT    o.idAffiliate
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS AffiliateID ,
        ( SELECT    idWebinar
          FROM      dbo.OrdersRows
          WHERE     o.idOrder = idOrder
        ) AS WebinarID ,
					--idRegType	RegTypeExplain
			--221	Register for all five events and get five days access to the OnDemand Playback. You'll have an opportunity to ask questions during the presentation <i>and</i> be free to review the content for the next 5 (business) days.  Registration also includes links to presenter materials, handouts, and pdfs.
			--222	Interested in the topic but unable to attend the regularly scheduled event? Purchase the recorded version and receive OnDemand playback for 6 months (includes presenter materials).
			--223	Attend live; includes six <i>months</i> access to OnDemand playback. Combine the advantages of live attendance with an unlimited number of replays for the next six months.
			--224	CD-ROM plus Hardcopy Handouts. This option includes 6-months OnDemand playback but does <i>not</i> include live session.
			--225	Includes all three options above.  Live, OnDemand playback, <i>and</i> CD-ROM plus Hardcopy Handouts.
        ( CASE r.idWebinar
            WHEN 1711 THEN ( CASE r.registrationType
                               WHEN 79 THEN 221
                               WHEN 80 THEN 222
                               WHEN 82 THEN 223
                               WHEN 81 THEN 224
                               WHEN 83 THEN 225
                             END )
            WHEN 1713 THEN ( CASE r.registrationType
                               WHEN 79 THEN 221
                               WHEN 80 THEN 222
                               WHEN 82 THEN 223
                               WHEN 81 THEN 224
                               WHEN 83 THEN 225
                             END )
            WHEN 1714 THEN ( CASE r.registrationType
                               WHEN 79 THEN 221
                               WHEN 80 THEN 222
                               WHEN 82 THEN 223
                               WHEN 81 THEN 224
                               WHEN 83 THEN 225
                             END )
            WHEN 1715 THEN ( CASE r.registrationType
                               WHEN 79 THEN 221
                               WHEN 80 THEN 222
                               WHEN 82 THEN 223
                               WHEN 81 THEN 224
                               WHEN 83 THEN 225
                             END )
            WHEN 1716 THEN ( CASE r.registrationType
                               WHEN 79 THEN 221
                               WHEN 80 THEN 222
                               WHEN 82 THEN 223
                               WHEN 81 THEN 224
                               WHEN 83 THEN 225
                             END )
            --WHEN 1790 THEN ( CASE r.registrationType
            --                   WHEN 79 THEN 221
            --                   WHEN 80 THEN 222
            --                   WHEN 82 THEN 223
            --                   WHEN 81 THEN 224
            --                   WHEN 83 THEN 225
            --                 END )
            ELSE r.registrationType
          END ) AS idRegType ,
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
        email AS Email ,
        o.phone AS Phone ,
        o.address AS Address ,
        '' AS Address2 ,
        o.city AS City ,
        ISNULL(o.state, 'na') AS State ,
        o.zip AS Zip ,
        ISNULL(CAST(r.idDiscount AS VARCHAR), '') AS DiscountCode ,--r.idDiscount ,
        ( SELECT    REPLACE(ISNULL(additional_locations_emails, ''), 'NULL', '')
          FROM      dbo.OrdersRowsOptions
          WHERE     r.idOrderRow = idOrderRow
        ) AS AdditionalLocations ,
        REPLACE(ISNULL(o.shippingFirstName, o.firstName), 'NULL', o.firstName) AS firstname ,
        REPLACE(ISNULL(o.shippingLastName, o.lastName), 'NULL', o.lastName) AS lastname ,
        REPLACE(ISNULL(o.shippingPhone, o.phone), 'NULL', o.phone) AS phone ,
        REPLACE(ISNULL(o.shippingAddress, o.address), 'NULL', o.address) AS address ,
        REPLACE(ISNULL(o.shippingCity, o.city), 'NULL', o.city) AS city ,
        REPLACE(ISNULL(o.shippingState, o.state), 'NULL', o.state) AS St ,
        REPLACE(ISNULL(o.shippingZip, o.zip), 'NULL', o.zip) AS Zip ,
        o.total ,
        r.shipmentDate ,
        'Migrated on: ' + CAST(GETDATE() AS VARCHAR) + ISNULL(o.storeCommentsPriv, '') AS StoreComments ,
        o.idOrder ,
        o.orderDate ,
        r.status
FROM    TTSWebinars2.dbo.Orders o
        INNER JOIN dbo.OrdersRows r ON r.idOrder = o.idOrder
WHERE   r.status < 6
        AND r.status > 1
        AND r.idWebinar IN (1792)--( SELECT r.idWebinar FROM dbo.Webinar WHERE status = 2 OR status = 3) -- 1745 = understanding...
ORDER BY orderDate DESC
GO
