USE TTSWebinars2
GO
--SELECT (SELECT optionGroupDesc FROM dbo.OptionsGroups WHERE idOptionGroup = o.idoptiongroup),  * FROM dbo.OptionsGroupsXref o WHERE idWebinar IN (SELECT idWebinar FROM dbo.Webinar WHERE status =2) 
--SELECT TOP 100 * FROM dbo.Orders o INNER JOIN	dbo.OrdersRows r ON r.idOrder = o.idOrder
----WHERE o.idOrder NOT IN (78945,78944,78861,78851,78831,78803,78764,78735,78640,78639,78508,78488,78442,78353,78331,78324,78321,78300,78298,78295,78283,78282,78259,78228,78208,78204,78187,78177,78174,78170,77898,77897,77857,77828,77811,77798)
--AND r.status > 1 AND r.status < 5
--AND r.idWebinar = 1745
--ORDER BY o.orderDate desc

SELECT  ( SELECT    o.idAffiliate
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
        'Migrated on: ' + GETDATE() + ISNULL(o.storeCommentsPriv, '') AS StoreComments ,
        o.idOrder ,
        o.orderDate ,
        r.status
FROM    TTSWebinars2.dbo.Orders o
        INNER JOIN dbo.OrdersRows r ON r.idOrder = o.idOrder
WHERE   r.idWebinar IN (1745)
AND o.idOrder NOT IN (78959,78954,78952,78947,78938,78937,78936,78935,78928,78917,78915,78858,78842,78827,78816,78808,78717,78642,78604,78514,78509,78470,78467,78434,78431,78348,78329,78320,78306,78296,78288,78280,78279,78269,78258,78251,78237,78231,78222,78195,78194,77907,77899,77860,77848,77843,77812,77801,77784,77704,77642,77584,77583,77581,77580,77579,77578,77577,77576,77575,77574,77573)
        
--o.idOrder < 49709 --  AND r.idWebinar IN (SELECT r.idWebinar FROM dbo.Webinar WHERE status = 2 OR status = 3)
        AND ( r.status < 5
              AND r.status > 1
            )
        AND o.idAffiliate > 1
ORDER BY o.email DESC
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