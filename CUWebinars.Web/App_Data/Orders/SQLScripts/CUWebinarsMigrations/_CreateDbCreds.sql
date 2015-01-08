USE CUWebinars
GO


CREATE USER Dave
	FOR LOGIN Dave
	WITH DEFAULT_SCHEMA = dbo
GO

-- Add user to the database owner role
EXEC sp_addrolemember N'db_owner', N'Dave'
GO

-- run once from master anytime new server is created.
-- =================================================
-- Create User as DBO template for Windows Azure SQL Database
-- =================================================
-- For login Dave, create a user in the database
--
--CREATE LOGIN Dave WITH PASSWORD = 'n9ODDR6a'
--i had deleted dave from both instance and master db and ran the above command but got
--Msg 15025, Level 16, State 1, Procedure sp_create_login, Line 1
--The server principal 'Dave' already exists.
--ALTER USER WITH password = 'dfslk.,ds'

--CREATE USER Dave
--	FOR LOGIN Dave
--	WITH DEFAULT_SCHEMA 
--GO

---- Add user to the database owner role
--EXEC sp_addrolemember N'db_owner', N'Dave'
--GO


--first time run i get error:
--Msg 15151, Level 16, State 1, Line 11
--Cannot alter the role 'db_owner', because it does not exist or you do not have permission.

--however, the user is created with the following properties:

--USE [master]
--GO

--CREATE USER [Dave] FOR LOGIN [Dave] WITH DEFAULT_SCHEMA=[dbo]
--GO
