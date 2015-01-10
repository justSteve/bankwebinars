USE [master]
GO
RESTORE DATABASE [TTSWebinars2] FROM  DISK = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\Backup\mostRecent.BAK' WITH  FILE = 1,  MOVE N'TTSWebinars' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\TTSWebinars2.mdf',  MOVE N'TTSWebinars_log' TO N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\TTSWebinars2_1.ldf',  NOUNLOAD,  REPLACE,  STATS = 5

GO

DECLARE @DatabaseName NVARCHAR(50)
SET @DatabaseName = N'CUWebinarsClean'

DECLARE @SQL VARCHAR(MAX)

SELECT  @SQL = COALESCE(@SQL, '') + 'Kill ' + CONVERT(VARCHAR, SPId) + ';'
FROM    master..SysProcesses
WHERE   DBId = DB_ID(@DatabaseName)
        AND SPId <> @@SPId

--Use this to see results
SELECT  @SQL 
--Uncomment this to run it
EXEC(@SQL)

/****** Object:  Database [CUWebinarsClean]    Script Date: 7/5/2014 6:34:08 PM ******/
DROP DATABASE [CUWebinarsClean]
GO

USE [master]
GO


/****** Object:  Database [CUWebinarsClean]    Script Date: 7/5/2014 6:59:57 PM ******/
CREATE DATABASE [CUWebinarsClean] CONTAINMENT = NONE
 ON  PRIMARY ( NAME = N'CUWebinarsClean', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\CUWebinarsClean.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON ( NAME = N'CUWebinarsClean_log', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\CUWebinarsClean_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [CUWebinarsClean] SET COMPATIBILITY_LEVEL = 110
GO

IF ( 1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled') )
    BEGIN
        EXEC [CUWebinarsClean].[dbo].[sp_fulltext_database] @action = 'enable'
    END
GO

ALTER DATABASE [CUWebinarsClean] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET ARITHABORT OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [CUWebinarsClean] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [CUWebinarsClean] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [CUWebinarsClean] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET  DISABLE_BROKER 
GO

ALTER DATABASE [CUWebinarsClean] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [CUWebinarsClean] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET RECOVERY FULL 
GO

ALTER DATABASE [CUWebinarsClean] SET  MULTI_USER 
GO

ALTER DATABASE [CUWebinarsClean] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [CUWebinarsClean] SET DB_CHAINING OFF 
GO

ALTER DATABASE [CUWebinarsClean] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF
 ) 
GO

ALTER DATABASE [CUWebinarsClean] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [CUWebinarsClean] SET  READ_WRITE 
GO
PRINT '-------------------------------------------------'
PRINT '---------Building CUWebinarsClean Objects------------------'
PRINT '-------------------------------------------------'


USE [CUWebinarsClean]
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
	
----    SET IDENTITY_INSERT CUWebinarsClean.dbo.OptionsGroupsXref ON
--    DECLARE my_Cursor CURSOR
--    FOR
--    SELECT  w.idWebinar ,
--            idOptionGroup
--    FROM    CUWebinarsClean.dbo.Webinar w
--            INNER JOIN TTSWebinars2.dbo.OptionsGroupsXref ox ON ox.idWebinar = w.idWebinar
--    WHERE   w.status < 4
--            AND w.status > 1
--        --AND w.idWebinar NOT IN ( 1589, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1595, 1596, 1597, 1598, 1599, 1600, 1601,
--        --                         1602, 1603 )
--            AND w.idWebinar IN ( SELECT idWebinar
--                                 FROM   CUWebinarsClean.dbo.Webinar )
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
--                INSERT  CUWebinarsClean.dbo.OptionsGroupsXref
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
--    --SET IDENTITY_INSERT CUWebinarsClean.dbo.RegTypesGroupsXref OFF

--END
--go


CREATE TABLE [CUWebinarsClean].dbo.[__MigrationHistory]
    (
      [MigrationId] [NVARCHAR](150) NOT NULL ,
      [ContextKey] [NVARCHAR](300) NOT NULL ,
      [Model] [VARBINARY](MAX) NOT NULL ,
      [ProductVersion] [NVARCHAR](32) NOT NULL ,
      CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED ( [MigrationId] ASC, [ContextKey] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AdditionalLocations]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[AdditionalLocations]
    (
      [Id] [INT] IDENTITY(1, 1)
                 NOT NULL ,
      [Email] [NVARCHAR](150) NOT NULL ,
      [FullName] [NVARCHAR](100) NULL ,
      [idOrderRowOption] [INT] NOT NULL ,
      CONSTRAINT [PK_dbo.AdditionalLocations] PRIMARY KEY CLUSTERED ( [Id] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[Addresses]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Addresses]
    (
      [Id] [INT] IDENTITY(1, 1)
                 NOT NULL ,
      [AddressType] [NVARCHAR](MAX) NULL ,
      [Name] [NVARCHAR](MAX) NULL ,
      [Phone] [NVARCHAR](MAX) NULL ,
      [StreetAddress] [NVARCHAR](MAX) NULL ,
      [StreetAddress2] [NVARCHAR](MAX) NULL ,
      [City] [NVARCHAR](MAX) NULL ,
      [Zip] [NVARCHAR](MAX) NULL ,
      [State] [NVARCHAR](MAX) NULL ,
      [Country] [NVARCHAR](MAX) NULL ,
      [idUser] [INT] NOT NULL ,
      CONSTRAINT [PK_dbo.Addresses] PRIMARY KEY CLUSTERED ( [Id] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Affiliate]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Affiliate]
    (
      [idUserAff] [INT] NOT NULL ,
      [CommissionModel] [TINYINT] NOT NULL ,
      [URL] [NVARCHAR](500) NULL ,
      [WebBanner] [NVARCHAR](4000) NOT NULL ,
      [WebFooter] [NVARCHAR](4000) NOT NULL ,
      [EmailBanner] [NVARCHAR](4000) NOT NULL ,
      [EmailFooter] [NVARCHAR](4000) NOT NULL ,
      [ttsDomain] [NVARCHAR](50) NULL ,
      [GAPass] [NVARCHAR](50) NULL ,
      [supportEmail] [NVARCHAR](75) NULL ,
      [DisplayTitle] [NVARCHAR](150) NULL ,
      [BillingModel] [NVARCHAR](150) NULL ,
      [Logo] [NVARCHAR](150) NULL ,
      [ContactPerson] [NVARCHAR](150) NULL ,
      [ContactPhone] [NVARCHAR](150) NULL ,
      [ContactEmail] [NVARCHAR](150) NULL ,
      [ContactFax] [NVARCHAR](150) NULL ,
      [ContactAddress] [NVARCHAR](150) NULL ,
      [TechEmail] [NVARCHAR](150) NULL ,
      [TechPhone] [NVARCHAR](150) NULL ,
      [TechName] [NVARCHAR](150) NULL ,
      [EmailPromo] [NVARCHAR](50) NULL ,
      [WebUser_Id] [INT] NULL ,
      CONSTRAINT [PK_dbo.Affiliate] PRIMARY KEY CLUSTERED ( [idUserAff] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[Discounts]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Discounts]
    (
      [idDiscounts] [INT] IDENTITY(1, 1)
                          NOT NULL ,
      [DiscountType] [INT] NOT NULL ,
      [DiscountCode] [NVARCHAR](50) NOT NULL ,
      [PercentOff] [DECIMAL](18, 2) NOT NULL ,
      [FlatOff] [DECIMAL](18, 2) NOT NULL ,
      [usesNumber] [INT] NOT NULL ,
      [dateValidFrom] [DATETIME] NOT NULL ,
      [dateValidTo] [DATETIME] NOT NULL ,
      [status] [NVARCHAR](50) NOT NULL ,
      [dateBilled] [DATETIME] NULL ,
      [cost] [DECIMAL](18, 2) NULL ,
      [Notes] [NVARCHAR](MAX) NULL ,
      CONSTRAINT [PK_dbo.Discounts] PRIMARY KEY CLUSTERED ( [idDiscounts] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[ErrorLog]
    (
      [ErrorLogID] [INT] IDENTITY(1, 1)
                         NOT NULL ,
      [ErrorTime] [DATETIME] NOT NULL ,
      [UserName] [sysname] NOT NULL ,
      [ErrorNumber] [INT] NOT NULL ,
      [ErrorSeverity] [INT] NULL ,
      [ErrorState] [INT] NULL ,
      [ErrorProcedure] [NVARCHAR](126) NULL ,
      [ErrorLine] [INT] NULL ,
      [ErrorMessage] [NVARCHAR](4000) NOT NULL ,
      CONSTRAINT [PK_ErrorLog_ErrorLogID] PRIMARY KEY CLUSTERED ( [ErrorLogID] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[Institution]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Institution]
    (
      [idInstitution] [INT] IDENTITY(1, 1)
                            NOT NULL ,
      [InstitutionName] [NVARCHAR](250) NOT NULL ,
      [InstitutionType] [NVARCHAR](20) NOT NULL ,
      [domainName] [NVARCHAR](75) NULL ,
      [RegIdentifier] [NVARCHAR](40) NULL ,
      [Address] [NVARCHAR](MAX) NULL ,
      [City] [NVARCHAR](MAX) NULL ,
      [State] [NVARCHAR](MAX) NULL ,
      [Zip] [NVARCHAR](MAX) NULL ,
      CONSTRAINT [PK_dbo.Institution] PRIMARY KEY CLUSTERED ( [idInstitution] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Options]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Options]
    (
      [idOption] [INT] IDENTITY(1, 1)
                       NOT NULL ,
      [OptionExplain] [NVARCHAR](MAX) NULL ,
      [OptionLabel] [NVARCHAR](200) NULL ,
      [PriceToAdd] [FLOAT] NULL ,
      [TaxExempt] [BIT] NULL ,
      [PercToAdd] [FLOAT] NULL ,
      [SortOrder] [INT] NULL ,
      [Type] [NVARCHAR](32) NOT NULL ,
      [MsgConfirm] [NVARCHAR](10) NULL ,
      [SKU] [NVARCHAR](150) NULL ,
      [ShowLiveNotifications] [NVARCHAR](MAX) NULL ,
      [ShowRecordingNotifications] [NVARCHAR](MAX) NULL ,
      [ShowShippedNotifications] [NVARCHAR](MAX) NULL ,
      [Stage1CheckoutConfirmationMsg] [NVARCHAR](MAX) NULL ,
      [Stage2CheckoutConfirmationMsg] [NVARCHAR](MAX) NULL ,
      [Stage1EmailConfirmationMsg] [NVARCHAR](MAX) NULL ,
      [Stage2EmailConfirmationMsg] [NVARCHAR](MAX) NULL ,
      CONSTRAINT [PK_dbo.Options] PRIMARY KEY CLUSTERED ( [idOption] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OptionsGroups]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[OptionsGroups]
    (
      [idOptionGroup] [INT] IDENTITY(1, 1)
                            NOT NULL ,
      [OptionGroupDesc] [NVARCHAR](50) NULL ,
      [OptionType] [NVARCHAR](1) NULL ,
      [SortOrder] [INT] NULL ,
      CONSTRAINT [PK_dbo.OptionsGroups] PRIMARY KEY CLUSTERED ( [idOptionGroup] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[OptionsGroupsXref]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[OptionsGroupsXref]
    (
      [idWebinarOptionGroup] [INT] IDENTITY(1, 1)
                                   NOT NULL ,
      [idWebinar] [INT] NOT NULL ,
      [idOptionGroup] [INT] NOT NULL ,
      CONSTRAINT [PK_dbo.OptionsGroupsXref] PRIMARY KEY CLUSTERED ( [idWebinarOptionGroup] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[OptionsXref]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[OptionsXref]
    (
      [idOptionsXref] [INT] IDENTITY(1, 1)
                            NOT NULL ,
      [idOptionGroup] [INT] NOT NULL ,
      [idOption] [INT] NOT NULL ,
      CONSTRAINT [PK_dbo.OptionsXref] PRIMARY KEY CLUSTERED ( [idOptionsXref] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderRow]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[OrderRow]
    (
      [idOrderRow] [INT] IDENTITY(1, 1)
                         NOT NULL ,
      [idOrder] [INT] NOT NULL ,
      [idWebinar] [INT] NOT NULL ,
      [UnitPrice] [DECIMAL](18, 2) NOT NULL ,
      [RowPrice] [DECIMAL](18, 2) NOT NULL ,
      [idDiscount] [INT] NULL ,
      [AlternateEmail] [NVARCHAR](100) NOT NULL ,
      [RegistrationType] [INT] NOT NULL ,
      [Status] [INT] NOT NULL ,
      [ShipmentDate] [DATETIME] NULL ,
      [isAdHocRecording] [BIT] NOT NULL ,
      [Royalty] [DECIMAL](18, 2) NULL ,
      CONSTRAINT [PK_dbo.OrderRow] PRIMARY KEY CLUSTERED ( [idOrderRow] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderRowOptions]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[OrderRowOptions]
    (
      [idOrderRowOption] [INT] IDENTITY(1, 1)
                               NOT NULL ,
      [idOrderRow] [INT] NOT NULL ,
      [idOption] [INT] NOT NULL ,
      [OptionPrice] [DECIMAL](18, 2) NOT NULL ,
      [OptionDescription] [NVARCHAR](255) NOT NULL ,
      [TaxExempt] [BIT] NOT NULL ,
      [Type] [NVARCHAR](32) NOT NULL ,
      CONSTRAINT [PK_dbo.OrderRowOptions] PRIMARY KEY CLUSTERED ( [idOrderRowOption] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[Orders]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Orders]
    (
      [idOrder] [INT] IDENTITY(1, 1)
                      NOT NULL ,
      [idUser] [INT] NOT NULL ,
      [idAffiliate] [INT] NOT NULL ,
      [OrderDate] [DATETIME] NOT NULL ,
      [Total] [DECIMAL](18, 2) NOT NULL ,
      [FirstName] [NVARCHAR](100) NULL ,
      [LastName] [NVARCHAR](100) NULL ,
      [Institution] [NVARCHAR](250) NULL ,
      [BillingPhone] [NVARCHAR](30) NULL ,
      [BillingEmail] [NVARCHAR](150) NULL ,
      [BillingAddress] [NVARCHAR](100) NULL ,
      [BillingAddress2] [NVARCHAR](100) NULL ,
      [BillingCity] [NVARCHAR](100) NULL ,
      [BillingState] [NVARCHAR](100) NULL ,
      [BillingZip] [NVARCHAR](20) NULL ,
      [ShippingFirstName] [NVARCHAR](100) NULL ,
      [ShippingLastName] [NVARCHAR](100) NULL ,
      [ShippingPhone] [NVARCHAR](30) NULL ,
      [ShippingAddress] [NVARCHAR](100) NULL ,
      [ShippingAddress2] [NVARCHAR](100) NULL ,
      [ShippingCity] [NVARCHAR](100) NULL ,
      [ShippingState] [NVARCHAR](100) NULL ,
      [ShippingZip] [NVARCHAR](20) NULL ,
      [PaymentType] [TINYINT] NOT NULL ,
      [UserComments] [NVARCHAR](2550) NULL ,
      [AuditInfo] [NVARCHAR](MAX) NULL ,
      [AffiliateComments] [NVARCHAR](MAX) NULL ,
      [AdminComments] [NVARCHAR](MAX) NULL ,
      [TaxExempt] [BIT] NOT NULL ,
      [InitiatedBy] [TINYINT] NOT NULL ,
      [PaidByCCNumber] [NVARCHAR](40) NULL ,
      [Origin] [NVARCHAR](MAX) NULL ,
      CONSTRAINT [PK_dbo.Orders] PRIMARY KEY CLUSTERED ( [idOrder] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Presenter]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Presenter]
    (
      [idUser] [INT] NOT NULL ,
      [Biography] [NVARCHAR](2500) NULL ,
      [BiographyLong] [NVARCHAR](2500) NOT NULL ,
      [PhotoFull] [NVARCHAR](400) NULL ,
      [PhotoThumb] [NVARCHAR](400) NULL ,
      CONSTRAINT [PK_dbo.Presenter] PRIMARY KEY CLUSTERED ( [idUser] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[Topic]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Topic]
    (
      [idTopic] [INT] IDENTITY(1, 1)
                      NOT NULL ,
      [topicDesc] [NVARCHAR](50) NOT NULL ,
      [idParentTopic] [INT] NULL ,
      [topicHTML] [NVARCHAR](255) NOT NULL ,
      [sortOrder] [INT] NOT NULL ,
      CONSTRAINT [PK_dbo.Topic] PRIMARY KEY CLUSTERED ( [idTopic] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[Webinar]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[Webinar]
    (
      [idWebinar] [INT] IDENTITY(1, 1)
                        NOT NULL ,
      [Description] [NVARCHAR](MAX) NOT NULL ,
      [DescriptionLong] [NVARCHAR](MAX) NOT NULL ,
      [ImageUrl] [NVARCHAR](50) NULL ,
      [SmallImageUrl] [NVARCHAR](50) NULL ,
      [Status] [INT] NOT NULL ,
      [Title] [NVARCHAR](125) NOT NULL ,
      [Date] [DATETIME] NOT NULL ,
      [LearnCaption] [NVARCHAR](255) NOT NULL ,
      [LearnBody] [NVARCHAR](MAX) NOT NULL ,
      [WhoAttend] [NVARCHAR](MAX) NOT NULL ,
      [Duration] [DECIMAL](18, 2) NOT NULL ,
      [RecordingUrl] [NVARCHAR](300) NOT NULL ,
      [idPresenter] [INT] NOT NULL ,
      [AdditionalNotifications] [NVARCHAR](MAX) NULL ,
      [ceu] [NVARCHAR](1000) NULL ,
      [ConnectionInfo] [NVARCHAR](MAX) NULL ,
      [DateCreated] [DATETIME] NOT NULL ,
      [DateChanged] [DATETIME] NOT NULL ,
      CONSTRAINT [PK_dbo.Webinar] PRIMARY KEY CLUSTERED ( [idWebinar] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WebinarFile]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[WebinarFile]
    (
      [idWebinarFile] [INT] IDENTITY(1, 1)
                            NOT NULL ,
      [idWebinar] [INT] NOT NULL ,
      [fileLocation] [NVARCHAR](255) NOT NULL ,
      [fileDesc] [NVARCHAR](1000) NOT NULL ,
      CONSTRAINT [PK_dbo.WebinarFile] PRIMARY KEY CLUSTERED ( [idWebinarFile] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[WebinarTopicXref]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[WebinarTopicXref]
    (
      [idWebinarTopicXref] [INT] IDENTITY(1, 1)
                                 NOT NULL ,
      [idWebinar] [INT] NOT NULL ,
      [idTopic] [INT] NOT NULL ,
      CONSTRAINT [PK_dbo.WebinarTopicXref] PRIMARY KEY CLUSTERED ( [idWebinarTopicXref] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]

GO
/****** Object:  Table [dbo].[WebUser]    Script Date: 7/5/2014 6:33:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CUWebinarsClean].dbo.[WebUser]
    (
      [idUser] [INT] NOT NULL ,
      [UserType] [INT] NOT NULL ,
      [AcctStatus] [NVARCHAR](1) NOT NULL ,
      [DateCreated] [DATETIME] NOT NULL ,
      [FirstName] [NVARCHAR](50) NOT NULL ,
      [LastName] [NVARCHAR](50) NOT NULL ,
      [Initial] [NVARCHAR](MAX) NULL ,
      [idUserInstitution] [INT] NOT NULL ,
      [email] [NVARCHAR](150) NOT NULL ,
      [futureMail] [NVARCHAR](1) NULL ,
      [generalComments] [NVARCHAR](1000) NULL ,
      [taxExempt] [BIT] NULL ,
      [idSubscriptionDiscount] [INT] NULL ,
      [timeZone] [INT] NOT NULL ,
      [Title] [NVARCHAR](200) NULL ,
      CONSTRAINT [PK_dbo.WebUser] PRIMARY KEY CLUSTERED ( [idUser] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ErrorLog] ADD  CONSTRAINT [DF_ErrorLog_ErrorTime]  DEFAULT (GETDATE()) FOR [ErrorTime]
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
PRINT '---EXEC CUWebinarsMigrator.dbo._SeedCUWebinars----------------------------------------------'
PRINT '-------------------------------------------------'
EXEC CUWebinarsMigrator.dbo._SeedCUWebinars

--SET IDENTITY_INSERT CUWebinars.dbo.RegType ON 

