USE BankWebinars
go
PRINT '--==========--'
PRINT 'BEGINS AFFILIATE IMPORTATION'
PRINT '--==========--'

DECLARE @ErrorLogID INT
--DECLARE @institutionExists INT = 0
--DECLARE @institutionId INT
DECLARE @UserID INT
--DECLARE @emailExists INT = 0
SET NOCOUNT ON;
DECLARE @id2Insert INT
DECLARE @QueryString VARCHAR(MAX)

DECLARE my_Cursor CURSOR
FOR
SELECT
        idUser
FROM    TTSWebinarsSeeder.dbo.Users
WHERE   userType = 2
        AND idUser <> 389
        AND idUser <> 12041

OPEN my_Cursor

SET @id2Insert = 1
--DECLARE @bAddress INT
--DECLARE @sAddress INT
--DECLARE @InstitutionExists INT

FETCH NEXT FROM my_Cursor INTO @id2Insert
WHILE @@FETCH_STATUS = 0
    BEGIN    		
        BEGIN TRY
            SET @queryString = 'FirstName=' + ( SELECT  firstName
                                                FROM    TTSWebinarsSeeder.dbo.Users
                                                WHERE   idUser = @id2Insert
                                              )
            SET @queryString = @queryString + '&lastname=' + ( SELECT   lastname
                                                               FROM     TTSWebinarsSeeder.dbo.Users
                                                               WHERE    idUser = @id2Insert
                                                             )
            SET @queryString = @queryString + '&email=' + ( SELECT  email
                                                            FROM    TTSWebinarsSeeder.dbo.Users
                                                            WHERE   idUser = @id2Insert
                                                          )
            SET @queryString = @queryString + '&institution=TTS'
            SET @queryString = @queryString + '&addresstype=Billing'
            SET @queryString = @queryString + '&city=' + ( SELECT   mCity
                                                           FROM     TTSWebinarsSeeder.dbo.Users
                                                           WHERE    idUser = @id2Insert
                                                         )
            SET @queryString = @queryString + '&country=US'
            SET @queryString = @queryString + '&username=' + ( SELECT   firstName + lastName
                                                               FROM     TTSWebinarsSeeder.dbo.Users
                                                               WHERE    idUser = @id2Insert
                                                             )
            SET @queryString = @queryString + '&phone=' + ( SELECT  phone1
                                                            FROM    TTSWebinarsSeeder.dbo.Users
                                                            WHERE   idUser = @id2Insert
                                                          )
            SET @queryString = @queryString + '&state=' + ( SELECT  mstate
                                                            FROM    TTSWebinarsSeeder.dbo.Users
                                                            WHERE   idUser = @id2Insert
                                                          )
            SET @queryString = @queryString + '&streetaddress=' + ( SELECT  mAddress
                                                                    FROM    TTSWebinarsSeeder.dbo.Users
                                                                    WHERE   idUser = @id2Insert
                                                                  )
            SET @queryString = @queryString + '&streetaddress2=na'    
            SET @queryString = @queryString + '&zip=55555'
			 --+ ( SELECT    mzip
    --                                                      FROM      TTSWebinarsSeeder.dbo.Users
    --                                                      WHERE     idUser = @id2Insert
    --                                                    )
            SET @queryString = @queryString + '&password=....' + ( SELECT   LOWER(lastname)
                                                                   FROM     TTSWebinarsSeeder.dbo.Users
                                                                   WHERE    idUser = @id2Insert
                                                                 )
            SET @queryString = @queryString + '&confirmPassword=....' + ( SELECT    LOWER(lastname)
                                                                          FROM      TTSWebinarsSeeder.dbo.Users
                                                                          WHERE     idUser = @id2Insert
                                                                        )
            SET @queryString = @queryString + '&usertype=1&title=na'
            SET @queryString = @queryString + '&idWebUser=' + CAST(@id2Insert AS VARCHAR)

            PRINT @QueryString
            --EXEC Migrator.dbo.[CallCreateUser] @qString = @queryString
            EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = @queryString
        END TRY
        BEGIN CATCH

			-- Call procedure to print error information.
            EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
            IF XACT_STATE() <> 0
                BEGIN
                    ROLLBACK TRANSACTION;
                END

            EXECUTE dbo.uspLogError @ErrorLogID = @ErrorLogID OUTPUT;

            PRINT '--==========--'
            PRINT 'Error PresenterID: ' + CAST(@id2Insert AS VARCHAR)
            PRINT '--==========--'
			
            FETCH NEXT FROM my_Cursor INTO @id2Insert
        END CATCH
        DECLARE @WebUser_Id INT
        SET @WebUser_Id = @id2Insert
		--( SELECT  ID
  --                          FROM    webUser
  --                          WHERE   email = ( SELECT    email
  --                                            FROM      TTSWebinarsSeeder.dbo.Users
  --                                            WHERE     idUser = @id2Insert
  --                                          )
  --                        )

        BEGIN TRY
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
            VALUES  ( @id2Insert ,
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
                      @id2Insert
                    )
		
        END TRY
        BEGIN CATCH

			-- Call procedure to print error information.
            EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
            IF XACT_STATE() <> 0
                BEGIN
                    ROLLBACK TRANSACTION;
                END

            EXECUTE dbo.uspLogError @ErrorLogID = @ErrorLogID OUTPUT;

            PRINT '--==========--'
            PRINT 'Error PresenterID: ' + CAST(@id2Insert AS VARCHAR)
            PRINT '--==========--'
        END CATCH

       
        FETCH NEXT FROM my_Cursor INTO @id2Insert
        
    END
CLOSE my_Cursor
DEALLOCATE my_Cursor
