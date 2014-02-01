ALTER DATABASE [TTSWebinars2_testing] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
GO


RESTORE DATABASE [TTSWebinars2_testing] FROM  DISK = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\backup\TTSWebinars2_testing.BAK'
 WITH  FILE = 1,  MOVE N'TTSWebinars' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\TTSWebinars2_testing.mdf',  
 MOVE N'TTSWebinars_log' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\TTSWebinars2_testing.ldf',  NOUNLOAD,  REPLACE,  STATS = 5

--RESTORE DATABASE [TTSWebinars2_testing] FROM  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\Backup\Daily\TTSWebinars2\mostRecent.BAK'
-- WITH  FILE = 1,  MOVE N'TTSWebinars' TO N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\TTSWebinars2_testing.mdf',  
-- MOVE N'TTSWebinars_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\TTSWebinars2_testing_1.ldf',  NOUNLOAD,  REPLACE,  STATS = 5

 ALTER DATABASE [TTSWebinars2_testing] SET MULTI_USER WITH ROLLBACK IMMEDIATE
GO

--GO
--/****** Object:  Database [TTSWebinarsSeeder]    Script Date: 11/23/2013 1:57:49 PM ******/
--IF NOT EXISTS ( SELECT  name
--                FROM    sys.databases
--                WHERE   name = N'TTSWebinarsSeeder' )
--    BEGIN
--        CREATE DATABASE [TTSWebinarsSeeder] ON PRIMARY 
--( NAME = N'TTSWebinarsSeeder', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\TTSWebinarsSeeder.mdf' , SIZE = 7168KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
--            LOG ON 
--( NAME = N'TTSWebinarsSeeder_log', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\TTSWebinarsSeeder_log.ldf' , SIZE = 6272KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
--    END

--GO
USE TTSWebinarsSeeder
go

ALTER DATABASE [TTSWebinarsSeeder] SET COMPATIBILITY_LEVEL = 110
GO
IF ( 1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled') )
    BEGIN
        EXEC [TTSWebinarsSeeder].[dbo].[sp_fulltext_database] @action = 'enable'
    END
GO
ALTER DATABASE [TTSWebinarsSeeder] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET ARITHABORT OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET RECOVERY FULL 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET  MULTI_USER 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TTSWebinarsSeeder] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TTSWebinarsSeeder] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF
 ) 
GO

EXEC sys.sp_db_vardecimal_storage_format N'TTSWebinarsSeeder', N'ON'

GO
/****** Object:  StoredProcedure [dbo].[CallCreateUser]    Script Date: 1/21/2014 7:20:31 PM ******/

DROP PROCEDURE [dbo].[CallCreateUser] 
GO

-- =============================================
-- Author:		sjh
-- Create date: 12/13
-- Description:	Seeds CUWebinarsDev
-- =============================================
CREATE PROCEDURE [dbo].[CallCreateUser] 
	-- Add the parameters for the stored procedure here
	@qString VARCHAR(max)
AS
BEGIN
DECLARE	@url VARCHAR(3000),
	@win INT,
	@hr INT,
	@Text VARCHAR(3000)
	-- SET NOINTNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


		SET	@url = 'http://localhost:5555/account/get/?' + @qString
		SET	@url = @url + '&nocache=' + CAST(GETDATE() AS VARCHAR)

	
		EXEC @hr = sp_OACreate 'WinHttp.WinHttpRequest.5.1', @win OUT
		IF @hr <> 0 EXEC sp_OAGetErrorInfo @win

		EXEC @hr = sp_OAMethod @win, 'Open', NULL, 'GET', @url, 'false'
		IF @hr <> 0 EXEC sp_OAGetErrorInfo @win

		EXEC @hr = sp_OAMethod @win, 'Send'
		IF @hr <> 0 EXEC sp_OAGetErrorInfo @win

		EXEC @hr = sp_OAGetProperty @win, 'ResponseText', @Text OUT
		IF @hr <> 0 EXEC sp_OAGetErrorInfo @win

		EXEC @hr = sp_OADestroy @win 
		IF @hr <> 0 EXEC sp_OAGetErrorInfo @win 

    -- Insert statements for procedure here
	SELECT @qString
END

GO



GO
/****** Object:  Table [dbo].[WebinarTopicXref]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[WebinarTopicXref]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[WebinarTopicXref]
GO
/****** Object:  Table [dbo].[WebinarTopicXref]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Topic]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].Topic
GO
/****** Object:  Table [dbo].[WebinarFile]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[WebinarFile]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[WebinarFile]
GO
/****** Object:  Table [dbo].[Webinar]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Webinar]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[Webinar]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Users]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[Users]
GO
/****** Object:  Table [dbo].[Presenter]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Presenter]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[Presenter]
GO
/****** Object:  Table [dbo].[OptionsXref]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[OptionsXref]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[OptionsXref]
GO
/****** Object:  Table [dbo].[OptionsGroupsXref]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[OptionsGroupsXref]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[OptionsGroupsXref]
GO
/****** Object:  Table [dbo].[Options]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Options]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[Options]
GO
/****** Object:  Table [dbo].[Institution]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Institution]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[Institution]
GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[ErrorLog]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[ErrorLog]
GO
/****** Object:  Table [dbo].[Affiliate]    Script Date: 11/23/2013 1:57:49 PM ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Affiliate]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[Affiliate]

--/****** Object:  Database [TTSWebinarsSeeder]    Script Date: 11/23/2013 1:57:49 PM ******/
--IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'TTSWebinarsSeeder')
--DROP DATABASE [TTSWebinarsSeeder]
/****** Object:  Table [dbo].[Affiliate]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[Affiliate]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[Affiliate]
        (
          [idUser] [int] NOT NULL ,
          [commissionModel] [tinyint] NOT NULL ,
          [URL] [varchar](500) NULL ,
          [webBanner] [varchar](4000) NOT NULL ,
          [webFooter] [varchar](4000) NOT NULL ,
          [emailBanner] [varchar](4000) NOT NULL ,
          [emailFooter] [varchar](4000) NOT NULL ,
          [ttsDomain] [varchar](50) NULL ,
          [GAPass] [varchar](50) NULL ,
          [supportEmail] [varchar](75) NULL ,
          [DisplayTitle] [varchar](150) NULL ,
          [BillingModel] [varchar](150) NULL ,
          [Logo] [varchar](150) NULL ,
          [ContactPerson] [varchar](150) NULL ,
          [ContactPhone] [varchar](150) NULL ,
          [ContactEmail] [varchar](150) NULL ,
          [ContactFax] [varchar](150) NULL ,
          [ContactAddress] [varchar](150) NULL ,
          [TechEmail] [varchar](150) NULL ,
          [TechPhone] [varchar](150) NULL ,
          [TechName] [nvarchar](150) NULL ,
          [EmailPromo] [nvarchar](50) NULL ,
          CONSTRAINT [PK_Affiliate] PRIMARY KEY CLUSTERED ( [idUser] ASC )
        )
            ON
        [PRIMARY]
    END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[ELMAH_Error]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[ELMAH_Error]
        (
          [ErrorId] [uniqueidentifier] NOT NULL ,
          [Application] [nvarchar](60) NOT NULL ,
          [Host] [nvarchar](50) NOT NULL ,
          [Type] [nvarchar](100) NOT NULL ,
          [Source] [nvarchar](60) NOT NULL ,
          [Message] [nvarchar](500) NOT NULL ,
          [User] [nvarchar](50) NOT NULL ,
          [StatusCode] [int] NOT NULL ,
          [TimeUtc] [datetime] NOT NULL ,
          [Sequence] [int] IDENTITY(1, 1)
                           NOT NULL ,
          [AllXml] [nvarchar](MAX) NOT NULL ,
          CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY CLUSTERED ( [ErrorId] ASC )
        )
            ON
        [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    END
GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[ErrorLog]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[ErrorLog]
        (
          [ErrorLogID] [int] IDENTITY(1, 1)
                             NOT NULL ,
          [ErrorTime] [datetime] NOT NULL ,
          [UserName] [sysname] NOT NULL ,
          [ErrorNumber] [int] NOT NULL ,
          [ErrorSeverity] [int] NULL ,
          [ErrorState] [int] NULL ,
          [ErrorProcedure] [nvarchar](126) NULL ,
          [ErrorLine] [int] NULL ,
          [ErrorMessage] [nvarchar](4000) NOT NULL ,
          CONSTRAINT [PK_ErrorLog_ErrorLogID] PRIMARY KEY CLUSTERED ( [ErrorLogID] ASC )
        )
            ON
        [PRIMARY]
    END
GO
/****** Object:  Table [dbo].[Institution]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[Institution]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[Institution]
        (
          [idInstitution] [int] NOT NULL ,
          [name] [varchar](250) NOT NULL ,
          [idPrimaryContact] [int] NULL ,
          [zip] [varchar](20) NOT NULL ,
          [domainName] [varchar](75) NULL ,
          [cert] [nvarchar](40) NULL ,
          CONSTRAINT [PK_Institution] PRIMARY KEY CLUSTERED ( [idInstitution] ASC )
        )
            ON
        [PRIMARY]
    END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Options]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[Options]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[Options]
        (
          [idOption] [int] NOT NULL ,
          [optionExplain] [nvarchar](MAX) NULL ,
          [optionLabel] [nvarchar](150) NULL ,
          [priceToAdd] [float] NULL ,
          [SKU] [nvarchar](150) NULL ,
          [percToAdd] [float] NULL ,
          [sortOrder] [int] NULL ,
          [type] [nvarchar](32) NOT NULL ,
          [ShowLiveNotifications] [nvarchar](MAX) NULL ,
          [ShowRecordingNotifications] [nvarchar](MAX) NULL ,
          [ShowShippedNotifications] [nvarchar](MAX) NULL ,
          [Stage1CheckoutConfirmationMsg] [nvarchar](MAX) NULL ,
          [Stage2CheckoutConfirmationMsg] [nvarchar](MAX) NULL ,
          [Stage1EmailConfirmationMsg] [nvarchar](MAX) NULL ,
          [Stage2EmailConfirmationMsg] [nvarchar](MAX) NULL ,
          [taxExempt] [nchar](10) NULL ,
          [msgConfirm] [nchar](10) NULL ,
          [MigrationXRef] [int] NULL ,
          CONSTRAINT [PK_Options] PRIMARY KEY CLUSTERED ( [idOption] ASC )
        )
            ON
        [PRIMARY] 
    END
GO


/****** Object:  Table [dbo].[OptionsGroups]    Script Date: 1/23/2014 2:11:16 PM ******/

CREATE TABLE [dbo].[OptionsGroups](
	[idOptionGroup] [int] IDENTITY(1,1) NOT NULL,
	[optionGroupDesc] [nvarchar](50) NULL,
	[optionReq] [nvarchar](1) NULL,
	[optionType] [nvarchar](1) NULL,
	[sortOrder] [int] NULL,
 CONSTRAINT [PK_OptionsGroups] PRIMARY KEY CLUSTERED 
(
	[idOptionGroup] ASC
)) ON [PRIMARY]

SET IDENTITY_INSERT TTSWebinarsSeeder.dbo.OptionsGroups on

INSERT TTSWebinarsSeeder.dbo.OptionsGroups
        (  [idOptionGroup]
      ,[optionGroupDesc]
      ,[optionReq]
      ,[optionType]
      ,[sortOrder])

SELECT [idOptionGroup]
      ,[optionGroupDesc]
      ,[optionReq]
      ,[optionType]
      ,[sortOrder]
  FROM TTSWebinars2_testing.[dbo].[OptionsGroups]


SET IDENTITY_INSERT TTSWebinarsSeeder.dbo.OptionsGroups off



/****** Object:  Table [dbo].[OptionsGroupsXref]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[OptionsGroupsXref]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[OptionsGroupsXref]
        (
          [idWebinarOptionGroup] [int] NOT NULL ,
          [idWebinar] [int] NOT NULL ,
          [idOptionGroup] [int] NOT NULL ,
          CONSTRAINT [PK_OptionsGroupsXref] PRIMARY KEY CLUSTERED ( [idWebinarOptionGroup] ASC )
        )
            ON
        [PRIMARY]
    END
GO
/****** Object:  Table [dbo].[OptionsXref]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[OptionsXref]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[OptionsXref]
        (
          [idOptionsXref] [int] IDENTITY(1, 1)
                                NOT NULL ,
          [idOptionGroup] [int] NOT NULL ,
          [idOption] [int] NOT NULL ,
          CONSTRAINT [PK_OptionsXref] PRIMARY KEY CLUSTERED ( [idOptionsXref] ASC )
        )
            ON
        [PRIMARY]
    END
GO
/****** Object:  Table [dbo].[Presenter]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[Presenter]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[Presenter]
        (
          [idUser] [int] NOT NULL ,
          [biography] [varchar](140) NULL ,
          [biographyLong] [varchar](2500) NOT NULL ,
          [altEmail] [varchar](50) NULL ,
          [altPhone] [varchar](50) NULL ,
          [instantMessage] [varchar](50) NULL ,
          [photoFull] [varchar](200) NULL ,
          [photoThumb] [varchar](200) NULL ,
          CONSTRAINT [PK_Presenter] PRIMARY KEY CLUSTERED ( [idUser] ASC )
        )
            ON
        [PRIMARY]
    END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[Users]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[Users]
        (
          [idUser] [int] NOT NULL ,
          [userType] [int] NOT NULL ,
          [acctStatus] [nvarchar](1) NOT NULL ,
          [acctPassword] [nvarchar](80) NOT NULL ,
          [dateCreated] [smalldatetime] NOT NULL ,
          [firstName] [nvarchar](50) NOT NULL ,
          [lastName] [nvarchar](50) NOT NULL ,
          [idUserInstitution] [int] NULL ,
          [phone1] [nvarchar](30) NOT NULL ,
          [phone2] [nvarchar](30) NULL ,
          [mAddress] [nvarchar](100) NOT NULL ,
          [mCity] [nvarchar](100) NOT NULL ,
          [mZip] [varchar](16) NOT NULL ,
          [mState] [nvarchar](2) NOT NULL ,
          [fax] [varchar](30) NULL ,
          [shippingFirstName] [nvarchar](50) NULL ,
          [shippingLastName] [nvarchar](50) NULL ,
          [shippingAddress] [nvarchar](100) NULL ,
          [shippingCity] [nvarchar](100) NULL ,
          [shippingState] [nvarchar](2) NULL ,
          [shippingZip] [nvarchar](20) NULL ,
          [shippingPhone] [nvarchar](30) NULL ,
          [email] [nvarchar](150) NOT NULL ,
          [futureMail] [nvarchar](1) NULL ,
          [generalComments] [varchar](1000) NULL ,
          [taxExempt] [bit] NULL ,
          [idAffiliate] [int] NOT NULL ,
          [idSubscriptionDiscount] [int] NULL ,
          [timeZone] [tinyint] NOT NULL ,
          [provisionalInstitution] [varchar](250) NOT NULL ,
          [mCountry] [varchar](2) NOT NULL ,
          [shippingCountry] [varchar](2) NOT NULL ,
          [hasLoggedIn] [tinyint] NULL ,
          [title] [varchar](200) NULL ,
          [shippingAddress2] [varchar](200) NULL ,
          [address2] [varchar](200) NULL ,
          CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ( [idUser] ASC )
        )
            ON
        [PRIMARY]
    END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Webinar]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[Webinar]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[Webinar]
        (
          [idWebinar] [int] NOT NULL ,
          [description] [varchar](MAX) NOT NULL ,
          [descriptionLong] [varchar](MAX) NOT NULL ,
          [imageUrl] [varchar](50) NULL ,
          [smallImageUrl] [varchar](50) NULL ,
          [status] [tinyint] NOT NULL ,
          [featuredWebinar] [bit] NOT NULL ,
          [reviewAllow] [bit] NOT NULL ,
          [title] [varchar](125) NOT NULL ,
          [date] [smalldatetime] NOT NULL ,
          [costRecording] [money] NOT NULL ,
          [costLive] [money] NOT NULL ,
          [costBoth] [money] NOT NULL ,
          [learnCaption] [varchar](255) NOT NULL ,
          [learnBody] [varchar](MAX) NOT NULL ,
          [whoAttend] [varchar](MAX) NOT NULL ,
          [duration] [decimal](4, 1) NOT NULL ,
          [recordingUrl] [varchar](300) NOT NULL ,
          [idPresenter] [int] NOT NULL ,
          [idHost] [int] NOT NULL ,
          [additionalNotifications] [varchar](MAX) NULL ,
          [cost6Months] [money] NOT NULL ,
          [cost12Months] [money] NOT NULL ,
          [ceu] [varchar](1000) NULL ,
          [DateCreated] [smalldatetime] NULL ,
          [DateChanged] [smalldatetime] NULL ,
          CONSTRAINT [PK_Webinars] PRIMARY KEY CLUSTERED ( [idWebinar] ASC )
        )
            ON
        [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WebinarFile]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[WebinarFile]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[WebinarFile]
        (
          [idWebinarFile] [int] NOT NULL ,
          [idWebinar] [int] NOT NULL ,
          [fileLocation] [varchar](255) NOT NULL ,
          [fileDesc] [varchar](1000) NOT NULL ,
          [myFilename] [varchar](255) NOT NULL ,
          CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED ( [idWebinarFile] ASC )
        )
            ON
        [PRIMARY]
    END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WebinarTopicXref]    Script Date: 11/23/2013 1:57:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS ( SELECT  *
                FROM    sys.objects
                WHERE   object_id = OBJECT_ID(N'[dbo].[WebinarTopicXref]')
                        AND type IN ( N'U' ) )
    BEGIN
        CREATE TABLE [dbo].[WebinarTopicXref]
        (
          [idWebinarTopicXref] [int] NOT NULL ,
          [idWebinar] [int] NOT NULL ,
          [idTopic] [int] NOT NULL ,
          CONSTRAINT [PK_Webinars_Categories] PRIMARY KEY CLUSTERED ( [idWebinarTopicXref] ASC )
        )
            ON
        [PRIMARY]
    END
GO
IF NOT EXISTS ( SELECT  *
                FROM    dbo.sysobjects
                WHERE   id = OBJECT_ID(N'[dbo].[DF_ELMAH_Error_ErrorId]')
                        AND type = 'D' )
    BEGIN
        ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (NEWID()) FOR [ErrorId]
    END

PRINT 'loading topics'
CREATE TABLE [dbo].[Topic]
(
  [Id] [int] NOT NULL ,
  [TopicDescription] [nvarchar](50) NOT NULL ,
  [ParentTopicId] [int] NULL ,
  [TopicHTML] [nvarchar](255) NOT NULL ,
  [SortOrder] [int] NOT NULL ,
  CONSTRAINT [PK_dbo.Topic] PRIMARY KEY CLUSTERED ( [Id] ASC )
)
    ON
[PRIMARY]

GO

GO
USE [master]
GO
ALTER DATABASE [TTSWebinarsSeeder] SET  READ_WRITE 
GO


USE TTSWebinarsSeeder
go


INSERT  INTO [dbo].[Affiliate]
        ( [idUser] ,
          [commissionModel] ,
          [URL] ,
          [webBanner] ,
          [webFooter] ,
          [emailBanner] ,
          [emailFooter] ,
          [ttsDomain] ,
          [GAPass] ,
          [supportEmail] ,
          [DisplayTitle] ,
          [BillingModel] ,
          [Logo] ,
          [ContactPerson] ,
          [ContactPhone] ,
          [ContactEmail] ,
          [ContactFax] ,
          [ContactAddress] ,
          [TechEmail] ,
          [TechPhone] ,
          [TechName] ,
          [EmailPromo]
		 )
        SELECT  idUser ,
                [commissionModel] ,
                [URL] ,
                [webBanner] ,
                [webFooter] ,
                [emailBanner] ,
                [emailFooter] ,
                [ttsDomain] ,
                [GAPass] ,
                [supportEmail] ,
                [DisplayTitle] ,
                [BillingModel] ,
                [Logo] ,
                [ContactPerson] ,
                [ContactPhone] ,
                [ContactEmail] ,
                [ContactFax] ,
                [ContactAddress] ,
                [TechEmail] ,
                [TechPhone] ,
                [TechName] ,
                [EmailPromo]
        FROM    TTSWebinars2_testing.[dbo].[Affiliate]



INSERT  INTO dbo.Users
        ( idUser ,
          userType ,
          acctStatus ,
          acctPassword ,
          dateCreated ,
          firstName ,
          lastName ,
          idUserInstitution ,
          phone1 ,
          phone2 ,
          mAddress ,
          mCity ,
          mZip ,
          mState ,
          fax ,
          shippingFirstName ,
          shippingLastName ,
          shippingAddress ,
          shippingCity ,
          shippingState ,
          shippingZip ,
          shippingPhone ,
          email ,
          futureMail ,
          generalComments ,
          taxExempt ,
          idAffiliate ,
          idSubscriptionDiscount ,
          timeZone ,
          provisionalInstitution ,
          mCountry ,
          shippingCountry ,
          hasLoggedIn ,
          title ,
          shippingAddress2 ,
          address2
        )
        SELECT  iduser ,
                [userType] ,
                [acctStatus] ,
                [acctPassword] ,
                [dateCreated] ,
                [firstName] ,
                [lastName] ,
                [idUserInstitution] ,
                [phone1] ,
                [phone2] ,
                [mAddress] ,
                [mCity] ,
                [mZip] ,
                [mState] ,
                [fax] ,
                [shippingFirstName] ,
                [shippingLastName] ,
                [shippingAddress] ,
                [shippingCity] ,
                [shippingState] ,
                [shippingZip] ,
                [shippingPhone] ,
                [email] ,
                [futureMail] ,
                [generalComments] ,
                [taxExempt] ,
                [idAffiliate] ,
                [idSubscriptionDiscount] ,
                [timeZone] ,
                [provisionalInstitution] ,
                [mCountry] ,
                [shippingCountry] ,
                [hasLoggedIn] ,
                [title] ,
                [shippingAddress2] ,
                [address2]
        FROM    TTSWebinars2_testing.dbo.Users
        WHERE   userType = 2
                OR userType = 3


INSERT  INTO [dbo].[Presenter]
        ( [idUser] ,
          [biography] ,
          [biographyLong] ,
          [altEmail] ,
          [altPhone] ,
          [instantMessage] ,
          [photoFull] ,
          [photoThumb]
        )
        SELECT  [idUser] ,
                [biography] ,
                [biographyLong] ,
                [altEmail] ,
                [altPhone] ,
                [instantMessage] ,
                [photoFull] ,
                [photoThumb]
        FROM    TTSWebinars2_testing.[dbo].[Presenter]


USE [TTSWebinarsSeeder]
GO

INSERT  INTO [dbo].[Institution]
        ( idInstitution ,
          [name] ,
          [idPrimaryContact] ,
          [zip] ,
          [domainName] ,
          [cert]
        )
        SELECT  [idInstitution] ,
                [name] ,
                [idPrimaryContact] ,
                [zip] ,
                [domainName] ,
                [cert]
        FROM    TTSWebinars2_testing.[dbo].[Institution]

INSERT  INTO [dbo].[Options]
        ( idOption ,
          [optionExplain] ,
          [optionLabel] ,
          [priceToAdd] ,
          --[SKU] ,
          [percToAdd] ,
          [sortOrder] ,
          [type] ,
          --[ShowLiveNotifications] ,
          --[ShowRecordingNotifications] ,
          --[ShowShippedNotifications] ,
          --[Stage1CheckoutConfirmationMsg] ,
          --[Stage2CheckoutConfirmationMsg] ,
          --[Stage1EmailConfirmationMsg] ,
          --[Stage2EmailConfirmationMsg] ,
          [taxExempt] ,
          [msgConfirm]
		 )
        SELECT  [idOption] ,
                [optionExplain] ,
                [optionLabel] ,
                [priceToAdd] ,
                --(SELECT SKU FROM TTSWebinars2_testing.dbo.Options WHERE [TTSWebinars2].[dbo].[Options].idOption = TTSWebinars2_testing.dbo.Options.idOption),
                [percToAdd] ,
                [sortOrder] ,
                [type] ,
				--,
    --            ,
    --            ,
    --            ,
    --            ,
    --            ,
    --            ,
                [taxExempt] ,
                [msgConfirm]
        FROM    TTSWebinars2_testing.[dbo].[Options]
USE [TTSWebinarsSeeder]
GO
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Session_Only_2Hr_255_1', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Connection information is summerized below. Detailed information has been sent to your email and is also available at http://www.bankwebinars.com.', [Stage2CheckoutConfirmationMsg] ='This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	1
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='On-Demand_Recording_Only_2Hr_255_2', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	2
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Plus_OnDemand_Weblinks_2Hr_355_3', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live session plus OnDemand playback for six months. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	3
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='6-Month_Subscription_6Month_9544', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Thank you for your 6 Month Registration to the Compliance Perspectives Series!', [Stage2CheckoutConfirmationMsg] ='Thank you for your 6 Month Registration to the Compliance Perspectives Series!', [Stage1EmailConfirmationMsg] ='Thank you for your 6 Month Registration to the Compliance Perspectives Series!', [Stage2EmailConfirmationMsg] ='Thank you for your 6 Month Registration to the Compliance Perspectives Series!' where idOption =	4
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='12-Month_Subscription_12Month_17495', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Thank you for your 12 Month Registration to the Compliance Perspectives Series!', [Stage2CheckoutConfirmationMsg] ='Thank you for your 12 Month Registration to the Compliance Perspectives Series!', [Stage1EmailConfirmationMsg] ='Thank you for your 12 Month Registration to the Compliance Perspectives Series!', [Stage2EmailConfirmationMsg] ='Thank you for your 12 Month Registration to the Compliance Perspectives Series!' where idOption =	5
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location(s)_2Hr_1509', [ShowLiveNotifications] ='', [ShowRecordingNotifications] ='', [ShowShippedNotifications] ='', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='', [Stage2EmailConfirmationMsg] ='' where idOption =	9
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location(s)_1Hr_10014', [ShowLiveNotifications] ='', [ShowRecordingNotifications] ='', [ShowShippedNotifications] ='', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='', [Stage2EmailConfirmationMsg] ='' where idOption =	14
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='6-Month_OnDemand_Weblink_2Hr_255_16', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Connection information is summerized below. Detailed information has been sent to your email and is also available at http://www.bankwebinars.com.', [Stage2CheckoutConfirmationMsg] ='This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	16
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_2Hr_295_17', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the CD-ROM and hardcopy of the presenter handouts and will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. ', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.' where idOption =	17
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Premier_Package_2Hr_395_18', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the live session plus OnDemand playback for six months. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. Watch your email or ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation). ', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	18
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Session_Only_1Hr_155_27', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live session only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email.', [Stage2CheckoutConfirmationMsg] ='This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	27
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='On-Demand_Recording_Only_1Hr_155_32', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	32
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Plus_OnDemand_Weblinks_1Hr_225_33', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live session plus OnDemand playback for six months. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	33
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='6-Month_OnDemand_Weblink_1Hr_155_34', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	34
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_1Hr_185_35', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the CD-ROM and hardcopy of the presenter handouts and will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. ', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.' where idOption =	35
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Premier_Package_1Hr_245_36', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='Your order provides for attendance to the live event plus all our post event options. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	36
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='1-Month_Trial_1Month_15938', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Thank you for your 1 Month Trial Registration to the Compliance Perspectives Series!', [Stage2CheckoutConfirmationMsg] ='Thank you for your 1 Month Trial Registration to the Compliance Perspectives Series!', [Stage1EmailConfirmationMsg] ='Thank you for your 1 Month Trial Registration to the Compliance Perspectives Series!', [Stage2EmailConfirmationMsg] ='Thank you for your 1 Month Trial Registration to the Compliance Perspectives Series!' where idOption =	38
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Only_-_4_Part_Series_Series4_899_39', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live sessions (4) only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email.', [Stage2CheckoutConfirmationMsg] ='This order is for all four live sessions in this series. The information required to connect to the first session is summarized below - new connection information will be provided for each of the following sessions a couple days prior to the event. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The webinar ID and password are distributed 2-3 days before each event.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	39
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='6-Month_OnDemand_Weblink_-_Series_Series4_899_40', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	40
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_-_Series_Series4_979_41', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the CD-ROMs and hardcopy of the presenter handouts for all 4 sessions and will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. ', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.' where idOption =	41
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Premium_Package_-_Series_Series4_1399_42', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='Your order provides for attendance to the live events (4) plus all our post event options. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	42
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location(s)_Series3_475_43', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='', [Stage2EmailConfirmationMsg] ='' where idOption =	43
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Series_In-Progress_2Hr_0_44', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Connection information is summerized below. Detailed information has been sent to your email and is also available at http://www.bankwebinars.com.', [Stage2CheckoutConfirmationMsg] ='This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	44
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Only_-_3_Part_Series_Series3_699_48', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live sessions (3) only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email.', [Stage2CheckoutConfirmationMsg] ='This order is for all three live sessions in this series. The information required to connect to the first session is summarized below - new connection information will be provided for each of the following sessions a couple days prior to the event. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The webinar ID and password are distributed 2-3 days before each event.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	48
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='6-Month_OnDemand_Weblink_-_Series_Series3_699_49', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback of all 3 sessions for six months following the date for the event by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event. Watch your email or ', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	49
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROMand_Hardcopy_Handouts_-_Series_Series3_749_50', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the CD-ROM and hardcopy of the presenter handouts for all 3 sessions and will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. ', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='' where idOption =	50
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Premium_Package_-_Series_Series3_995_51', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the live sessions (3) plus OnDemand playback for six months. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. Watch your email or ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	51
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location(s)_Series4_37552', [ShowLiveNotifications] ='', [ShowRecordingNotifications] ='', [ShowShippedNotifications] ='', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='', [Stage2EmailConfirmationMsg] ='' where idOption =	52
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
--NULL
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Session_Only_2Hr_255_84', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Connection information is summerized below. Detailed information has been sent to your email and is also available at http://www.bankwebinars.com.', [Stage2CheckoutConfirmationMsg] ='This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	84
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='OnDemand_Recording_Only_2Hr_255_85', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	85
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_2Hr_295_86', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the CD-ROM and hardcopy of the presenter handouts and will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. ', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.' where idOption =	86
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Plus_OnDemand_Weblinks_2Hr_355_87', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live session plus OnDemand playback for six months. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	87
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Premier_Package_2Hr_395_88', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the live session plus OnDemand playback for six months. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. Watch your email or ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation). ', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	88
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location_2Hr_150_89', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  =' ', [Stage2CheckoutConfirmationMsg] =' ', [Stage1EmailConfirmationMsg] =' ', [Stage2EmailConfirmationMsg] =' ' where idOption =	89
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='OnDemand_Recording_Only_2Hr_255_91', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	91
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_2Hr_295_92', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.' where idOption =	92
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Upgrade_to_OnDemand_Weblinks_2Hr_355_93', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='"', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	93
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Upgrade_to_the_Premier_Package_2Hr_395_95', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='"', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	95
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Session_Only_1Hr_155_97', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live session only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email.', [Stage2CheckoutConfirmationMsg] ='This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	97
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='OnDemand_Recording_Only_1Hr_155_98', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	98
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_1Hr_195_99', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the CD-ROM and hardcopy of the presenter handouts and will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. ', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.' where idOption =	99
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Plus_OnDemand_Weblinks_1Hr_225_100', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live session plus OnDemand playback for six months. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	100
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Premier_Package_1Hr_255_101', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='Your order provides for attendance to the live event plus all our post event options. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	101
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location_1Hr_100_102', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='', [Stage2EmailConfirmationMsg] ='' where idOption =	102
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='OnDemand_Recording_Only_1Hr_155_103', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	103
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_1Hr_195_104', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be accessed from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='' where idOption =	104
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Upgrade_to_OnDemand_Weblinks_1Hr_225_105', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='Your order is upgraded with a link for 6 months of unlimited OnDemand playback.', [Stage1EmailConfirmationMsg] ='"', [Stage2EmailConfirmationMsg] ='Your order is upgraded with a link for 6 months of unlimited OnDemand playback.' where idOption =	105
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Upgrade_to_the_Premier_Package_1Hr_255_107', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='Your order is now upgraded with a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped shortly.  We are emailing a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='"', [Stage2EmailConfirmationMsg] ='Your order is now upgraded with a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped shortly.  We are emailing a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	107
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Sessions_Only_Series3_699_109', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live sessions (3) only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email.', [Stage2CheckoutConfirmationMsg] ='This order is for all three live sessions in this series. The information required to connect to the first session is summarized below - new connection information will be provided for each of the following sessions a couple days prior to the event. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The webinar ID and password are distributed 2-3 days before each event.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	109
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='OnDemand_Recordings_Only_Series3_699_110', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback of all 3 sessions for six months following the date for the event by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event. Watch your email or ', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	110
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROMs_and_Hardcopy_Handouts_Series3_749_111', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the CD-ROM and hardcopy of the presenter handouts for all 3 sessions and will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. ', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='' where idOption =	111
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Plus_OnDemand_Weblinks_Series3_895_112', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for all three live sessions plus OnDemand playback for six months. The connection information for each event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	112
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Premier_Package_Series3_995_113', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the live sessions (3) plus OnDemand playback for six months. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event. Watch your email or ', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	113
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location_Series3_475_114', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='', [Stage2EmailConfirmationMsg] ='' where idOption =	114
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_Series3_749_115', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='' where idOption =	115
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Upgrade_to_OnDemand_Weblinks_Series3_1249_116', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	116
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Upgrade_to_the_Premier_Package_Series3_1399_117', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	117
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='One_Month_Trial_150_159_118', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='The webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage2CheckoutConfirmationMsg] ='Connection information is summerized below. Detailed information has been sent to your email and is also available at http://www.bankwebinars.com. For future events, the webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage1EmailConfirmationMsg] ='The webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com. For future events, the webinar ID and password and presenter notes are distributed 2-3 days before each months event.' where idOption =	118
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='6-Month_Subscription_954_119', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='The webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage2CheckoutConfirmationMsg] ='Connection information is summerized below. Detailed information has been sent to your email and is also available at http://www.bankwebinars.com. For future events, the webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage1EmailConfirmationMsg] ='The webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com. For future events, the webinar ID and password and presenter notes are distributed 2-3 days before each months event.' where idOption =	119
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='12-Month_Subscription_1749_120', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='The webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage2CheckoutConfirmationMsg] ='Connection information is summerized below. Detailed information has been sent to your email and is also available at http://www.bankwebinars.com. For future events, the webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage1EmailConfirmationMsg] ='The webinar ID and password and presenter notes are distributed 2-3 days before each months event.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com. For future events, the webinar ID and password and presenter notes are distributed 2-3 days before each months event.' where idOption =	120
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location_Subscription_150_121', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='', [Stage2EmailConfirmationMsg] ='' where idOption =	121
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='OnDemand_Recordings_Only_Series3_699_123', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	123
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Sessions_Only_Series4_899_124', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for the live sessions (4) only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email.', [Stage2CheckoutConfirmationMsg] ='This order is for all four live sessions in this series. The information required to connect to the first session is summarized below - new connection information will be provided for each of the following sessions a couple days prior to the event. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='The webinar ID and password are distributed 2-3 days before each event.', [Stage2EmailConfirmationMsg] ='Connection information is summerized below and is also available at http://www.BankWebinars.com.' where idOption =	124
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='OnDemand_Recordings_Only_Series4_899_125', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	125
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROMs_and_Hardcopy_Handouts_Series4_979_126', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='This order is for the CD-ROMs and hardcopy of the presenter handouts for all 4 sessions and will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. ', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.' where idOption =	126
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Live_Plus_OnDemand_Weblinks_Series4_1249_127', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='This order is for all four live sessions plus OnDemand playback for six months. The connection information for each event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	127
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Premier_Package_Series4_1399_128', [ShowLiveNotifications] ='Yes', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='Your order provides for attendance to the live events (4) plus all our post event options. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	128
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Additional_Location_Series4_600_129', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='No', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='', [Stage1EmailConfirmationMsg] ='', [Stage2EmailConfirmationMsg] ='' where idOption =	129
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='CD-ROM_and_Hardcopy_Handouts_Series4_979_130', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='', [Stage2CheckoutConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.', [Stage1EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped soon after the event. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution.', [Stage2EmailConfirmationMsg] ='The CD-ROM and hardcopy of the presenter handouts will be shipped within the next day or two. In addition, your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution and can be access from MyWebinars.' where idOption =	130
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='OnDemand_Recordings_Only_Series4_899_131', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.', [Stage1EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. You will receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order provides for unlimited, web-based playback (for six months following the date for the event) by any associate within your institution. A link for playback can be found at http://www.BankWebinars.com/MyWebinars.' where idOption =	131
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Upgrade_to_OnDemand_Weblinks_Series4_1249_132', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='No', [Stage1CheckoutConfirmationMsg]  ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a link to the recording (plus all presenter handouts) soon after the completion of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar plus a link for 6 months of unlimited OnDemand playback. You will receive a link to the recording (plus all presenter handouts) shortly after the completion of the event.' where idOption =	132
UPDATE  TTSWebinarsSeeder.[dbo].[Options] SET [SKU]='Upgrade_to_the_Premier_Package_Series4_1399_133', [ShowLiveNotifications] ='No', [ShowRecordingNotifications] ='Yes', [ShowShippedNotifications] ='Yes', [Stage1CheckoutConfirmationMsg]  ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2CheckoutConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.', [Stage1EmailConfirmationMsg] ='The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions. You will also receive a CD-ROM plus handouts shipped shortly after the event. Additionally, you will receive a link for unlimited playback for 6 months following the date of the event.', [Stage2EmailConfirmationMsg] ='Your order includes the Live Webinar (Webinar ID and password below); a link for 6-Month OnDemand playback; and a CD-ROM plus hardcopy handouts (to be shipped soon after the presentation).  You will receive a link to the OnDemand Recording (plus electronic handouts) soon after the completion of the event.' where idOption =	133


GO



INSERT  INTO [dbo].[OptionsXref]
        ( idOptionGroup ,
          idOption
        )
        SELECT  idOptionGroup ,
                idOption
        FROM    TTSWebinars2_testing.dbo.OptionsXref


INSERT  INTO [dbo].[Webinar]
        ( idwebinar ,
          [description] ,
          [descriptionLong] ,
          [imageUrl] ,
          [smallImageUrl] ,
          [status] ,
          [featuredWebinar] ,
          [reviewAllow] ,
          [title] ,
          [date] ,
          [costRecording] ,
          [costLive] ,
          [costBoth] ,
          [learnCaption] ,
          [learnBody] ,
          [whoAttend] ,
          [duration] ,
          [recordingUrl] ,
          [idPresenter] ,
          [idHost] ,
          [additionalNotifications] ,
          [cost6Months] ,
          [cost12Months] ,
          [ceu] ,
          [DateCreated] ,
          [DateChanged]
        )
        SELECT  [idWebinar] ,
                [description] ,
                [descriptionLong] ,
                [imageUrl] ,
                [smallImageUrl] ,
                [status] ,
                [featuredWebinar] ,
                [reviewAllow] ,
                [title] ,
                [date] ,
                [costRecording] ,
                [costLive] ,
                [costBoth] ,
                [learnCaption] ,
                [learnBody] ,
                [whoAttend] ,
                [duration] ,
                [recordingUrl] ,
                [idPresenter] ,
                [idHost] ,
                [additionalNotifications] ,
                [cost6Months] ,
                [cost12Months] ,
                [ceu] ,
                [DateCreated] ,
                [DateChanged]
        FROM    TTSWebinars2_testing.[dbo].[Webinar]



INSERT  INTO [dbo].[WebinarFile]
        ( idwebinarfile ,
          [idWebinar] ,
          [fileLocation] ,
          [fileDesc] ,
          [myFilename]
        )
        SELECT  [idWebinarFile] ,
                [idWebinar] ,
                [fileLocation] ,
                [fileDesc] ,
                [myFilename]
        FROM    TTSWebinars2_testing.[dbo].[WebinarFile]



INSERT  INTO [dbo].[OptionsGroupsXref]
        ( [idWebinarOptionGroup] ,
          [idWebinar] ,
          [idOptionGroup]
        )
        SELECT  [idWebinarOptionGroup] ,
                [idWebinar] ,
                [idOptionGroup]
        FROM    TTSWebinars2_testing.[dbo].[OptionsGroupsXref]
		

INSERT  INTO [dbo].[WebinarTopicXref]
        ( [idWebinarTopicXref] ,
          [idWebinar] ,
          [idTopic]
        )
        SELECT  [idWebinarTopicXref] ,
                [idWebinar] ,
                [idTopic]
        FROM    TTSWebinars2_testing.[dbo].[WebinarTopicXref]

INSERT  INTO [dbo].[Topic]
        ( id ,
          [TopicDescription] ,
          [TopicHTML] ,
          [SortOrder]
        )
        SELECT  idTopic ,
                topicDesc ,
                topicHTML ,
                sortOrder
        FROM    TTSWebinars2_testing.dbo.Topic

/****** Object:  StoredProcedure [dbo].[uspLogError]    Script Date: 11/23/2013 7:42:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


---- uspPrintError prints error information about the error that caused 
---- execution to jump to the CATCH block of a TRY...CATCH construct. 
---- Should be executed from within the scope of a CATCH block otherwise 
---- it will return without printing any error information.
--CREATE PROCEDURE [dbo].[uspPrintError]
--AS
--BEGIN
--    SET NOCOUNT ON;

--    -- Print error information. 
--    PRINT 'Error ' + CONVERT(VARCHAR(50), ERROR_NUMBER()) + ', Severity ' + CONVERT(VARCHAR(5), ERROR_SEVERITY()) + ', State '
--        + CONVERT(VARCHAR(5), ERROR_STATE()) + ', Procedure ' + ISNULL(ERROR_PROCEDURE(), '-') + ', Line ' + CONVERT(VARCHAR(5), ERROR_LINE());
--    PRINT ERROR_MESSAGE();
--END;

GO



-- uspLogError logs error information in the ErrorLog table about the 
-- error that caused execution to jump to the CATCH block of a 
-- TRY...CATCH construct. This should be executed from within the scope 
-- of a CATCH block otherwise it will return without inserting error 
-- information. 
--CREATE PROCEDURE [dbo].[uspLogError]
--    @ErrorLogID [int] = 0 OUTPUT -- contains the ErrorLogID of the row inserted
--AS -- by uspLogError in the ErrorLog table
--BEGIN
--    SET NOCOUNT ON;

--    -- Output parameter value of 0 indicates that error 
--    -- information was not logged
--    SET @ErrorLogID = 0;

--    BEGIN TRY
--        -- Return if there is no error information to log
--        IF ERROR_NUMBER() IS NULL
--            RETURN;

--        -- Return if inside an uncommittable transaction.
--        -- Data insertion/modification is not allowed when 
--        -- a transaction is in an uncommittable state.
--        IF XACT_STATE() = -1
--            BEGIN
--                PRINT 'Cannot log error since the current transaction is in an uncommittable state. '
--                    + 'Rollback the transaction before executing uspLogError in order to successfully log error information.';
--                RETURN;
--            END

--        INSERT  [dbo].[ErrorLog]
--                ( [UserName] ,
--                  [ErrorNumber] ,
--                  [ErrorSeverity] ,
--                  [ErrorState] ,
--                  [ErrorProcedure] ,
--                  [ErrorLine] ,
--                  [ErrorMessage]
--                )
--        VALUES  ( CONVERT(SYSNAME, CURRENT_USER) ,
--                  ERROR_NUMBER() ,
--                  ERROR_SEVERITY() ,
--                  ERROR_STATE() ,
--                  ERROR_PROCEDURE() ,
--                  ERROR_LINE() ,
--                  ERROR_MESSAGE()
--                );

--        -- Pass back the ErrorLogID of the row inserted
--        SET @ErrorLogID = @@IDENTITY;
--    END TRY
--    BEGIN CATCH
--        PRINT 'An error occurred in stored procedure uspLogError: ';
--        EXECUTE [dbo].[uspPrintError];
--        RETURN -1;
--    END CATCH
--END;
--go
USE master 
		GO

