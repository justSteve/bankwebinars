USE [master]
GO
RESTORE DATABASE [TTSWebinars2] FROM  DISK = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\Backup\mostRecent.BAK' WITH  FILE = 1,  MOVE N'TTSWebinars' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\TTSWebinars2.mdf',  MOVE N'TTSWebinars_log' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\TTSWebinars2_1.ldf',  NOUNLOAD,  REPLACE,  STATS = 5

GO

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'BWClean'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--Use this to see results
SELECT @SQL 
--Uncomment this to run it
EXEC(@SQL)

/****** Object:  Database [BWClean]    Script Date: 7/5/2014 6:34:08 PM ******/
DROP DATABASE [BWClean]
GO

USE [master]
GO


/****** Object:  Database [BWClean]    Script Date: 7/5/2014 6:59:57 PM ******/
CREATE DATABASE [BWClean] CONTAINMENT = NONE
 ON  PRIMARY ( NAME = N'BWClean', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\BWClean.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON ( NAME = N'BWClean_log', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\BWClean_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [BWClean] SET COMPATIBILITY_LEVEL = 110
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BWClean].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [BWClean] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [BWClean] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [BWClean] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [BWClean] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [BWClean] SET ARITHABORT OFF 
GO

ALTER DATABASE [BWClean] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [BWClean] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [BWClean] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [BWClean] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [BWClean] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [BWClean] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [BWClean] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [BWClean] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [BWClean] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [BWClean] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [BWClean] SET  DISABLE_BROKER 
GO

ALTER DATABASE [BWClean] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [BWClean] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [BWClean] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [BWClean] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [BWClean] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [BWClean] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [BWClean] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [BWClean] SET RECOVERY FULL 
GO

ALTER DATABASE [BWClean] SET  MULTI_USER 
GO

ALTER DATABASE [BWClean] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [BWClean] SET DB_CHAINING OFF 
GO

ALTER DATABASE [BWClean] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [BWClean] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [BWClean] SET  READ_WRITE 
GO
PRINT '-------------------------------------------------'
PRINT '---------Building BWClean Objects------------------'
PRINT '-------------------------------------------------'


USE [BWClean]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
--create PROCEDURE [dbo].[SeedOptionsGroupsXRef]
--AS
--BEGIN
--    SET NOCOUNT ON
--    PRINT '---------------------Starting [SeedOptionsGroupsXRef]'
--    DECLARE @ErrorLogID INT
    
--    DECLARE @QueryString VARCHAR(MAX)
	
----    SET IDENTITY_INSERT BWClean.dbo.OptionsGroupsXref ON
--    DECLARE my_Cursor CURSOR
--    FOR
--    SELECT  w.idWebinar ,
--            idOptionGroup
--    FROM    BWClean.dbo.Webinar w
--            INNER JOIN TTSWebinars2.dbo.OptionsGroupsXref ox ON ox.idWebinar = w.idWebinar
--    WHERE   w.status < 4
--            AND w.status > 1
--        --AND w.idWebinar NOT IN ( 1589, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1595, 1596, 1597, 1598, 1599, 1600, 1601,
--        --                         1602, 1603 )
--            AND w.idWebinar IN ( SELECT idWebinar
--                                 FROM   BWClean.dbo.Webinar )
--        --AND idOptionGroup IN ( 23, 24, 25, 26, 27, 28, 29, 30, 32, 33 )

--    DECLARE @idWebinar INT
--    DECLARE @idOptionGroup INT


--    OPEN my_Cursor

    
--    FETCH NEXT FROM my_Cursor INTO @idWebinar, @idOptionGroup
--    WHILE @@FETCH_STATUS = 0
--        BEGIN    		
--            BEGIN TRY
--                DECLARE @transOptionGroupID INT

--                SET @transOptionGroupID = ( SELECT  CASE ( SELECT   idOptionGroup
--                                                           FROM     TTSWebinars2.dbo.OptionsGroupsXref x
--                                                           WHERE    @idWebinar = x.idWebinar
--                                                         )
--                                                      WHEN 13 THEN 34
--                                                      WHEN 14 THEN 35
--                                                      WHEN 17 THEN 37
--                                                      WHEN 18 THEN 36
--                                                      WHEN 23 THEN 41
--                                                      WHEN 24 THEN 42
--                                                      WHEN 25 THEN 40
--                                                      WHEN 27 THEN 34
--                                                      WHEN 29 THEN 36
--                                                      WHEN 30 THEN 37
--                                                      WHEN 32 THEN 32
--                                                      WHEN 33 THEN 33
--                                                      WHEN 15 THEN 15
--                                                      WHEN 28 THEN 35
--                                                      ELSE 0
--                                                    END
--                                          )
--                INSERT  BWClean.dbo.OptionsGroupsXref
--                        ( idWebinar, idOptionGroup )
--                VALUES  ( @idWebinar, @transOptionGroupID )

--            END TRY
--            BEGIN CATCH

--			-- Call procedure to print error information.
--                --EXECUTE dbo.uspPrintError;

--    -- Roll back any active or uncommittable transactions before
--    -- inserting information in the ErrorLog.
--                IF XACT_STATE() <> 0
--                    BEGIN
--                        ROLLBACK TRANSACTION;
--                    END

--                EXECUTE dbo.uspLogError @Msg = @idWebinar, @ErrorLogID = @ErrorLogID OUTPUT;

--                PRINT '--==========--'
--                PRINT 'Error idWebinar: ' + CAST(@idWebinar AS VARCHAR) + ' idOptionGroup: ' + CAST(@idOptionGroup AS VARCHAR)+ ' New idOptionGroup: ' + CAST(@transOptionGroupID AS VARCHAR)
--                PRINT '--==========--'
			
--                FETCH NEXT FROM my_Cursor INTO @idWebinar
--            END CATCH

--            FETCH NEXT FROM my_Cursor INTO @idWebinar, @idOptionGroup
        
--        END
--    CLOSE my_Cursor
--    DEALLOCATE my_Cursor
--    --SET IDENTITY_INSERT BWClean.dbo.RegTypesGroupsXref OFF

--END
--go


Create table [BWClean].dbo.[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AdditionalLocations]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[AdditionalLocations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[idOrderRowOption] [int] NOT NULL,
 CONSTRAINT [PK_dbo.AdditionalLocations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Addresses]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Addresses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AddressType] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[StreetAddress] [nvarchar](max) NULL,
	[StreetAddress2] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Zip] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[idUser] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Addresses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Affiliate]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Affiliate](
	[idUserAff] [int] NOT NULL,
	[CommissionModel] [tinyint] NOT NULL,
	[URL] [nvarchar](500) NULL,
	[WebBanner] [nvarchar](4000) NOT NULL,
	[WebFooter] [nvarchar](4000) NOT NULL,
	[EmailBanner] [nvarchar](4000) NOT NULL,
	[EmailFooter] [nvarchar](4000) NOT NULL,
	[ttsDomain] [nvarchar](50) NULL,
	[GAPass] [nvarchar](50) NULL,
	[supportEmail] [nvarchar](75) NULL,
	[DisplayTitle] [nvarchar](150) NULL,
	[BillingModel] [nvarchar](150) NULL,
	[Logo] [nvarchar](150) NULL,
	[ContactPerson] [nvarchar](150) NULL,
	[ContactPhone] [nvarchar](150) NULL,
	[ContactEmail] [nvarchar](150) NULL,
	[ContactFax] [nvarchar](150) NULL,
	[ContactAddress] [nvarchar](150) NULL,
	[TechEmail] [nvarchar](150) NULL,
	[TechPhone] [nvarchar](150) NULL,
	[TechName] [nvarchar](150) NULL,
	[EmailPromo] [nvarchar](50) NULL,
	[WebUser_Id] [int] NULL,
 CONSTRAINT [PK_dbo.Affiliate] PRIMARY KEY CLUSTERED 
(
	[idUserAff] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Discounts]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Discounts](
	[idDiscounts] [int] IDENTITY(1,1) NOT NULL,
	[DiscountType] [int] NOT NULL,
	[DiscountCode] [nvarchar](50) NOT NULL,
	[PercentOff] [decimal](18, 2) NOT NULL,
	[FlatOff] [decimal](18, 2) NOT NULL,
	[usesNumber] [int] NOT NULL,
	[dateValidFrom] [datetime] NOT NULL,
	[dateValidTo] [datetime] NOT NULL,
	[status] [nvarchar](50) NOT NULL,
	[dateBilled] [datetime] NULL,
	[cost] [decimal](18, 2) NULL,
	[Notes] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Discounts] PRIMARY KEY CLUSTERED 
(
	[idDiscounts] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[ErrorLog](
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
/****** Object:  Table [dbo].[Institution]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Institution](
	[idInstitution] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionName] [nvarchar](250) NOT NULL,
	[InstitutionType] [nvarchar](20) NOT NULL,
	[domainName] [nvarchar](75) NULL,
	[RegIdentifier] [nvarchar](40) NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[Zip] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Institution] PRIMARY KEY CLUSTERED 
(
	[idInstitution] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Options]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Options](
	[idOption] [int] IDENTITY(1,1) NOT NULL,
	[OptionExplain] [nvarchar](max) NULL,
	[OptionLabel] [nvarchar](200) NULL,
	[PriceToAdd] [float] NULL,
	[TaxExempt] [bit] NULL,
	[PercToAdd] [float] NULL,
	[SortOrder] [int] NULL,
	[Type] [nvarchar](32) NOT NULL,
	[MsgConfirm] [nvarchar](10) NULL,
	[SKU] [nvarchar](150) NULL,
	[ShowLiveNotifications] [nvarchar](max) NULL,
	[ShowRecordingNotifications] [nvarchar](max) NULL,
	[ShowShippedNotifications] [nvarchar](max) NULL,
	[Stage1CheckoutConfirmationMsg] [nvarchar](max) NULL,
	[Stage2CheckoutConfirmationMsg] [nvarchar](max) NULL,
	[Stage1EmailConfirmationMsg] [nvarchar](max) NULL,
	[Stage2EmailConfirmationMsg] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Options] PRIMARY KEY CLUSTERED 
(
	[idOption] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OptionsGroups]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[OptionsGroups](
	[idOptionGroup] [int] IDENTITY(1,1) NOT NULL,
	[OptionGroupDesc] [nvarchar](50) NULL,
	[OptionType] [nvarchar](1) NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_dbo.OptionsGroups] PRIMARY KEY CLUSTERED 
(
	[idOptionGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OptionsGroupsXref]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[OptionsGroupsXref](
	[idWebinarOptionGroup] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[idOptionGroup] [int] NOT NULL,
 CONSTRAINT [PK_dbo.OptionsGroupsXref] PRIMARY KEY CLUSTERED 
(
	[idWebinarOptionGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OptionsXref]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[OptionsXref](
	[idOptionsXref] [int] IDENTITY(1,1) NOT NULL,
	[idOptionGroup] [int] NOT NULL,
	[idOption] [int] NOT NULL,
 CONSTRAINT [PK_dbo.OptionsXref] PRIMARY KEY CLUSTERED 
(
	[idOptionsXref] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderRow]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[OrderRow](
	[idOrderRow] [int] IDENTITY(1,1) NOT NULL,
	[idOrder] [int] NOT NULL,
	[idWebinar] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[RowPrice] [decimal](18, 2) NOT NULL,
	[idDiscount] [int] NULL,
	[AlternateEmail] [nvarchar](100) NOT NULL,
	[RegistrationType] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[ShipmentDate] [datetime] NULL,
	[isAdHocRecording] [bit] NOT NULL,
	[Royalty] [decimal](18, 2) NULL,
 CONSTRAINT [PK_dbo.OrderRow] PRIMARY KEY CLUSTERED 
(
	[idOrderRow] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderRowOptions]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[OrderRowOptions](
	[idOrderRowOption] [int] IDENTITY(1,1) NOT NULL,
	[idOrderRow] [int] NOT NULL,
	[idOption] [int] NOT NULL,
	[OptionPrice] [decimal](18, 2) NOT NULL,
	[OptionDescription] [nvarchar](255) NOT NULL,
	[TaxExempt] [bit] NOT NULL,
	[Type] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.OrderRowOptions] PRIMARY KEY CLUSTERED 
(
	[idOrderRowOption] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Orders]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Orders](
	[idOrder] [int] IDENTITY(1,1) NOT NULL,
	[idUser] [int] NOT NULL,
	[idAffiliate] [int] NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Institution] [nvarchar](250) NULL,
	[BillingPhone] [nvarchar](30) NULL,
	[BillingEmail] [nvarchar](150) NULL,
	[BillingAddress] [nvarchar](100) NULL,
	[BillingAddress2] [nvarchar](100) NULL,
	[BillingCity] [nvarchar](100) NULL,
	[BillingState] [nvarchar](100) NULL,
	[BillingZip] [nvarchar](20) NULL,
	[ShippingFirstName] [nvarchar](100) NULL,
	[ShippingLastName] [nvarchar](100) NULL,
	[ShippingPhone] [nvarchar](30) NULL,
	[ShippingAddress] [nvarchar](100) NULL,
	[ShippingAddress2] [nvarchar](100) NULL,
	[ShippingCity] [nvarchar](100) NULL,
	[ShippingState] [nvarchar](100) NULL,
	[ShippingZip] [nvarchar](20) NULL,
	[PaymentType] [tinyint] NOT NULL,
	[UserComments] [nvarchar](2550) NULL,
	[AuditInfo] [nvarchar](max) NULL,
	[AffiliateComments] [nvarchar](max) NULL,
	[AdminComments] [nvarchar](max) NULL,
	[TaxExempt] [bit] NOT NULL,
	[InitiatedBy] [tinyint] NOT NULL,
	[PaidByCCNumber] [nvarchar](40) NULL,
	[Origin] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Orders] PRIMARY KEY CLUSTERED 
(
	[idOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Presenter]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Presenter](
	[idUser] [int] NOT NULL,
	[Biography] [nvarchar](2500) NULL,
	[BiographyLong] [nvarchar](2500) NOT NULL,
	[PhotoFull] [nvarchar](400) NULL,
	[PhotoThumb] [nvarchar](400) NULL,
 CONSTRAINT [PK_dbo.Presenter] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Topic]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Topic](
	[idTopic] [int] IDENTITY(1,1) NOT NULL,
	[topicDesc] [nvarchar](50) NOT NULL,
	[idParentTopic] [int] NULL,
	[topicHTML] [nvarchar](255) NOT NULL,
	[sortOrder] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Topic] PRIMARY KEY CLUSTERED 
(
	[idTopic] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Webinar]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[Webinar](
	[idWebinar] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[DescriptionLong] [nvarchar](max) NOT NULL,
	[ImageUrl] [nvarchar](50) NULL,
	[SmallImageUrl] [nvarchar](50) NULL,
	[Status] [int] NOT NULL,
	[Title] [nvarchar](125) NOT NULL,
	[Date] [datetime] NOT NULL,
	[LearnCaption] [nvarchar](255) NOT NULL,
	[LearnBody] [nvarchar](max) NOT NULL,
	[WhoAttend] [nvarchar](max) NOT NULL,
	[Duration] [decimal](18, 2) NOT NULL,
	[RecordingUrl] [nvarchar](300) NOT NULL,
	[idPresenter] [int] NOT NULL,
	[AdditionalNotifications] [nvarchar](max) NULL,
	[ceu] [nvarchar](1000) NULL,
	[ConnectionInfo] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateChanged] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Webinar] PRIMARY KEY CLUSTERED 
(
	[idWebinar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WebinarFile]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[WebinarFile](
	[idWebinarFile] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[fileLocation] [nvarchar](255) NOT NULL,
	[fileDesc] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_dbo.WebinarFile] PRIMARY KEY CLUSTERED 
(
	[idWebinarFile] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WebinarTopicXref]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[WebinarTopicXref](
	[idWebinarTopicXref] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[idTopic] [int] NOT NULL,
 CONSTRAINT [PK_dbo.WebinarTopicXref] PRIMARY KEY CLUSTERED 
(
	[idWebinarTopicXref] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WebUser]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table [BWClean].dbo.[WebUser](
	[idUser] [int] NOT NULL,
	[UserType] [int] NOT NULL,
	[AcctStatus] [nvarchar](1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Initial] [nvarchar](max) NULL,
	[idUserInstitution] [int] NOT NULL,
	[email] [nvarchar](150) NOT NULL,
	[futureMail] [nvarchar](1) NULL,
	[generalComments] [nvarchar](1000) NULL,
	[taxExempt] [bit] NULL,
	[idSubscriptionDiscount] [int] NULL,
	[timeZone] [int] NOT NULL,
	[Title] [nvarchar](200) NULL,
 CONSTRAINT [PK_dbo.WebUser] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ErrorLog] ADD  CONSTRAINT [DF_ErrorLog_ErrorTime]  DEFAULT (getdate()) FOR [ErrorTime]
GO
ALTER TABLE [dbo].[AdditionalLocations]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdditionalLocations_dbo.OrderRowOptions_idOrderRowOption] FOREIGN KEY([idOrderRowOption])
REFERENCES [dbo].[OrderRowOptions] ([idOrderRowOption])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdditionalLocations] CHECK CONSTRAINT [FK_dbo.AdditionalLocations_dbo.OrderRowOptions_idOrderRowOption]
GO
ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Addresses_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES [dbo].[WebUser] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Addresses] CHECK CONSTRAINT [FK_dbo.Addresses_dbo.WebUser_idUser]
GO
ALTER TABLE [dbo].[Affiliate]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Affiliate_dbo.WebUser_idUserAff] FOREIGN KEY([idUserAff])
REFERENCES [dbo].[WebUser] ([idUser])
GO
ALTER TABLE [dbo].[Affiliate] CHECK CONSTRAINT [FK_dbo.Affiliate_dbo.WebUser_idUserAff]
GO
ALTER TABLE [dbo].[OptionsGroupsXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OptionsGroupsXref_dbo.OptionsGroups_idOptionGroup] FOREIGN KEY([idOptionGroup])
REFERENCES [dbo].[OptionsGroups] ([idOptionGroup])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OptionsGroupsXref] CHECK CONSTRAINT [FK_dbo.OptionsGroupsXref_dbo.OptionsGroups_idOptionGroup]
GO
ALTER TABLE [dbo].[OptionsGroupsXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OptionsGroupsXref_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OptionsGroupsXref] CHECK CONSTRAINT [FK_dbo.OptionsGroupsXref_dbo.Webinar_idWebinar]
GO
ALTER TABLE [dbo].[OptionsXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OptionsXref_dbo.Options_idOption] FOREIGN KEY([idOption])
REFERENCES [dbo].[Options] ([idOption])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OptionsXref] CHECK CONSTRAINT [FK_dbo.OptionsXref_dbo.Options_idOption]
GO
ALTER TABLE [dbo].[OptionsXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OptionsXref_dbo.OptionsGroups_idOptionGroup] FOREIGN KEY([idOptionGroup])
REFERENCES [dbo].[OptionsGroups] ([idOptionGroup])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OptionsXref] CHECK CONSTRAINT [FK_dbo.OptionsXref_dbo.OptionsGroups_idOptionGroup]
GO
ALTER TABLE [dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.Orders_idOrder] FOREIGN KEY([idOrder])
REFERENCES [dbo].[Orders] ([idOrder])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.Orders_idOrder]
GO
ALTER TABLE [dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.Webinar_idWebinar]
GO
ALTER TABLE [dbo].[OrderRowOptions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRowOptions_dbo.Options_idOption] FOREIGN KEY([idOption])
REFERENCES [dbo].[Options] ([idOption])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRowOptions] CHECK CONSTRAINT [FK_dbo.OrderRowOptions_dbo.Options_idOption]
GO
ALTER TABLE [dbo].[OrderRowOptions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRowOptions_dbo.OrderRow_idOrderRow] FOREIGN KEY([idOrderRow])
REFERENCES [dbo].[OrderRow] ([idOrderRow])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRowOptions] CHECK CONSTRAINT [FK_dbo.OrderRowOptions_dbo.OrderRow_idOrderRow]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Orders_dbo.Affiliate_idAffiliate] FOREIGN KEY([idAffiliate])
REFERENCES [dbo].[Affiliate] ([idUserAff])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_dbo.Orders_dbo.Affiliate_idAffiliate]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Orders_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES [dbo].[WebUser] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_dbo.Orders_dbo.WebUser_idUser]
GO
ALTER TABLE [dbo].[Presenter]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Presenter_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES [dbo].[WebUser] ([idUser])
GO
ALTER TABLE [dbo].[Presenter] CHECK CONSTRAINT [FK_dbo.Presenter_dbo.WebUser_idUser]
GO
ALTER TABLE [dbo].[Topic]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Topic_dbo.Topic_idParentTopic] FOREIGN KEY([idParentTopic])
REFERENCES [dbo].[Topic] ([idTopic])
GO
ALTER TABLE [dbo].[Topic] CHECK CONSTRAINT [FK_dbo.Topic_dbo.Topic_idParentTopic]
GO
ALTER TABLE [dbo].[Webinar]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Webinar_dbo.Presenter_idPresenter] FOREIGN KEY([idPresenter])
REFERENCES [dbo].[Presenter] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Webinar] CHECK CONSTRAINT [FK_dbo.Webinar_dbo.Presenter_idPresenter]
GO
ALTER TABLE [dbo].[WebinarFile]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarFile_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebinarFile] CHECK CONSTRAINT [FK_dbo.WebinarFile_dbo.Webinar_idWebinar]
GO
ALTER TABLE [dbo].[WebinarTopicXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Topic_idTopic] FOREIGN KEY([idTopic])
REFERENCES [dbo].[Topic] ([idTopic])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebinarTopicXref] CHECK CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Topic_idTopic]
GO
ALTER TABLE [dbo].[WebinarTopicXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebinarTopicXref] CHECK CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Webinar_idWebinar]
GO
ALTER TABLE [dbo].[WebUser]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebUser_dbo.Institution_idUserInstitution] FOREIGN KEY([idUserInstitution])
REFERENCES [dbo].[Institution] ([idInstitution])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebUser] CHECK CONSTRAINT [FK_dbo.WebUser_dbo.Institution_idUserInstitution]
GO
USE master
GO
PRINT '-------------------------------------------------'
PRINT '---EXEC BWMigrator.dbo._SeedBankWebinars----------------------------------------------'
PRINT '-------------------------------------------------'
    EXEC BWMigrator.dbo._SeedBankWebinars


DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'BankWebinars'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--Use this to see results
SELECT @SQL 
--Uncomment this to run it
EXEC(@SQL)

PRINT '-------------------------------------------------'
PRINT '---DROP & RECREATE BankWebinars----------------------------------------------'
PRINT '-------------------------------------------------'
 USE [master]
GO


/****** Object:  Database [BankWebinars]    Script Date: 7/7/2014 7:51:40 AM ******/
DROP DATABASE [BankWebinars]
GO
USE [master]
GO

/****** Object:  Database [BankWebinars]    Script Date: 7/7/2014 7:52:03 AM ******/
CREATE DATABASE [BankWebinars]
 CONTAINMENT = NONE
 ON  PRIMARY 

( NAME = N'BankWebinars_Data', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\BankWebinars.mdf' , SIZE = 14400KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'BankWebinars_Log', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\BankWebinars.ldf' , SIZE = 20096KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 COLLATE  Latin1_General_CI_AS
GO
 ALTER DATABASE [BankWebinars] SET MULTI_USER WITH ROLLBACK IMMEDIATE

ALTER DATABASE [BankWebinars] SET COMPATIBILITY_LEVEL = 110
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BankWebinars].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO

ALTER DATABASE [BankWebinars] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [BankWebinars] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [BankWebinars] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [BankWebinars] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [BankWebinars] SET ARITHABORT OFF 
GO

ALTER DATABASE [BankWebinars] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [BankWebinars] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [BankWebinars] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [BankWebinars] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [BankWebinars] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [BankWebinars] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [BankWebinars] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [BankWebinars] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [BankWebinars] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [BankWebinars] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [BankWebinars] SET  DISABLE_BROKER 
GO

ALTER DATABASE [BankWebinars] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [BankWebinars] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [BankWebinars] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [BankWebinars] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO

ALTER DATABASE [BankWebinars] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [BankWebinars] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [BankWebinars] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [BankWebinars] SET RECOVERY FULL 
GO

ALTER DATABASE [BankWebinars] SET  MULTI_USER 
GO

ALTER DATABASE [BankWebinars] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [BankWebinars] SET DB_CHAINING OFF 
GO

ALTER DATABASE [BankWebinars] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [BankWebinars] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [BankWebinars] SET  READ_WRITE 
GO

/****** Object:  StoredProcedure [dbo].[InsertGTWConnectionInfo]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE BankWebinars
GO

/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[AdditionalLocation]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[AdditionalLocation](
	[idAdditionalLocation] [int] IDENTITY(1,1) NOT NULL,
	[idOrderRow] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[DescriptionPromo] [nvarchar](max) NULL,
	[DescriptionConfirm] [nvarchar](max) NULL,
	[TaxExempt] [bit] NOT NULL,
	[Email] [nvarchar](max) NULL,
	[FullName] [nvarchar](max) NULL,
	[Billable] [bit] NOT NULL,
	[JoinURL] [varchar](225) NULL,
	[RegistrantKey] [varchar](25) NULL,
CONSTRAINT [PK_dbo.AdditionalLocation] PRIMARY KEY CLUSTERED 
(
	[idAdditionalLocation] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Address]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[Address](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AddressType] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[StreetAddress] [nvarchar](max) NULL,
	[StreetAddress2] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Zip] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[idUser] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Address] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Affiliate]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[Affiliate](
	[idUserAff] [int] NOT NULL,
	[CommissionModel] [tinyint] NOT NULL,
	[URL] [nvarchar](500) NULL,
	[WebBanner] [nvarchar](4000) NOT NULL,
	[WebFooter] [nvarchar](4000) NOT NULL,
	[EmailBanner] [nvarchar](4000) NOT NULL,
	[EmailFooter] [nvarchar](4000) NOT NULL,
	[ttsDomain] [nvarchar](50) NULL,
	[GAPass] [nvarchar](50) NULL,
	[supportEmail] [nvarchar](75) NULL,
	[DisplayTitle] [nvarchar](150) NULL,
	[BillingModel] [nvarchar](150) NULL,
	[Logo] [nvarchar](150) NULL,
	[ContactPerson] [nvarchar](150) NULL,
	[ContactPhone] [nvarchar](150) NULL,
	[ContactEmail] [nvarchar](150) NULL,
	[ContactFax] [nvarchar](150) NULL,
	[ContactAddress] [nvarchar](150) NULL,
	[TechEmail] [nvarchar](150) NULL,
	[TechPhone] [nvarchar](150) NULL,
	[TechName] [nvarchar](150) NULL,
	[EmailPromo] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Affiliate] PRIMARY KEY CLUSTERED 
(
	[idUserAff] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Discount]    Script Date: 7/7/2014 7:53:31 AM ******/

Create table BankWebinars.dbo.[Discount](
	[idDiscount] [int] IDENTITY(1,1) NOT NULL,
	[DiscountType] [int] NOT NULL,
	[DiscountCode] [nvarchar](50) NOT NULL,
	[PercentOff] [decimal](18, 2) NULL,
	[FlatOff] [decimal](18, 2) NULL,
	[UsesCount] [int] NOT NULL,
	[UsesRemain] [int] NOT NULL,
	[DateValidFrom] [datetime] NOT NULL,
	[DateValidTo] [datetime] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[DateBilled] [datetime] NULL,
	[Cost] [decimal](18, 2) NULL,
	[Notes] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Discount] PRIMARY KEY CLUSTERED 
(
	[idDiscount] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Institution]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[Institution](
	[idInstitution] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionName] [nvarchar](250) NOT NULL,
	[InstitutionType] [nvarchar](20) NOT NULL,
	[domainName] [nvarchar](75) NULL,
	[RegIdentifier] [nvarchar](40) NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[Zip] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Institution] PRIMARY KEY CLUSTERED 
(
	[idInstitution] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Order]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[Order](
	[idOrder] [int] IDENTITY(1,1) NOT NULL,
	[idUser] [int] NOT NULL,
	[idAffiliate] [int] NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[OrderStatus] [int] NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Institution] [nvarchar](250) NULL,
	[BillingPhone] [nvarchar](30) NULL,
	[BillingEmail] [nvarchar](150) NULL,
	[BillingAddress] [nvarchar](200) NULL,
	[BillingAddress2] [nvarchar](200) NULL,
	[BillingCity] [nvarchar](100) NULL,
	[BillingState] [nvarchar](100) NULL,
	[BillingZip] [nvarchar](20) NULL,
	[ShippingFirstName] [nvarchar](100) NULL,
	[ShippingLastName] [nvarchar](100) NULL,
	[ShippingPhone] [nvarchar](30) NULL,
	[ShippingAddress] [nvarchar](100) NULL,
	[ShippingAddress2] [nvarchar](100) NULL,
	[ShippingCity] [nvarchar](100) NULL,
	[ShippingState] [nvarchar](100) NULL,
	[ShippingZip] [nvarchar](20) NULL,
	[PaymentType] [tinyint] NOT NULL,
	[UserComments] [nvarchar](2550) NULL,
	[AuditInfo] [nvarchar](max) NULL,
	[AffiliateComments] [nvarchar](max) NULL,
	[AdminComments] [nvarchar](max) NULL,
	[TaxExempt] [bit] NOT NULL,
	[Origin] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Order] PRIMARY KEY CLUSTERED 
(
	[idOrder] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[OrderRow]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[OrderRow](
	[idOrderRow] [int] IDENTITY(1,1) NOT NULL,
	[idOrder] [int] NOT NULL,
	[idWebinar] [int] NOT NULL,
	[idRegType] [int] NOT NULL,
	[RegistrantKey] [nvarchar](25) NULL,
	[JoinURL] [nvarchar](125) NULL,
	[UnitPrice] [decimal](8, 2) NOT NULL,
	[RowPrice] [decimal](8, 2) NOT NULL,
	[Royalty] [decimal](8, 2) NOT NULL,
	[ShipmentDate] [datetime] NULL,
	[AccessExpires] [datetime] NULL,
	[RowStatus] [int] NOT NULL,
	[Discount_idDiscount] [int] NULL,
 CONSTRAINT [PK_dbo.OrderRow] PRIMARY KEY CLUSTERED 
(
	[idOrderRow] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Presenter]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[Presenter](
	[idUser] [int] NOT NULL,
	[Biography] [nvarchar](2500) NULL,
	[BiographyLong] [nvarchar](2500) NOT NULL,
	[PhotoFull] [nvarchar](400) NULL,
	[PhotoThumb] [nvarchar](400) NULL,
 CONSTRAINT [PK_dbo.Presenter] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

/****** Object:  Table [dbo].[RegType]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[RegType](
	[idRegType] [int] IDENTITY(1,1) NOT NULL,
	[RegTypeExplain] [nvarchar](max) NULL,
	[RegTypeLabel] [nvarchar](max) NULL,
	[Price] [float] NOT NULL,
	[TaxExempt] [bit] NULL,
	[SortOrder] [int] NOT NULL,
	[SKU] [nvarchar](max) NULL,
	[ShowLiveNotifications] [nvarchar](max) NULL,
	[ShowRecordingNotifications] [nvarchar](max) NULL,
	[ShowShippedNotifications] [nvarchar](max) NULL,
	[Stage1CheckoutConfirmationMsg] [nvarchar](max) NULL,
	[Stage2CheckoutConfirmationMsg] [nvarchar](max) NULL,
	[Stage1EmailConfirmationMsg] [nvarchar](max) NULL,
	[Stage2EmailConfirmationMsg] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.RegType] PRIMARY KEY CLUSTERED 
(
	[idRegType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[RegTypesGroups]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[RegTypesGroups](
	[idRegTypeGroup] [int] IDENTITY(1,1) NOT NULL,
	[RegTypeGroupDesc] [nvarchar](50) NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_dbo.RegTypesGroups] PRIMARY KEY CLUSTERED 
(
	[idRegTypeGroup] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[RegTypesGroupsXref]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[RegTypesGroupsXref](
	[idWebinarRegTypeGroup] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[idRegTypeGroup] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RegTypesGroupsXref] PRIMARY KEY CLUSTERED 
(
	[idWebinarRegTypeGroup] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[RegTypesXref]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[RegTypesXref](
	[idRegTypesXref] [int] IDENTITY(1,1) NOT NULL,
	[idRegTypeGroup] [int] NOT NULL,
	[idRegType] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RegTypesXref] PRIMARY KEY CLUSTERED 
(
	[idRegTypesXref] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Topic]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[Topic](
	[idTopic] [int] IDENTITY(1,1) NOT NULL,
	[topicDesc] [nvarchar](50) NOT NULL,
	[idParentTopic] [int] NULL,
	[topicHTML] [nvarchar](255) NOT NULL,
	[sortOrder] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Topic] PRIMARY KEY CLUSTERED 
(
	[idTopic] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Webinar]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[Webinar](
	[idWebinar] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[DescriptionLong] [nvarchar](max) NOT NULL,
	[ImageUrl] [nvarchar](50) NULL,
	[SmallImageUrl] [nvarchar](50) NULL,
	[Status] [int] NOT NULL,
	[Title] [nvarchar](125) NOT NULL,
	[Date] [datetime] NOT NULL,
	[LearnCaption] [nvarchar](255) NOT NULL,
	[LearnBody] [nvarchar](max) NOT NULL,
	[WhoAttend] [nvarchar](max) NOT NULL,
	[Duration] [decimal](18, 2) NOT NULL,
	[RecordingUrl] [nvarchar](300) NOT NULL,
	[idPresenter] [int] NOT NULL,
	[WebinarKey] [nvarchar](max) NULL,
	[OrganizerKey] [nvarchar](max) NULL,
	[OrganizerOAuthKey] [nvarchar](max) NULL,
	[ceu] [nvarchar](1000) NULL,
	[ConnectionInfo] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateChanged] [datetime] NOT NULL,
	[AccessPhoneAttendee] [nvarchar](max) NULL,
	[AccessCodeAttendee] [nvarchar](max) NULL,
	[AccessPhonePresenter] [nvarchar](max) NULL,
	[AccessCodePresenter] [nvarchar](max) NULL,
	[AccessPhoneOrganizer] [nvarchar](max) NULL,
	[AccessCodeOrganizer] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Webinar] PRIMARY KEY CLUSTERED 
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[WebinarFile]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[WebinarFile](
	[idWebinarFile] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[fileLocation] [nvarchar](255) NOT NULL,
	[fileDesc] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_dbo.WebinarFile] PRIMARY KEY CLUSTERED 
(
	[idWebinarFile] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[WebinarTopicXref]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[WebinarTopicXref](
	[idWebinarTopicXref] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[idTopic] [int] NOT NULL,
 CONSTRAINT [PK_dbo.WebinarTopicXref] PRIMARY KEY CLUSTERED 
(
	[idWebinarTopicXref] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[WebUser]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table BankWebinars.dbo.[WebUser](
	[idUser] [int] NOT NULL,
	[UserType] [int] NOT NULL,
	[AcctStatus] [nvarchar](1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Initial] [nvarchar](50) NULL,
	[idUserInstitution] [int] NOT NULL,
	[email] [nvarchar](150) NOT NULL,
	[futureMail] [nvarchar](1) NULL,
	[generalComments] [nvarchar](1000) NULL,
	[taxExempt] [bit] NULL,
	[idSubscriptionDiscount] [int] NULL,
	[timeZone] [int] NOT NULL,
	[Title] [nvarchar](200) NULL,
 CONSTRAINT [PK_dbo.WebUser] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Index [IX_idOrderRow]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idOrderRow] ON [dbo].[AdditionalLocation]
(
	[idOrderRow] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUser]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON [dbo].[Address]
(
	[idUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUserAff]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUserAff] ON [dbo].[Affiliate]
(
	[idUserAff] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idAffiliate]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idAffiliate] ON [dbo].[Order]
(
	[idAffiliate] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUser]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON [dbo].[Order]
(
	[idUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_Discount_idDiscount]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_Discount_idDiscount] ON [dbo].[OrderRow]
(
	[Discount_idDiscount] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idOrder]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idOrder] ON [dbo].[OrderRow]
(
	[idOrder] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idRegType]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idRegType] ON [dbo].[OrderRow]
(
	[idRegType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON [dbo].[OrderRow]
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUser]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON [dbo].[Presenter]
(
	[idUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idRegTypeGroup]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idRegTypeGroup] ON [dbo].[RegTypesGroupsXref]
(
	[idRegTypeGroup] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON [dbo].[RegTypesGroupsXref]
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idRegType]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idRegType] ON [dbo].[RegTypesXref]
(
	[idRegType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idRegTypeGroup]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idRegTypeGroup] ON [dbo].[RegTypesXref]
(
	[idRegTypeGroup] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idParentTopic]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idParentTopic] ON [dbo].[Topic]
(
	[idParentTopic] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idPresenter]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idPresenter] ON [dbo].[Webinar]
(
	[idPresenter] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON [dbo].[WebinarFile]
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idTopic]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idTopic] ON [dbo].[WebinarTopicXref]
(
	[idTopic] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON [dbo].[WebinarTopicXref]
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUserInstitution]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUserInstitution] ON [dbo].[WebUser]
(
	[idUserInstitution] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
ALTER TABLE [dbo].[AdditionalLocation]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdditionalLocation_dbo.OrderRow_idOrderRow] FOREIGN KEY([idOrderRow])
REFERENCES [dbo].[OrderRow] ([idOrderRow])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdditionalLocation] CHECK CONSTRAINT [FK_dbo.AdditionalLocation_dbo.OrderRow_idOrderRow]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Address_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES [dbo].[WebUser] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_dbo.Address_dbo.WebUser_idUser]
GO
ALTER TABLE [dbo].[Affiliate]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Affiliate_dbo.WebUser_idUserAff] FOREIGN KEY([idUserAff])
REFERENCES [dbo].[WebUser] ([idUser])
GO
ALTER TABLE [dbo].[Affiliate] CHECK CONSTRAINT [FK_dbo.Affiliate_dbo.WebUser_idUserAff]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Order_dbo.Affiliate_idAffiliate] FOREIGN KEY([idAffiliate])
REFERENCES [dbo].[Affiliate] ([idUserAff])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_dbo.Order_dbo.Affiliate_idAffiliate]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Order_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES [dbo].[WebUser] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_dbo.Order_dbo.WebUser_idUser]
GO
ALTER TABLE [dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.Discount_Discount_idDiscount] FOREIGN KEY([Discount_idDiscount])
REFERENCES [dbo].[Discount] ([idDiscount])
GO
ALTER TABLE [dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.Discount_Discount_idDiscount]
GO
ALTER TABLE [dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.Order_idOrder] FOREIGN KEY([idOrder])
REFERENCES [dbo].[Order] ([idOrder])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.Order_idOrder]
GO
ALTER TABLE [dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.RegType_idRegType] FOREIGN KEY([idRegType])
REFERENCES [dbo].[RegType] ([idRegType])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.RegType_idRegType]
GO
ALTER TABLE [dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.Webinar_idWebinar]
GO
ALTER TABLE [dbo].[Presenter]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Presenter_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES [dbo].[WebUser] ([idUser])
GO
ALTER TABLE [dbo].[Presenter] CHECK CONSTRAINT [FK_dbo.Presenter_dbo.WebUser_idUser]
GO
ALTER TABLE [dbo].[RegTypesGroupsXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RegTypesGroupsXref_dbo.RegTypesGroups_idRegTypeGroup] FOREIGN KEY([idRegTypeGroup])
REFERENCES [dbo].[RegTypesGroups] ([idRegTypeGroup])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RegTypesGroupsXref] CHECK CONSTRAINT [FK_dbo.RegTypesGroupsXref_dbo.RegTypesGroups_idRegTypeGroup]
GO
ALTER TABLE [dbo].[RegTypesGroupsXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RegTypesGroupsXref_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RegTypesGroupsXref] CHECK CONSTRAINT [FK_dbo.RegTypesGroupsXref_dbo.Webinar_idWebinar]
GO
ALTER TABLE [dbo].[RegTypesXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RegTypesXref_dbo.RegType_idRegType] FOREIGN KEY([idRegType])
REFERENCES [dbo].[RegType] ([idRegType])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RegTypesXref] CHECK CONSTRAINT [FK_dbo.RegTypesXref_dbo.RegType_idRegType]
GO
ALTER TABLE [dbo].[RegTypesXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RegTypesXref_dbo.RegTypesGroups_idRegTypeGroup] FOREIGN KEY([idRegTypeGroup])
REFERENCES [dbo].[RegTypesGroups] ([idRegTypeGroup])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RegTypesXref] CHECK CONSTRAINT [FK_dbo.RegTypesXref_dbo.RegTypesGroups_idRegTypeGroup]
GO
ALTER TABLE [dbo].[Topic]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Topic_dbo.Topic_idParentTopic] FOREIGN KEY([idParentTopic])
REFERENCES [dbo].[Topic] ([idTopic])
GO
ALTER TABLE [dbo].[Topic] CHECK CONSTRAINT [FK_dbo.Topic_dbo.Topic_idParentTopic]
GO
ALTER TABLE [dbo].[Webinar]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Webinar_dbo.Presenter_idPresenter] FOREIGN KEY([idPresenter])
REFERENCES [dbo].[Presenter] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Webinar] CHECK CONSTRAINT [FK_dbo.Webinar_dbo.Presenter_idPresenter]
GO
ALTER TABLE [dbo].[WebinarFile]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarFile_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebinarFile] CHECK CONSTRAINT [FK_dbo.WebinarFile_dbo.Webinar_idWebinar]
GO
ALTER TABLE [dbo].[WebinarTopicXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Topic_idTopic] FOREIGN KEY([idTopic])
REFERENCES [dbo].[Topic] ([idTopic])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebinarTopicXref] CHECK CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Topic_idTopic]
GO
ALTER TABLE [dbo].[WebinarTopicXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebinarTopicXref] CHECK CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Webinar_idWebinar]
GO
ALTER TABLE [dbo].[WebUser]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebUser_dbo.Institution_idUserInstitution] FOREIGN KEY([idUserInstitution])
REFERENCES [dbo].[Institution] ([idInstitution])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebUser] CHECK CONSTRAINT [FK_dbo.WebUser_dbo.Institution_idUserInstitution]
GO


-- =============================================
-- Author:		sjh
-- Create date: 5/14
-- Description:	Updates webinar table with GTW connection info
-- =============================================
CREATE PROCEDURE [dbo].[InsertGTWConnectionInfo] 
	-- Add the parameters for the stored procedure here
    @idWebinar INT, 
	@Status INT ,
    @WebinarKey NVARCHAR(MAX) ,
    @OrganizerKey NVARCHAR(MAX) ,
    @OrganizerOAuthKey NVARCHAR(MAX) ,
    @AccessPhoneAttendee NVARCHAR(MAX) ,
    @AccessCodeAttendee NVARCHAR(MAX) ,
    @AccessPhonePresenter NVARCHAR(MAX) ,
    @AccessCodePresenter NVARCHAR(MAX) ,
    @AccessPhoneOrganizer NVARCHAR(MAX) ,
    @AccessCodeOrganizer NVARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
    SET NOCOUNT ON;
	UPDATE [dbo].[Webinar]
   SET [Status] = 7
      ,[WebinarKey] = @WebinarKey
      ,[OrganizerKey] = @OrganizerKey
      ,[OrganizerOAuthKey] = @OrganizerOAuthKey
      ,[AccessPhoneAttendee] = @AccessPhoneAttendee
      ,[AccessCodeAttendee] = @AccessCodeAttendee
      ,[AccessPhonePresenter] = @AccessPhonePresenter
      ,[AccessCodePresenter] = @AccessCodePresenter
      ,[AccessPhoneOrganizer] = @AccessPhoneOrganizer
      ,[AccessCodeOrganizer] = @AccessCodeOrganizer
	  WHERE idWebinar = @idWebinar

END
GO
EXEC	BWMigrator.[dbo].[Migrate_Runner]


GO
IF EXISTS (

SELECT idWebinar, Status, Title FROM BankWebinars.dbo.Webinar WHERE idWebinar NOT IN (SELECT idWebinar FROM  BankWebinars.dbo.WebinarTopicXref))
BEGIN
	PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'
	PRINT '           Missing topic IDs'
	PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'
	

SELECT idWebinar, Status, Title FROM BankWebinars.dbo.Webinar WHERE idWebinar NOT IN (SELECT idWebinar FROM  BankWebinars.dbo.WebinarTopicXref)
PRINT ''
PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'
PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'
PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'


END
SET IDENTITY_INSERT BankWebinars.dbo.RegType ON 