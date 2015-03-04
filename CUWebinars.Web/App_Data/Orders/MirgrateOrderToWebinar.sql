USE TTSWebinars2
GO
--SELECT TOP 100 * FROM dbo.Orders o INNER JOIN	dbo.OrdersRows r ON r.idOrder = o.idOrder
--WHERE o.idAffiliate = 62 AND 
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
        'Migrated 2/22/2015 ' + ISNULL(o.storeCommentsPriv, '') AS StoreComments ,
        o.idOrder ,
        o.orderDate ,
        r.status
FROM    TTSWebinars2.dbo.Orders o
        INNER JOIN dbo.OrdersRows r ON r.idOrder = o.idOrder
WHERE   --r.idWebinar IN ( SELECT idWebinar
--                         FROM   dbo.Webinar
--                         WHERE  status = 2
--                                OR status = 3 )
        ( r.idWebinar != 1713
          OR r.idWebinar = 1760
        )
        AND o.idOrder IN ( 65680, 65348, 65815, 72007, 75651, 77750, 65894, 65918, 77755, 66111, 77775, 76397, 66250,
                           66254, 66109, 77783, 65127, 65380, 74693, 65403, 77661, 65919, 65338, 75775, 65351, 65352,
                           65844, 69212, 69213, 69214, 77406, 78059, 78060, 78061, 65383, 66204, 65636, 65272, 71427,
                           65637, 70840, 65819, 66069, 65521, 65704, 65963, 77892, 78151, 78152, 78153, 65769, 65193,
                           65497, 77664, 66009, 69775, 71297, 67153, 66160, 77026, 65080, 76128, 65162, 65203, 65529,
                           65393, 65523, 65670, 65736, 65821, 77778, 66043, 77030, 77368, 77549, 66027, 67509, 66148,
                           65543, 66703, 65699, 77560, 65970, 65562, 76663, 67989, 65320, 65825, 76277, 64857, 65471,
                           65197, 77869, 70121, 64933, 68091, 66303, 66890, 66891, 66892, 66893, 76039, 77047, 69327,
                           65312, 76339, 65385, 65468, 65476, 74813, 66053, 66042, 70969, 77995, 65727, 77085, 65696,
                           66068, 65596, 65332, 65701, 65354, 76198, 65268, 65498, 77914, 65807, 71013, 65762, 71932,
                           73878, 76036, 76168, 77996, 65513, 77834, 65546, 76007, 65561, 65818, 65887, 65330, 72382,
                           65373, 77023, 64946, 65544, 77125, 65295, 77890, 65186, 65494, 65257, 64806, 65831, 65377,
                           65375, 65495, 64915, 70540, 71403, 71399, 65491, 67404, 65610, 65499, 65826, 65192, 65817,
                           73926, 65289, 66011, 65139, 65135, 77409, 77690, 65475, 75508, 64704, 65043, 65581, 77714,
                           67013, 73987, 77165, 65913, 65608, 64911, 65548, 69229, 64638, 67010, 66030, 75724, 65401,
                           77637, 66327, 66112, 65240, 64567, 77333, 77612, 65391, 77497, 64969, 65185, 65855, 77880,
                           66187, 65592, 65823, 76005, 65231, 64709, 70778, 77428, 65343, 65213, 64913, 70890, 65333,
                           65589, 71989, 65265, 65969, 72158, 65528, 65074, 78002, 77050, 65414, 65527, 77446, 65267,
                           65840, 65568, 65347, 76241, 69610, 77955, 65472, 65516, 65030, 72316, 64473, 77450, 77752,
                           71041, 66179, 77962, 66107, 65150, 77514, 65526, 72143, 77795, 66131, 71895, 71894, 65044,
                           65724, 65040, 65144, 65893, 77481, 77405, 77605, 65125, 71404, 65715, 66219, 65756, 65723,
                           65930, 77888, 71701, 65846, 65728, 65180, 69097, 70732, 67007, 65342, 77130, 77779, 65870,
                           65781, 65997, 66016, 70155, 66029, 73518, 65167, 65609, 65075, 65857, 65679, 71488, 65453,
                           77024, 77452, 70004, 65258, 65350, 65889, 70120, 65967, 66002, 70033, 77476, 65178, 65910,
                           64754, 69080, 65994, 65156, 65230, 65914, 75442, 65547, 66088, 65446, 65371, 77707, 71717,
                           65549, 65628, 65810, 75718, 65878, 66706, 77107, 65142, 65321, 77719, 71921, 67024, 69123,
                           71594, 65950, 66337, 66338, 66966, 65897, 70028, 77090, 65659, 64856, 67360, 76130, 65400,
                           77902, 78159, 78160, 78161, 65451, 65879, 65942, 65816, 65407, 72098, 71985, 65908, 65372,
                           65392, 65820, 75843, 76038, 71026, 71037, 77836, 65340, 76275, 71971, 75004, 77904, 65754,
                           70967, 77696, 65658, 64937, 65885, 72057, 65553, 64955, 65263, 65951, 77861, 65345, 71727,
                           65657, 77467, 67063, 71090, 64487, 74841, 66313, 71356, 71354, 74763, 77265, 65110, 70957,
                           65463, 65725, 65640, 65394, 65648, 77025, 65328, 66087, 65217, 65216, 65226, 69942, 70099,
                           65454, 65877, 77603, 65902, 73600, 76151, 65656, 75741, 77841, 76179, 69697, 65915, 77790,
                           77160, 77104, 66157, 65545, 65402, 65026, 77485, 65360, 64620, 64619, 66070, 65319, 71507,
                           71489, 65374, 65824, 74847, 64936, 65151, 64939, 65252, 65726, 73693, 65598, 66154, 66926,
                           77829, 65421, 71654, 65672, 65859, 77932, 66924, 77952, 77610, 70538, 70539, 70566, 65998,
                           65789, 66028, 76030, 65822, 78181, 64591, 77557, 65171, 77594, 67264, 66681, 35799, 77788,
                           64914, 65418, 65971, 65664, 77334, 77693, 69713, 67120, 64916, 77685, 77984, 65943, 77102,
                           65379, 71016, 65381, 77901, 65413, 65655, 77413, 78067, 78068, 78069, 65376, 74878, 70542,
                           75635, 65111, 65774, 65574, 65890, 65978, 65570, 65901, 76235, 67532, 77021, 65790, 64682,
                           65355, 64683, 65361, 65865, 65858, 65872, 66984, 67334, 69224, 69256, 69768, 74701, 65873,
                           69222, 69225, 64953, 65331, 77028, 71829, 71820, 65871, 65129, 66082, 71197, 65435, 77078,
                           70715, 70927, 77536, 70860, 66006, 76132, 65834, 65594, 77393, 66005, 64737, 64853, 65071,
                           65587, 65260, 65349, 77148, 70772, 65283, 65229, 65477, 65293, 65753, 65752, 65737, 65702,
                           77858, 64705, 64863, 65830, 65604, 65423, 77511, 75992, 78031, 78032, 78033, 71084, 66188,
                           65785, 70543, 65746, 71006, 65533, 65600, 66323, 66905, 66906, 66907, 66908, 65508, 66004,
                           69219, 65448, 65264, 65396, 65433, 75422, 70065, 71723, 64677, 65160, 64942, 65060, 65398,
                           70894, 67867, 70729, 71822, 65892, 65945, 77792, 65517, 65502, 64998, 75239, 65514, 65181,
                           65085, 64801, 65405, 64870, 65625, 65552, 65253, 65928, 65665, 65478, 71794, 65692, 74893,
                           64964, 75527, 65524, 65062, 65759, 78005, 78167, 78168, 78169, 65122, 65434, 65187, 65313,
                           77307, 64663, 65912, 65408, 77656, 77558, 76297, 65572, 77531, 77793, 65722, 65586, 64924,
                           65470, 65123, 75940, 78023, 78024, 78025, 67606, 65163, 75314, 66026, 77440, 65199, 64934,
                           69273, 65654, 65119, 65104, 66090, 64579, 67545, 73594, 65955, 65200, 67766, 65541, 65607,
                           77720, 66046, 76634, 66065, 64817, 65459, 75958, 78027, 78028, 78029 )
--o.idOrder < 49709 --  AND r.idWebinar IN (SELECT r.idWebinar FROM dbo.Webinar WHERE status = 2 OR status = 3)
        AND ( r.status < 5
              AND r.status > 1
            )
        AND o.idAffiliate > 1
ORDER BY o.orderDate DESC
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