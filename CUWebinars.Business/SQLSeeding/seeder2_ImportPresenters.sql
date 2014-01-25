USE BankWebinars
go
--SELECT 'UPDATE dbo.Presenter SET Biography = BiographyLong, BiographyLong = |<p><img class="alignleft" src="https://ttseast.blob.core.windows.net/images'+(select REPLACE(PhotoFull, 'content/images/','') from Presenter where idUser = p.idUser)+' alt="Photo of '+(SELECT FirstName + ' '+ LastName FROM dbo.WebUser WHERE idUser = p.idUser)+'" />$'+(SELECT BiographyLong FROM dbo.Presenter WHERE idUser = p.idUser)+'| where idUser = '+ CAST(idUser AS VARCHAR)

DELETE BankWebinars.dbo.WebUser
DELETE BankWebinars.dbo.Affiliate
DELETE BankWebinars.dbo.Institution
DELETE BankWebinars.dbo.Addresses
DELETE BankWebinars.dbo.Presenter

SET NOCOUNT ON
ALTER TABLE presenter

    ALTER COLUMN Biography
     varchar(max)
PRINT '--==========--'
PRINT '_________________________________________________________________________BEGINS PRESENTER IMPORTATION'
PRINT '--==========--'

UPDATE  TTSWebinarsSeeder.dbo.users
SET     mAddress = 'add'
WHERE   TTSWebinarsSeeder.dbo.users.maddress IS NULL

UPDATE  TTSWebinarsSeeder.dbo.users
SET     mCity = 'city'
WHERE   TTSWebinarsSeeder.dbo.users.mCity IS NULL

UPDATE  TTSWebinarsSeeder.dbo.users
SET     mState = 'st'
WHERE   TTSWebinarsSeeder.dbo.users.mState IS NULL

UPDATE  TTSWebinarsSeeder.dbo.users
SET     mZip = 'zip'
WHERE   TTSWebinarsSeeder.dbo.users.mZip IS NULL

UPDATE  TTSWebinarsSeeder.dbo.users
SET     mAddress = 'add'
WHERE   TTSWebinarsSeeder.dbo.users.maddress = ''

UPDATE  TTSWebinarsSeeder.dbo.users
SET     mCity = 'city'
WHERE   TTSWebinarsSeeder.dbo.users.mCity = ''

UPDATE  TTSWebinarsSeeder.dbo.users
SET     mState = 'st'
WHERE   TTSWebinarsSeeder.dbo.users.mState = ''

UPDATE  TTSWebinarsSeeder.dbo.users
SET     mZip = 'zip'
WHERE   TTSWebinarsSeeder.dbo.users.mZip = ''
UPDATE  TTSWebinarsSeeder.dbo.users
SET     phone1 = '555-555-5555'
WHERE   TTSWebinarsSeeder.dbo.users.phone1 = ''
        OR TTSWebinarsSeeder.dbo.users.phone1 IS NULL

UPDATE  TTSWebinarsSeeder.dbo.users
SET     firstName = ' '
WHERE   TTSWebinarsSeeder.dbo.users.firstName IS NULL

UPDATE  TTSWebinarsSeeder.dbo.users
SET     firstname = REPLACE(firstname, '&', '')
WHERE   email LIKE 'holder@ttstrain.com'
        OR email = email
UPDATE  TTSWebinarsSeeder.dbo.users
SET     lastName = REPLACE(lastName, '&', '')
WHERE   email LIKE 'holder@ttstrain.com'
        OR email = email

UPDATE  TTSWebinarsSeeder.dbo.users
SET     firstname = REPLACE(firstname, ',', '')
WHERE   email LIKE 'holder@ttstrain.com'
        OR email = email

UPDATE  TTSWebinarsSeeder.dbo.users
SET     lastName = REPLACE(lastName, ',', '')
WHERE   email LIKE 'holder@ttstrain.com'
        OR email = email

UPDATE  TTSWebinarsSeeder.dbo.users
SET     provisionalInstitution = 'TTS' ,
        idUserInstitution = 3549
WHERE   TTSWebinarsSeeder.dbo.users.idUser = 19

UPDATE  TTSWebinarsSeeder.dbo.users
SET     email = REPLACE(firstname, ' ', '') + '_' + REPLACE(LASTname, ' ', '') + '@ttstrain.com'
WHERE   email LIKE 'holder@ttstrain.com'
        OR email = email

UPDATE  TTSWebinarsSeeder.dbo.users
SET     email = REPLACE(firstname, ' ', '') + '_' + REPLACE(LASTname, 'Gutiérrez', 'Gutierrez') + '@ttstrain.com'
WHERE   email LIKE 'holder@ttstrain.com'
        OR email = email


UPDATE  TTSWebinarsSeeder.dbo.users
SET     email = 'David_McGuinn@ttstrain.com'
WHERE   idUser = 10567
SELECT  *
FROM    TTSWebinarsSeeder.dbo.users
WHERE   idUser = 10567
--DELETE FROM users WHERE idUser = 

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
SELECT idUser 
FROM    TTSWebinarsSeeder.dbo.Users
WHERE   userType = 3
        --AND idUser IN (
        --SELECT  idUser
        --FROM    TTSWebinarsSeeder.dbo.Presenter
        --WHERE   idUser IN (
        --        SELECT  idPresenter
        --        FROM    TTSWebinarsSeeder.dbo.webinar
        --        WHERE   idWebinar IN ( 1472, 1471, 1412, 1435, 1470, 1268, 1437, 1285, 1438, 1432, 1433, 1426, 1422, 1416, 1410, 1430, 1358, 1326, 1411, 1415,
        --                               1420, 1436 ) 
								--	   ) )

OPEN my_Cursor

SET @id2Insert = 1
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
            INSERT  INTO BankWebinars.[dbo].[Presenter]
                    ( [Biography] ,
                      [BiographyLong] ,
                      [photoFull] ,
                      [photoThumb] ,
                      idUser
                    )
            VALUES  ( ( SELECT  biographyLong
                        FROM    TTSWebinarsSeeder.dbo.Presenter
                        WHERE   idUser = @id2Insert
                      ) ,
                      ( SELECT  biographyLong
                        FROM    TTSWebinarsSeeder.dbo.Presenter
                        WHERE   idUser = @id2Insert
                      ) ,
                      ( SELECT  photoFull
                        FROM    TTSWebinarsSeeder.dbo.Presenter
                        WHERE   idUser = @id2Insert
                      ) ,
                      ( SELECT  photoThumb
                        FROM    TTSWebinarsSeeder.dbo.Presenter
                        WHERE   idUser = @id2Insert
                      ) ,
                      @WebUser_Id
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
