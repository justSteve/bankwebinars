USE Master
GO
SET NOCOUNT on
UPDATE BankWebinars.dbo.RegType SET RegTypeLabel = REPLACE(RegTypeLabel,'On-Demand', 'OnDemand')
UPDATE BankWebinars.dbo.RegType SET ShowLiveNotifications = 'Yes', ShowRecordingNotifications = 'No', ShowShippedNotifications = 'No' WHERE idRegType = 1
SET IDENTITY_INSERT BankWebinars.dbo.RegType ON 
:r "C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\Create2015OptionsGroups.SQL"
:r "C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\1hourRegTypes.SQL"
:r "C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\2hourRegTypes.SQL"
:r "C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\3PartRegTypes.SQL"
:r "C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\4PartRegTypes.SQL"
:r "C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\5PartRegTypes.SQL"

SET IDENTITY_INSERT BankWebinars.dbo.RegType Off
:r "C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\GeneratesStatementsToInsertRegTypesXRef.SQL"
:r "C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\InsertTestWebinars.sql"

SET NOCOUNT OFF

INSERT dbo.WebUser
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
VALUES  ( 31656 , -- idUser - int
          1 , -- UserType - int
          N'A' , -- AcctStatus - nvarchar(1)
          GETDATE() , -- DateCreated - datetime
          N'Test' , -- FirstName - nvarchar(50)
          N'User' , -- LastName - nvarchar(50)
          N'' , -- Initial - nvarchar(max)
          16113 , -- idUserInstitution - int
          N'testuser1@cuwebinars.com' , -- email - nvarchar(150)
          N'n' , -- futureMail - nvarchar(1)
          N'' , -- generalComments - nvarchar(1000)
          0 , -- taxExempt - bit
          0 , -- idSubscriptionDiscount - int
          2 , -- timeZone - int
          N''  -- Title - nvarchar(200)
        )

		--INSERT BankWebinars.dbo.RegTypesXref
		--        ( idRegTypeGroup, idRegType )
		--VALUES  ( 0, -- idRegTypeGroup - int
		--          0  -- idRegType - int
		--          )


--SELECT  *
--FROM    BankWebinars.dbo.RegType WHERE TaxExempt = 0 ORDER BY idRegType desc
--SELECT  *
--FROM    BankWebinars.dbo.RegTypesGroups
--SELECT DISTINCT
--        reg.*
--FROM    BankWebinars.dbo.RegType reg
--        INNER JOIN BankWebinars.dbo.RegTypesXref rx ON rx.idRegType = reg.idRegType
--        INNER JOIN BankWebinars.dbo.RegTypesGroups rGroup ON rGroup.idRegTypeGroup = rx.idRegTypeGroup
--        INNER JOIN BankWebinars.dbo.RegTypesGroupsXref groupXRef ON groupXRef.idRegTypeGroup = rGroup.idRegTypeGroup
--WHERE   groupXRef.idRegTypeGroup = 34
