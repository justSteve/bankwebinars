--DELETE FROM dbo.UserClaims WHERE [Key] = (SELECT [Key] FROM dbo.UserAccounts WHERE Email = 'steve@juststeve.com')
--DELETE FROM dbo.UserAccounts WHERE	 Email = 'steve@juststeve.com'
SELECT * FROM dbo.UserAccounts WHERE Tenant = 'bankwebinars'
SELECT * FROM dbo.UserClaims WHERE [Key] IN (SELECT [Key] FROM dbo.UserAccounts WHERE Tenant = 'bankwebinars')

SELECT * FROM dbo.UserClaims WHERE KEY NOT IN (SELECT [Key] FROM dbo.UserAccounts)
SELECT * FROM dbo.UserClaims WHERE [Key] not IN (SELECT [Key] FROM dbo.UserAccounts)