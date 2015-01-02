SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
alter Procedure [dbo].[MigrateInstitution]
AS
BEGIN

    PRINT '---------------------Starting MigrateInstitution'
    SET NOCOUNT ON;
    SET IDENTITY_INSERT BankWebinars.[dbo].[Institution] ON 

    DECLARE @ErrorLogID INT
    DECLARE @id2Insert INT
    DECLARE @QueryString VARCHAR(MAX)

    DECLARE my_Cursor CURSOR
    FOR
    SELECT  idInstitution
    FROM    BWClean.dbo.Institution

    OPEN my_Cursor

    SET @id2Insert = 1
	
    FETCH NEXT FROM my_Cursor INTO @id2Insert
    WHILE @@FETCH_STATUS = 0
        BEGIN    		
            BEGIN TRY
                INSERT  BankWebinars.dbo.Institution
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
                VALUES  ( @id2Insert ,
                          ( SELECT  InstitutionName
                            FROM    BWClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  InstitutionType
                            FROM    BWClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  domainName
                            FROM    BWClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  RegIdentifier
                            FROM    BWClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  Address
                            FROM    BWClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  City
                            FROM    BWClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  State
                            FROM    BWClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          ) ,
                          ( SELECT  Zip
                            FROM    BWClean.dbo.Institution
                            WHERE   idInstitution = @id2Insert
                          )
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
                PRINT 'Error InstitutionID: ' + CAST(@id2Insert AS VARCHAR)
                PRINT '--==========--'
			
                FETCH NEXT FROM my_Cursor INTO @id2Insert
            END CATCH

            FETCH NEXT FROM my_Cursor INTO @id2Insert
        
        END
    CLOSE my_Cursor
    DEALLOCATE my_Cursor

SET IDENTITY_INSERT BankWebinars.[dbo].[Institution] off
END



GO
