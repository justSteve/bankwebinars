USE BankWebinars
go
EXEC sp_MSforeachtable @command1 = "ALTER TABLE ? NOCHECK CONSTRAINT ALL"

--EXEC sp_MSforeachtable @command1 = "DELETE  ? "

go
 
USE MembershipReboot
 go

EXEC sp_MSforeachtable @command1 = "ALTER TABLE ? NOCHECK CONSTRAINT ALL"

--EXEC sp_MSforeachtable @command1 = "DELETE  ? "
go

--USE MembershipReboot
--GO
--delete [dbo].UserClaims
--DELETE [dbo].[UserAccounts]

