USE BWClean
GO

 -- idWebinar - int
--          0  -- idTopic - int
--          )
/****** Object:  StoredProcedure [dbo].[MigrateWebinar]    Script Date: 1/12/2015 1:52:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

SET NOCOUNT ON 

PRINT '---------------------Starting MigrateWebinar'
DECLARE @ErrorLogID INT
DECLARE @id2Insert INT
DECLARE @QueryString VARCHAR(MAX)

DECLARE my_Cursor CURSOR
FOR
    SELECT  [idWebinarTopicXref]
    FROM    TTSWebinars2ForWesley.dbo.WebinarTopicXref
    WHERE   idWebinar IN ( 1747, 1749, 1750, 1752, 1751 ) 

OPEN my_Cursor

SET @id2Insert = 1
	
SET IDENTITY_INSERT CUWebinars.dbo.Webinar ON
	
FETCH NEXT FROM my_Cursor INTO @id2Insert

WHILE @@FETCH_STATUS = 0
    BEGIN    		
        BEGIN TRY
            SELECT  'INSERT cuwebinars.dbo.WebinarTopicXref ( idWebinar, idTopic ) VALUES  ('
                    + CAST(( SELECT idwebinar
                             FROM   TTSWebinars2ForWesley.dbo.WebinarTopicXref
                             WHERE  [idWebinarTopicXref] = @id2Insert
                           ) AS VARCHAR) + ' ,' + CAST(( SELECT idTopic
                                                         FROM   TTSWebinars2ForWesley.dbo.WebinarTopicXref
                                                         WHERE  [idWebinarTopicXref] = @id2Insert
                                                       ) AS VARCHAR) + ')'                        
        END TRY
        BEGIN CATCH

			-- Call procedure to print error information.
                --EXECUTE dbo.uspPrintError;

    -- Roll back any active or uncommittable transactions before
    -- inserting information in the ErrorLog.
            IF XACT_STATE() <> 0
                BEGIN
                    ROLLBACK TRANSACTION;
                END

            EXECUTE dbo.uspLogError @Msg = @id2Insert, @ErrorLogID = @ErrorLogID OUTPUT;

            PRINT '--==========--'
            PRINT 'Error WebinarID: ' + CAST(@id2Insert AS VARCHAR)
            PRINT '--==========--'
			
            FETCH NEXT FROM my_Cursor INTO @id2Insert
        END CATCH

        FETCH NEXT FROM my_Cursor INTO @id2Insert
        
    END
CLOSE my_Cursor
DEALLOCATE my_Cursor



SET IDENTITY_INSERT CUWebinars.dbo.Webinar OFF
--END




