USE MembershipReboot 
GO
SELECT * FROM dbo.UserClaims WHERE ParentKey = (
SELECT [Key] FROM dbo.UserAccounts WHERE Email = 'affiliate@ttstrain.com'
)

USE MembershipReboot 
GO
SELECT * FROM dbo.UserClaims WHERE ParentKey = (
SELECT [Key] FROM dbo.UserAccounts WHERE Email = 'affiliate@ttstrain.com'
)

