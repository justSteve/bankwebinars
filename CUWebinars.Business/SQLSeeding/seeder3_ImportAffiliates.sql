USE TTSWebinars
go
PRINT '--==========--'
PRINT 'BEGINS AFFILIATE IMPORTATION'
PRINT '--==========--'
EXEC migrator.dbo.[CallCreateUser] @qString = 'FirstName=Mark&LastName=Bennett&Email=affiliate@ttstrain.com&Institution=TTS&AddressType=Billing&City=city&Country=country&Name=Mark Bennett&Phone=608-849-5563&State=state&StreetAddress=street&StreetAddress2=street2&Zip=zip&Password=bennett....&ConfirmPassword=bennett....&userType=2&title=na&idWebUser=19'

DECLARE @id2Insert INT
--SET IDENTITY_INSERT dbo.Affiliate ON
DECLARE @defaultAffId int
SET @defaultAffId = (SELECT idUser FROM dbo.WebUser WHERE email = 'affiliate@ttstrain.com')
SET @id2Insert = 19
        INSERT  INTO [dbo].Affiliate
                ( idUserAff ,
                  [CommissionModel] ,
                  [URL] ,
                  [WebBanner] ,
                  [WebFooter] ,
                  [EmailBanner] ,
                  [EmailFooter] ,
                  [ttsDomain] ,
                  [GAPass] ,
                  [SupportEmail] ,
                  [DisplayTitle] ,
                  [BillingModel] ,
                  [Logo] ,
                  [ContactPerson] ,
                  [ContactPhone] ,
                  [ContactEmail] ,
                  [ContactFax] ,
                  [ContactAddress] ,
                  [TechEmail] ,
                  [TechPhone] ,
                  [TechName] ,
                  [EmailPromo] ,
                  [WebUser_Id]
                )
        VALUES  ( 19,
                  ( SELECT  CommissionModel
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  URL
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  WebBanner
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  WebFooter
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  EmailBanner
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  EmailFooter
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  ttsDomain
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  GAPass
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  SupportEmail
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  DisplayTitle
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  BillingModel
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  Logo
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  ContactPerson
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  ContactPhone
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  ContactEmail
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  ContactFax
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  ContactAddress
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  TechEmail
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  TechPhone
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  TechName
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  ( SELECT  EmailPromo
                    FROM    TTSWebinarsSeeder.dbo.Affiliate
                    WHERE   idUser = @id2Insert
                  ) ,
                  @defaultAffId
                )




--DECLARE @id2Insert INT
--DECLARE my_Cursor CURSOR
--FOR
--SELECT  idUser
--FROM    TTSWebinarsSeeder.dbo.Users
--WHERE   userType = 2
--        AND idUser <> 389
--        AND idUser <> 12041

--OPEN my_Cursor

--SET @id2Insert = 1
--DECLARE @bAddress INT
--DECLARE @sAddress INT
--DECLARE @InstitutionExists INT

--FETCH NEXT FROM my_Cursor INTO @id2Insert
--WHILE @@FETCH_STATUS = 0
--    BEGIN
  
--        PRINT '--==========--'
--        PRINT 'Processing UserID: ' + CAST(@id2Insert AS VARCHAR)
--        PRINT '--==========--'

  
--        IF NOT EXISTS ( SELECT  Id
--                        FROM    Institution
--                        WHERE   Id = ( SELECT   idUserInstitution
--                                       FROM     TTSWebinarsSeeder.dbo.users
--                                       WHERE    idUser = @id2Insert
--                                     ) ) 
--            BEGIN
--                PRINT 'Inserting Institution' + CAST(@id2Insert AS VARCHAR)
--                SET IDENTITY_INSERT dbo.Institution ON

--                INSERT  INTO [dbo].[Institution]
--                        ( Id ,
--                          [InstitutionName] ,
--                          [zip] ,
--                          [domainName]
--                        )
--                VALUES  ( ( SELECT  idInstitution
--                            FROM    TTSWebinarsSeeder.dbo.Institution
--                            WHERE   idInstitution = ( SELECT  idUserInstitution
--                                                      FROM    TTSWebinarsSeeder.dbo.users
--                                                      WHERE   idUser = @id2Insert
--                                                    )
--                          ) ,
--                          ( SELECT  name
--                            FROM    TTSWebinarsSeeder.dbo.Institution
--                            WHERE   idInstitution = ( SELECT  idUserInstitution
--                                                      FROM    TTSWebinarsSeeder.dbo.users
--                                                      WHERE   idUser = @id2Insert
--                                                    )
--                          ) ,
--                          ( SELECT  zip
--                            FROM    TTSWebinarsSeeder.dbo.Institution
--                            WHERE   idInstitution = ( SELECT  idUserInstitution
--                                                      FROM    TTSWebinarsSeeder.dbo.users
--                                                      WHERE   idUser = @id2Insert
--                                                    )
--                          ) ,
--                          ''
--	                    )

--                SET IDENTITY_INSERT dbo.Institution OFF
--            END
  
--        PRINT 'Institution Completed'      

--        SET IDENTITY_INSERT dbo.WebUser ON

--        INSERT  INTO [dbo].[WebUser]
--                ( Id ,
--                  [UserType] ,
--                  [AccountStatus] ,
--                  [Password] ,
--                  [DateCreated] ,
--                  [FirstName] ,
--                  [LastName] ,
--                  [InstitutionId] ,
--                  [Email] ,
--                  [FutureMail] ,
--                  [generalComments] ,
--                  [DiscountId] ,
--                  [timeZone] ,
--                  [LoggedIn]
--           --,[AddressId]
--                )
--        VALUES  ( @id2Insert ,
--                  2 ,
--                  'A' ,
--                  ( SELECT  acctPassword
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  DateCreated
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  FirstName
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  LastName
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  id
--                    FROM    dbo.Institution
--                    WHERE   InstitutionName = ( SELECT TOP 1 name
--                                                FROM    TTSWebinarsSeeder.dbo.Institution i
--                                                        INNER JOIN TTSWebinarsSeeder.dbo.Users u ON i.idInstitution = u.idUserInstitution
--                                                WHERE   u.idUser = @id2Insert
--                                              )
--                  ) ,
--                  --( SELECT  idUserInstitution
--                  --  FROM    TTSWebinarsSeeder.dbo.Users
--                  --  WHERE   idUser = @id2Insert
--                  --) ,
--                  ( SELECT  Email
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  FutureMail
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  generalComments
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  idSubscriptionDiscount
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  timeZone
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  hasLoggedIn
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  )
--                )
--        SET IDENTITY_INSERT dbo.WebUser OFF
--        PRINT 'inserted Affiliate: ' + CAST(@id2Insert AS VARCHAR)
       
--        INSERT  INTO [dbo].[Address]
--                ( [AddressType] ,
--                  [Name] ,
--                  [Phone] ,
--                  [StreetAddress] ,
--                  --[StreetAddress2] ,
--                  [City] ,
--                  [Zip] ,
--                  [State] ,
--                  [Country] ,
--                  [WebUser_Id]
--                )
--        VALUES  ( 'Billing' ,
--                  ( SELECT  firstName + ' ' + lastName
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ISNULL(( SELECT   Phone1
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'ph') ,
--                  ISNULL(( SELECT   mAddress
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'add') ,
--                  ISNULL(( SELECT   mCity
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'city') ,
--                  ISNULL(( SELECT   mZip
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'zip') ,
--                  ISNULL(( SELECT   mState
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'state') ,
--                  ISNULL(( SELECT   mCountry
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'US') ,
--                  @id2Insert
--                )

--        SELECT  @bAddress = SCOPE_IDENTITY()
--        PRINT 'Billing Address: ' + CAST(@bAddress AS VARCHAR)

--        INSERT  INTO [dbo].[Address]
--                ( [AddressType] ,
--                  [Name] ,
--                  [Phone] ,
--                  [StreetAddress] ,
--                  [City] ,
--                  [Zip] ,
--                  [State] ,
--                  [Country] ,
--                  [WebUser_Id]
--                )
--        VALUES  ( 'Shipping' ,
--                  ( SELECT  firstName + ' ' + lastName
--                    FROM    TTSWebinarsSeeder.dbo.Users
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ISNULL(( SELECT   Phone1
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'ph') ,
--                  ISNULL(( SELECT   mAddress
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'add') ,
--                  ISNULL(( SELECT   mCity
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'city') ,
--                  ISNULL(( SELECT   mZip
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'zip') ,
--                  ISNULL(( SELECT   mState
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'state') ,
--                  ISNULL(( SELECT   mCountry
--                           FROM     TTSWebinarsSeeder.dbo.Users
--                           WHERE    idUser = @id2Insert
--                         ), 'US') ,
--                  @id2Insert
--                )

--        SELECT  @sAddress = SCOPE_IDENTITY()
--        PRINT 'Shipping Address: ' + CAST(@sAddress AS VARCHAR)

--        --SET IDENTITY_INSERT dbo.Affiliate ON
--        INSERT  INTO [dbo].Affiliate
--                ( [Id] ,
--                  [CommissionModel] ,
--                  [URL] ,
--                  [WebBanner] ,
--                  [WebFooter] ,
--                  [EmailBanner] ,
--                  [EmailFooter] ,
--                  [ttsDomain] ,
--                  [GAPass] ,
--                  [SupportEmail] ,
--                  [DisplayTitle] ,
--                  [BillingModel] ,
--                  [Logo] ,
--                  [ContactPerson] ,
--                  [ContactPhone] ,
--                  [ContactEmail] ,
--                  [ContactFax] ,
--                  [ContactAddress] ,
--                  [TechEmail] ,
--                  [TechPhone] ,
--                  [TechName] ,
--                  [EmailPromo] ,
--                  [WebUser_Id]
--                )
--        VALUES  ( @id2Insert ,
--                  ( SELECT  CommissionModel
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  URL
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  WebBanner
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  WebFooter
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  EmailBanner
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  EmailFooter
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  ttsDomain
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  GAPass
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  SupportEmail
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  DisplayTitle
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  BillingModel
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  Logo
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  ContactPerson
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  ContactPhone
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  ContactEmail
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  ContactFax
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  ContactAddress
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  TechEmail
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  TechPhone
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  TechName
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  ( SELECT  EmailPromo
--                    FROM    TTSWebinarsSeeder.dbo.Affiliate
--                    WHERE   idUser = @id2Insert
--                  ) ,
--                  @id2Insert
--                )
--        --SET IDENTITY_INSERT dbo.Affiliate OFF
--        PRINT 'inserted Affiliate: ' + CAST(@id2Insert AS VARCHAR)
       
--        FETCH NEXT FROM my_Cursor INTO @id2Insert
--        SET IDENTITY_INSERT WebUser  OFF
--    END
--CLOSE my_Cursor
--DEALLOCATE my_Cursor
