USE TTSWebinars
go
EXEC sp_MSforeachtable @command1 = "ALTER TABLE ? NOCHECK CONSTRAINT ALL"

EXEC sp_MSforeachtable @command1 = "DELETE  ? "
 USE MembershipReboot
 go

EXEC sp_MSforeachtable @command1 = "ALTER TABLE ? NOCHECK CONSTRAINT ALL"

EXEC sp_MSforeachtable @command1 = "DELETE  ? "


USE TTSWebinars
go