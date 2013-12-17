
USE TTSWebinars
go



--The following sproc will call the WebAPI controller (replaces the PowerShell script FROM proir versions)
--
--Note that I'm setting it in a different db altogether - the Ole call would not be compatible with Azure
--
--following needs to execute on whichever db you build it into.
--sp_configure 'show advanced options', 1;
--GO
--RECONFIGURE;
--GO
--sp_configure 'Ole Automation Procedures', 1;
--GO
--RECONFIGURE;


--CREATE PROCEDURE CallCreateUser 
--	-- Add the parameters for the stored procedure here
--	@qString  varchar(max), 
--	@p2 int = 0
--AS
--BEGIN
--	-- SET NOCOUNT ON added to prevent extra result sets from
--	-- interfering with SELECT statements.
--	SET NOCOUNT ON;

--	DECLARE @obj INT
--	DECLARE @val INT
--	DECLARE @sUrl VARCHAR(MAX)
--	DECLARE @response VARCHAR(MAX)
--	DECLARE @hr INT
--	DECLARE @src VARCHAR(MAX)
--	DECLARE @desc VARCHAR(MAX)

--	SET @surl = 'http://localhost:21746/api/acctapi?'+@qString 
	
--	EXEC sp_OAcreate 'MSXML2.ServerXMLHttp', @obj out
--	EXEC sp_OAMethod @obj, 'open', NULL, 'GET', @sUrl, false
--	EXEC sp_OAMethod @obj, 'send'
--	EXEC sp_OAGetProperty @obj, 'responseText', @response OUT

--	SELECT @response [response]
--	EXEC sp_OADestroy @obj




--END
--GO


EXEC sp_MSforeachtable @command1="ALTER TABLE ? CHECK CONSTRAINT ALL"