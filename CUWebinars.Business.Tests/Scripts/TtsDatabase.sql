USE [master]
GO
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'TTSDatabase')
DROP DATABASE [TTSDatabase];

CREATE DATABASE [TTSDatabase]
ON  (NAME = N'TTSDatabase_Data', FILENAME = N'E:\csv\TTSDatabase.mdf')
GO
USE [TTSDatabase]
GO
ALTER DATABASE [TTSDatabase] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TTSDatabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TTSDatabase] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TTSDatabase] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TTSDatabase] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TTSDatabase] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TTSDatabase] SET ARITHABORT OFF 
GO
ALTER DATABASE [TTSDatabase] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [TTSDatabase] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [TTSDatabase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TTSDatabase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TTSDatabase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TTSDatabase] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TTSDatabase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TTSDatabase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TTSDatabase] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TTSDatabase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TTSDatabase] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TTSDatabase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TTSDatabase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TTSDatabase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TTSDatabase] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [TTSDatabase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TTSDatabase] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [TTSDatabase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TTSDatabase] SET RECOVERY FULL 
GO
ALTER DATABASE [TTSDatabase] SET  MULTI_USER 
GO
ALTER DATABASE [TTSDatabase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TTSDatabase] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TTSDatabase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TTSDatabase] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [TTSDatabase]
GO
/****** Object:  User [TTSOp]    Script Date: 27/10/2014 11:27:27 AM ******/
CREATE USER [TTSOp] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [logger]    Script Date: 27/10/2014 11:27:27 AM ******/
CREATE USER [logger] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[logging]
GO
/****** Object:  Schema [logging]    Script Date: 27/10/2014 11:27:27 AM ******/
CREATE SCHEMA [logging]
GO
/****** Object:  Schema [TTS]    Script Date: 27/10/2014 11:27:27 AM ******/
CREATE SCHEMA [TTS]
GO
/****** Object:  Schema [TTSOrders]    Script Date: 27/10/2014 11:27:27 AM ******/
CREATE SCHEMA [TTSOrders]
GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]
(
@Application NVARCHAR(60),
@PageIndex INT = 0,
@PageSize INT = 15,
@TotalCount INT OUTPUT
)
AS 

SET NOCOUNT ON

DECLARE @FirstTimeUTC DATETIME
DECLARE @FirstSequence INT
DECLARE @StartRow INT
DECLARE @StartRowIndex INT

SELECT 
    @TotalCount = COUNT(1) 
FROM 
    [ELMAH_Error]
WHERE 
    [Application] = @Application

-- Get the ID of the first error for the requested page

SET @StartRowIndex = @PageIndex * @PageSize + 1

IF @StartRowIndex <= @TotalCount
BEGIN

    SET ROWCOUNT @StartRowIndex

    SELECT  
        @FirstTimeUTC = [TimeUtc],
        @FirstSequence = [Sequence]
    FROM 
        [ELMAH_Error]
    WHERE   
        [Application] = @Application
    ORDER BY 
        [TimeUtc] DESC, 
        [Sequence] DESC

END
ELSE
BEGIN

    SET @PageSize = 0

END

-- Now set the row count to the requested page size and get
-- all records below it for the pertaining application.

SET ROWCOUNT @PageSize

SELECT 
    errorId     = [ErrorId], 
    application = [Application],
    host        = [Host], 
    type        = [Type],
    source      = [Source],
    message     = [Message],
    [user]      = [User],
    statusCode  = [StatusCode], 
    time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
FROM 
    [ELMAH_Error] error
WHERE
    [Application] = @Application
AND
    [TimeUtc] <= @FirstTimeUTC
AND 
    [Sequence] <= @FirstSequence
ORDER BY
    [TimeUtc] DESC, 
    [Sequence] DESC
FOR
    XML AUTO

GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[ELMAH_GetErrorXml]
(
@Application NVARCHAR(60),
@ErrorId UNIQUEIDENTIFIER
)
AS

SET NOCOUNT ON

SELECT 
    [AllXml]
FROM 
    [ELMAH_Error]
WHERE
    [ErrorId] = @ErrorId
AND
    [Application] = @Application

GO
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
  @ErrorId UNIQUEIDENTIFIER ,
  @Application NVARCHAR(60) ,
  @Host NVARCHAR(30) ,
  @Type NVARCHAR(100) ,
  @Source NVARCHAR(60) ,
  @Message NVARCHAR(500) ,
  @User NVARCHAR(50) ,
  @AllXml NVARCHAR(MAX) ,
  @StatusCode INT ,
  @TimeUtc DATETIME
)
AS
SET NOCOUNT ON
IF ( @Type LIKE '%Logging%' )
    BEGIN
        INSERT  INTO TTS_Events
                ( ErrorId ,
                  Application ,
                  Host ,
                  Type ,
                  Source ,
                  Message ,
                  [User] ,
                  AllXml ,
                  StatusCode ,
                  TimeUtc
                )
        VALUES  ( @ErrorId ,
                  @Application ,
                  @Host ,
                  @Type ,
                  @Source ,
                  @Message ,
                  @User ,
                  @AllXml ,
                  @StatusCode ,
                  @TimeUtc
                )

        RETURN
    END
IF @message NOT LIKE '%/favicon.ico%'
    BEGIN
        INSERT  INTO ELMAH_Error
                ( ErrorId ,
                  Application ,
                  Host ,
                  Type ,
                  Source ,
                  Message ,
                  [User] ,
                  AllXml ,
                  StatusCode ,
                  TimeUtc 
                  --RealXML
                )
        VALUES  ( @ErrorId ,
                  @Application ,
                  @Host ,
                  @Type ,
                  @Source ,
                  @Message ,
                  @User ,
                  @AllXml ,
                  @StatusCode ,
                  @TimeUtc
                )

    END

GO
/****** Object:  StoredProcedure [dbo].[uspLogError]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- uspLogError logs error information in the ErrorLog table about the 
-- error that caused execution to jump to the CATCH block of a 
-- TRY...CATCH construct. This should be executed from within the scope 
-- of a CATCH block otherwise it will return without inserting error 
-- information. 
CREATE PROCEDURE [dbo].[uspLogError]
    @ErrorLogID [int] = 0 OUTPUT -- contains the ErrorLogID of the row inserted
AS -- by uspLogError in the ErrorLog table
    BEGIN
        SET NOCOUNT ON;

    -- Output parameter value of 0 indicates that error 
    -- information was not logged
        SET @ErrorLogID = 0;

        BEGIN TRY
        -- Return if there is no error information to log
            IF ERROR_NUMBER() IS NULL 
                RETURN;

        -- Return if inside an uncommittable transaction.
        -- Data insertion/modification is not allowed when 
        -- a transaction is in an uncommittable state.
            IF XACT_STATE() = -1 
                BEGIN
                    PRINT 'Cannot log error since the current transaction is in an uncommittable state. '
                        + 'Rollback the transaction before executing uspLogError in order to successfully log error information.';
                    RETURN;
                END

            INSERT  [dbo].[ErrorLog]
                    ( [UserName] ,
                      [ErrorNumber] ,
                      [ErrorSeverity] ,
                      [ErrorState] ,
                      [ErrorProcedure] ,
                      [ErrorLine] ,
                      [ErrorMessage]
                    )
            VALUES  ( CONVERT(SYSNAME, CURRENT_USER) ,
                      ERROR_NUMBER() ,
                      ERROR_SEVERITY() ,
                      ERROR_STATE() ,
                      ERROR_PROCEDURE() ,
                      ERROR_LINE() ,
                      ERROR_MESSAGE()
                    );

        -- Pass back the ErrorLogID of the row inserted
            SET @ErrorLogID = @@IDENTITY;
        END TRY
        BEGIN CATCH
            PRINT 'An error occurred in stored procedure uspLogError: ';
            EXECUTE [dbo].[uspPrintError];
            RETURN -1;
        END CATCH
    END;

GO
/****** Object:  StoredProcedure [dbo].[uspPrintError]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- uspPrintError prints error information about the error that caused 
-- execution to jump to the CATCH block of a TRY...CATCH construct. 
-- Should be executed from within the scope of a CATCH block otherwise 
-- it will return without printing any error information.
CREATE PROCEDURE [dbo].[uspPrintError]
AS 
    BEGIN
        SET NOCOUNT ON;

    -- Print error information. 
        PRINT 'Error ' + CONVERT(VARCHAR(50), ERROR_NUMBER()) + ', Severity '
            + CONVERT(VARCHAR(5), ERROR_SEVERITY()) + ', State '
            + CONVERT(VARCHAR(5), ERROR_STATE()) + ', Procedure '
            + ISNULL(ERROR_PROCEDURE(), '-') + ', Line '
            + CONVERT(VARCHAR(5), ERROR_LINE());
        PRINT ERROR_MESSAGE();
    END;

GO
/****** Object:  StoredProcedure [logging].[ELMAH_GetErrorsXml]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [logging].[ELMAH_GetErrorsXml]
(
@Application NVARCHAR(60),
@PageIndex INT = 0,
@PageSize INT = 15,
@TotalCount INT OUTPUT
)
AS 

SET NOCOUNT ON

DECLARE @FirstTimeUTC DATETIME
DECLARE @FirstSequence INT
DECLARE @StartRow INT
DECLARE @StartRowIndex INT

SELECT 
    @TotalCount = COUNT(1) 
FROM 
    [ELMAH_Error]
WHERE 
    [Application] = @Application

-- Get the ID of the first error for the requested page

SET @StartRowIndex = @PageIndex * @PageSize + 1

IF @StartRowIndex <= @TotalCount
BEGIN

    SET ROWCOUNT @StartRowIndex

    SELECT  
        @FirstTimeUTC = [TimeUtc],
        @FirstSequence = [Sequence]
    FROM 
        [ELMAH_Error]
    WHERE   
        [Application] = @Application
    ORDER BY 
        [TimeUtc] DESC, 
        [Sequence] DESC

END
ELSE
BEGIN

    SET @PageSize = 0

END

-- Now set the row count to the requested page size and get
-- all records below it for the pertaining application.

SET ROWCOUNT @PageSize

SELECT 
    errorId     = [ErrorId], 
    application = [Application],
    host        = [Host], 
    type        = [Type],
    source      = [Source],
    message     = [Message],
    [user]      = [User],
    statusCode  = [StatusCode], 
    time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
FROM 
    [ELMAH_Error] error
WHERE
    [Application] = @Application
AND
    [TimeUtc] <= @FirstTimeUTC
AND 
    [Sequence] <= @FirstSequence
ORDER BY
    [TimeUtc] DESC, 
    [Sequence] DESC
FOR
    XML AUTO

GO
/****** Object:  StoredProcedure [logging].[ELMAH_GetErrorXml]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [logging].[ELMAH_GetErrorXml]
(
@Application NVARCHAR(60),
@ErrorId UNIQUEIDENTIFIER
)
AS

SET NOCOUNT ON

SELECT 
    [AllXml]
FROM 
    [ELMAH_Error]
WHERE
    [ErrorId] = @ErrorId
AND
    [Application] = @Application

GO
/****** Object:  StoredProcedure [logging].[ELMAH_LogError]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [logging].[ELMAH_LogError]
(
@ErrorId UNIQUEIDENTIFIER,
@Application NVARCHAR(60),
@Host NVARCHAR(30),
@Type NVARCHAR(100),
@Source NVARCHAR(60),
@Message NVARCHAR(500),
@User NVARCHAR(50),
@AllXml NVARCHAR(MAX),
@StatusCode INT,
@TimeUtc DATETIME
)
AS

SET NOCOUNT ON

INSERT
INTO
    [ELMAH_Error]
    (
        [ErrorId],
        [Application],
        [Host],
        [Type],
        [Source],
        [Message],
        [User],
        [AllXml],
        [StatusCode],
        [TimeUtc]
    )
VALUES
    (
        @ErrorId,
        @Application,
        @Host,
        @Type,
        @Source,
        @Message,
        @User,
        @AllXml,
        @StatusCode,
        @TimeUtc
    )

GO
/****** Object:  Table [dbo].[BuildVersion]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BuildVersion](
	[SystemInformationID] [tinyint] IDENTITY(1,1) NOT NULL,
	[Database Version] [nvarchar](25) NOT NULL,
	[VersionDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_BuildVersion] PRIMARY KEY CLUSTERED 
(
	[SystemInformationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CULog]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CULog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NULL,
	[Level] [nvarchar](50) NULL,
	[Message] [nvarchar](4000) NULL,
	[Exception] [nvarchar](4000) NULL,
	[Logger] [varchar](4000) NULL,
 CONSTRAINT [PK_sample_table] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY CLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[ErrorLogID] [int] IDENTITY(1,1) NOT NULL,
	[ErrorTime] [datetime] NOT NULL,
	[UserName] [sysname] NOT NULL,
	[ErrorNumber] [int] NOT NULL,
	[ErrorSeverity] [int] NULL,
	[ErrorState] [int] NULL,
	[ErrorProcedure] [nvarchar](126) NULL,
	[ErrorLine] [int] NULL,
	[ErrorMessage] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_ErrorLog_ErrorLogID] PRIMARY KEY CLUSTERED 
(
	[ErrorLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ShowAllZIPCodes]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowAllZIPCodes](
	[ZIPCode] [nvarchar](14) NOT NULL,
	[City] [nvarchar](28) NULL,
	[StateAbbreviation] [nvarchar](2) NULL,
 CONSTRAINT [pk_ZipCode] PRIMARY KEY CLUSTERED 
(
	[ZIPCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[States]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[States](
	[StateCode] [nvarchar](14) NOT NULL,
	[StateAbbreviation] [nvarchar](2) NULL,
	[StateName] [nvarchar](15) NULL,
 CONSTRAINT [pk_StateCode] PRIMARY KEY CLUSTERED 
(
	[StateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TimeZoneByZip]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TimeZoneByZip](
	[zip] [varchar](50) NULL,
	[city] [varchar](50) NULL,
	[state] [varchar](50) NULL,
	[latitude] [varchar](50) NULL,
	[longitude] [varchar](50) NULL,
	[timezone] [varchar](50) NULL,
	[dst] [varchar](50) NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_TimeZoneByZip] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TTS_Events]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TTS_Events](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [nvarchar](max) NOT NULL,
	[RealXml] [xml] NOT NULL,
 CONSTRAINT [PK_TTS_Events] PRIMARY KEY CLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ZipCodes]    Script Date: 27/10/2014 11:27:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZipCodes](
	[ZIPCode] [int] NOT NULL,
	[Latitude] [nvarchar](10) NULL,
	[Longitude] [nvarchar](10) NULL,
	[Class] [nvarchar](1) NULL,
	[City] [nvarchar](28) NULL,
	[StateCode] [nvarchar](2) NULL,
 CONSTRAINT [pk_ZipCode1] PRIMARY KEY CLUSTERED 
(
	[ZIPCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET IDENTITY_INSERT [dbo].[TimeZoneByZip] ON 
INSERT [dbo].[TimeZoneByZip] ([zip], [city], [state], [latitude], [longitude], [timezone], [dst], [id]) VALUES (N'00210', N'Portsmouth', N'NH', N'43.005895', N'-71.013202', N'-5', N'1', 1)
SET IDENTITY_INSERT [dbo].[TimeZoneByZip] OFF
GO
ALTER TABLE [dbo].[BuildVersion] ADD  CONSTRAINT [DF_BuildVersion_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
GO
ALTER TABLE [dbo].[TTS_Events] ADD  CONSTRAINT [DF_TTS_Events_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
GO
USE [master]
GO
ALTER DATABASE [TTSDatabase] SET  READ_WRITE 
GO
