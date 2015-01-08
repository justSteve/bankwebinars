UPDATE  CUWebinars.dbo.Presenter
SET     BiographyLong = REPLACE(BiographyLong, 'https://ttseast.blob.core.windows.net', 'http://images.ttstrain.com') ,
        PhotoFull = REPLACE(PhotoFull, 'https://ttseast.blob.core.windows.net', 'http://images.ttstrain.com') ,
        PhotoThumb = REPLACE(PhotoThumb, 'https://ttseast.blob.core.windows.net', 'http://images.ttstrain.com')
WHERE   idUser > 1

INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4591, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4593, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4596, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4597, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4598, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4601, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4602, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4606, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4617, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4622, 45 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4623, 45 )

INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 1473, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4558, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4559, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4560, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4561, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4562, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4563, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4564, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4565, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4566, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4567, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4568, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4570, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4571, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4572, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4574, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4578, 46 )
INSERT  CUWebinars.dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4580, 46 )

SET IDENTITY_INSERT CUWebinars.dbo.Institution ON

INSERT  CUWebinars.dbo.Institution
        ( idInstitution ,
          InstitutionName ,
          InstitutionType ,
          domainName ,
          RegIdentifier ,
          Address ,
          City ,
          State ,
          Zip
        )
        ( SELECT    idInstitution ,
                    InstitutionName ,
                    InstitutionType ,
                    domainName ,
                    RegIdentifier ,
                    Address ,
                    City ,
                    State ,
                    Zip
          FROM      [CUWebinars-2015-1-7-13-40].dbo.Institution
          WHERE     idInstitution NOT IN ( SELECT   idInstitution
                                           FROM     CUWebinars.dbo.Institution )
        )

SET IDENTITY_INSERT CUWebinars.dbo.Institution OFF
INSERT  CUWebinars.dbo.WebUser
        ( idUser ,
          UserType ,
          AcctStatus ,
          DateCreated ,
          FirstName ,
          LastName ,
          Initial ,
          idUserInstitution ,
          email ,
          futureMail ,
          generalComments ,
          taxExempt ,
          idSubscriptionDiscount ,
          timeZone ,
          Title
        
        )
        ( SELECT    idUser ,
                    UserType ,
                    AcctStatus ,
                    DateCreated ,
                    FirstName ,
                    LastName ,
                    Initial ,
                    idUserInstitution ,
                    email ,
                    futureMail ,
                    generalComments ,
                    taxExempt ,
                    idSubscriptionDiscount ,
                    timeZone ,
                    Title
          FROM      [CUWebinars-2015-1-7-13-40].dbo.WebUser
          WHERE     UserType = 1
                    AND idUser NOT IN ( SELECT  idUser
                                        FROM    CUWebinars.dbo.WebUser )
        )


SET IDENTITY_INSERT CUWebinars.dbo.Address ON
INSERT  CUWebinars.dbo.Address
        ( Id ,
          AddressType ,
          Name ,
          Phone ,
          StreetAddress ,
          StreetAddress2 ,
          City ,
          Zip ,
          State ,
          Country ,
          idUser
        )
        ( SELECT    Id ,
                    AddressType ,
                    Name ,
                    Phone ,
                    StreetAddress ,
                    StreetAddress2 ,
                    City ,
                    Zip ,
                    State ,
                    Country ,
                    idUser
          FROM      [CUWebinars-2015-1-7-13-40].dbo.Address
        )
SET IDENTITY_INSERT CUWebinars.dbo.Address OFF
SET IDENTITY_INSERT CUWebinars.dbo.[Order] ON
INSERT  CUWebinars.dbo.[Order]
        ( idOrder ,
          idUser ,
          idAffiliate ,
          OrderDate ,
          OrderStatus ,
          Total ,
          FirstName ,
          LastName ,
          Institution ,
          BillingPhone ,
          BillingEmail ,
          BillingAddress ,
          BillingAddress2 ,
          BillingCity ,
          BillingState ,
          BillingZip ,
          ShippingFirstName ,
          ShippingLastName ,
          ShippingPhone ,
          ShippingAddress ,
          ShippingAddress2 ,
          ShippingCity ,
          ShippingState ,
          ShippingZip ,
          PaymentType ,
          UserComments ,
          AuditInfo ,
          AffiliateComments ,
          AdminComments ,
          TaxExempt ,
          Origin
        )
        ( SELECT    idOrder ,
                    idUser ,
                    idAffiliate ,
                    OrderDate ,
                    OrderStatus ,
                    Total ,
                    FirstName ,
                    LastName ,
                    Institution ,
                    BillingPhone ,
                    BillingEmail ,
                    BillingAddress ,
                    BillingAddress2 ,
                    BillingCity ,
                    BillingState ,
                    BillingZip ,
                    ShippingFirstName ,
                    ShippingLastName ,
                    ShippingPhone ,
                    ShippingAddress ,
                    ShippingAddress2 ,
                    ShippingCity ,
                    ShippingState ,
                    ShippingZip ,
                    PaymentType ,
                    UserComments ,
                    AuditInfo ,
                    AffiliateComments ,
                    AdminComments ,
                    TaxExempt ,
                    Origin
          FROM      [CUWebinars-2015-1-7-13-40].dbo.[Order]
          WHERE     idUser IN ( SELECT  idUser
                                FROM    [CUWebinars-2015-1-7-13-40].dbo.WebUser )
        )
SET IDENTITY_INSERT CUWebinars.dbo.[Order] OFF



PRINT '-----------------Seeder '
PRINT 'finished '
SELECT TOP 1
        Date ,
        LTRIM(RTRIM(Title)) ,
        idWebinar
FROM    CUWebinars.dbo.Webinar
WHERE   Status = 2
ORDER BY Date





