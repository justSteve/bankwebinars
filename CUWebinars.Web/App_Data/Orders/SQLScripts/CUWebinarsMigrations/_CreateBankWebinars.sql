

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'CUWebinars'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--Use this to see results
SELECT @SQL 
--Uncomment this to run it
EXEC(@SQL)

PRINT '-------------------------------------------------'
PRINT '---DROP & RECREATE CUWebinars----------------------------------------------'
PRINT '-------------------------------------------------'


/****** Object:  Database [CUWebinars]    Script Date: 7/7/2014 7:51:40 AM ******/
DROP DATABASE [CUWebinars]
GO
USE [master]
GO

/****** Object:  Database [CUWebinars]    Script Date: 7/7/2014 7:52:03 AM ******/
CREATE DATABASE [CUWebinars]
 CONTAINMENT = NONE
 ON  PRIMARY 

( NAME = N'CUWebinars_Data', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\CUWebinars.mdf' , SIZE = 14400KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CUWebinars_Log', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\DATA\CUWebinars.ldf' , SIZE = 20096KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 COLLATE  Latin1_General_CI_AS
GO
 ALTER DATABASE [CUWebinars] SET MULTI_USER WITH ROLLBACK IMMEDIATE

ALTER DATABASE [CUWebinars] SET COMPATIBILITY_LEVEL = 110
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CUWebinars].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO

ALTER DATABASE [CUWebinars] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [CUWebinars] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [CUWebinars] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [CUWebinars] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [CUWebinars] SET ARITHABORT OFF 
GO

ALTER DATABASE [CUWebinars] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [CUWebinars] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [CUWebinars] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [CUWebinars] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [CUWebinars] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [CUWebinars] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [CUWebinars] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [CUWebinars] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [CUWebinars] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [CUWebinars] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [CUWebinars] SET  DISABLE_BROKER 
GO

ALTER DATABASE [CUWebinars] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [CUWebinars] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [CUWebinars] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [CUWebinars] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO

ALTER DATABASE [CUWebinars] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [CUWebinars] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [CUWebinars] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [CUWebinars] SET RECOVERY FULL 
GO

ALTER DATABASE [CUWebinars] SET  MULTI_USER 
GO

ALTER DATABASE [CUWebinars] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [CUWebinars] SET DB_CHAINING OFF 
GO

ALTER DATABASE [CUWebinars] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [CUWebinars] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [CUWebinars] SET  READ_WRITE 
GO



/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table CUWebinars.dbo.[__MigrationHistory](
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
Create table CUWebinars.dbo.[AdditionalLocation](
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
Create table CUWebinars.dbo.[Address](
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


CREATE TABLE CUWebinars.[dbo].[AdditionalLocationsLookupPrice]
    (
      [idWebinar] [INT] NOT NULL ,
      [id] [INT] IDENTITY(1, 1)
                 NOT NULL ,
      [cost] [MONEY] NOT NULL ,
      CONSTRAINT [PK_AdditionalLocationsLookupPrice] PRIMARY KEY CLUSTERED
        ( [id] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,
               IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]


GO
/****** Object:  Table [dbo].[Affiliate]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table CUWebinars.dbo.[Affiliate](
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

--updated Discount table with FK added
CREATE TABLE CUWebinars.[dbo].[Discount](
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
	[renewalTerm] [int] NULL,
 CONSTRAINT [PK_dbo.Discount] PRIMARY KEY CLUSTERED 
(
	[idDiscount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

--GO
--/****** Object:  Table [dbo].[Discount]    Script Date: 7/7/2014 7:53:31 AM ******/

--Create table CUWebinars.dbo.[Discount](
--	[idDiscount] [int] IDENTITY(1,1) NOT NULL,
--	[DiscountType] [int] NOT NULL,
--	[DiscountCode] [nvarchar](50) NOT NULL,
--	[PercentOff] [decimal](18, 2) NULL,
--	[FlatOff] [decimal](18, 2) NULL,
--	[UsesCount] [int] NOT NULL,
--	[UsesRemain] [int] NOT NULL,
--	[DateValidFrom] [datetime] NOT NULL,
--	[DateValidTo] [datetime] NOT NULL,
--	[Status] [nvarchar](50) NOT NULL,
--	[DateBilled] [datetime] NULL,
--	[Cost] [decimal](18, 2) NULL,
--	[Notes] [nvarchar](max) NULL,
-- CONSTRAINT [PK_dbo.Discount] PRIMARY KEY CLUSTERED 
--(
--	[idDiscount] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
--)


CREATE TABLE CUWebinars.[dbo].[WebUserDiscountXref](
	[idWebUserDiscount] [int] IDENTITY(1,1) NOT NULL,
	[idWebUser] [int] NOT NULL,
	[idDiscount] [int] NOT NULL,
 CONSTRAINT [PK_WebUserDiscount] PRIMARY KEY CLUSTERED 
(
	[idWebUserDiscount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

GO
/****** Object:  Table [dbo].[Institution]    Script Date: 7/7/2014 7:53:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create table CUWebinars.dbo.[Institution](
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
Create table CUWebinars.dbo.[Order](
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
Create table CUWebinars.dbo.[OrderRow](
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
Create table CUWebinars.dbo.[Presenter](
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
Create table CUWebinars.dbo.[RegType](
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
Create table CUWebinars.dbo.[RegTypesGroups](
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
Create table CUWebinars.dbo.[RegTypesGroupsXref](
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
Create table CUWebinars.dbo.[RegTypesXref](
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
Create table CUWebinars.dbo.[Topic](
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
Create table CUWebinars.dbo.[Webinar](
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
	[CitrixRegisterUrl] [nvarchar](500) NULL,
	[AccessPhone] [nvarchar](50) NULL,
	[AccessCodeAttendee] [nvarchar](50) NULL,
	[AccessCodePresenter] [nvarchar](50) NULL,
	[AccessCodeOrganizer] [nvarchar](50) NULL
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
Create table CUWebinars.dbo.[WebinarFile](
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
Create table CUWebinars.dbo.[WebinarTopicXref](
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
Create table CUWebinars.dbo.[WebUser](
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
CREATE NONCLUSTERED INDEX [IX_idOrderRow] ON CUWebinars.[dbo].[AdditionalLocation]
(
	[idOrderRow] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUser]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON CUWebinars.[dbo].[Address]
(
	[idUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUserAff]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUserAff] ON CUWebinars.[dbo].[Affiliate]
(
	[idUserAff] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idAffiliate]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idAffiliate] ON CUWebinars.[dbo].[Order]
(
	[idAffiliate] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUser]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON CUWebinars.[dbo].[Order]
(
	[idUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_Discount_idDiscount]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_Discount_idDiscount] ON CUWebinars.[dbo].[OrderRow]
(
	[Discount_idDiscount] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idOrder]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idOrder] ON CUWebinars.[dbo].[OrderRow]
(
	[idOrder] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idRegType]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idRegType] ON CUWebinars.[dbo].[OrderRow]
(
	[idRegType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON CUWebinars.[dbo].[OrderRow]
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUser]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON CUWebinars.[dbo].[Presenter]
(
	[idUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idRegTypeGroup]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idRegTypeGroup] ON CUWebinars.[dbo].[RegTypesGroupsXref]
(
	[idRegTypeGroup] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON CUWebinars.[dbo].[RegTypesGroupsXref]
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idRegType]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idRegType] ON CUWebinars.[dbo].[RegTypesXref]
(
	[idRegType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idRegTypeGroup]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idRegTypeGroup] ON CUWebinars.[dbo].[RegTypesXref]
(
	[idRegTypeGroup] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idParentTopic]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idParentTopic] ON CUWebinars.[dbo].[Topic]
(
	[idParentTopic] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idPresenter]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idPresenter] ON CUWebinars.[dbo].[Webinar]
(
	[idPresenter] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON CUWebinars.[dbo].[WebinarFile]
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idTopic]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idTopic] ON CUWebinars.[dbo].[WebinarTopicXref]
(
	[idTopic] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON CUWebinars.[dbo].[WebinarTopicXref]
(
	[idWebinar] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_idUserInstitution]    Script Date: 7/7/2014 7:53:31 AM ******/
CREATE NONCLUSTERED INDEX [IX_idUserInstitution] ON CUWebinars.[dbo].[WebUser]
(
	[idUserInstitution] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
ALTER TABLE CUWebinars.[dbo].[AdditionalLocation]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdditionalLocation_dbo.OrderRow_idOrderRow] FOREIGN KEY([idOrderRow])
REFERENCES CUWebinars.[dbo].[OrderRow] ([idOrderRow])
ON DELETE CASCADE
GO
ALTER TABLE CUWebinars.[dbo].[AdditionalLocation] CHECK CONSTRAINT [FK_dbo.AdditionalLocation_dbo.OrderRow_idOrderRow]
GO
ALTER TABLE CUWebinars.[dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Address_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES CUWebinars.[dbo].[WebUser] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE CUWebinars.[dbo].[Address] CHECK CONSTRAINT [FK_dbo.Address_dbo.WebUser_idUser]
GO
ALTER TABLE CUWebinars.[dbo].[Affiliate]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Affiliate_dbo.WebUser_idUserAff] FOREIGN KEY([idUserAff])
REFERENCES CUWebinars.[dbo].[WebUser] ([idUser])
GO
ALTER TABLE CUWebinars.[dbo].[Affiliate] CHECK CONSTRAINT [FK_dbo.Affiliate_dbo.WebUser_idUserAff]
GO
ALTER TABLE CUWebinars.[dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Order_dbo.Affiliate_idAffiliate] FOREIGN KEY([idAffiliate])
REFERENCES CUWebinars.[dbo].[Affiliate] ([idUserAff])
ON DELETE CASCADE
GO
ALTER TABLE CUWebinars.[dbo].[Order] CHECK CONSTRAINT [FK_dbo.Order_dbo.Affiliate_idAffiliate]
GO
ALTER TABLE CUWebinars.[dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Order_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES CUWebinars.[dbo].[WebUser] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE CUWebinars.[dbo].[Order] CHECK CONSTRAINT [FK_dbo.Order_dbo.WebUser_idUser]
GO
ALTER TABLE CUWebinars.[dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.Discount_Discount_idDiscount] FOREIGN KEY([Discount_idDiscount])
REFERENCES CUWebinars.[dbo].[Discount] ([idDiscount])
GO
ALTER TABLE CUWebinars.[dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.Discount_Discount_idDiscount]
GO
ALTER TABLE CUWebinars.[dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.Order_idOrder] FOREIGN KEY([idOrder])
REFERENCES CUWebinars.[dbo].[Order] ([idOrder])
ON DELETE CASCADE
GO
ALTER TABLE CUWebinars.[dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.Order_idOrder]
GO
ALTER TABLE CUWebinars.[dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.RegType_idRegType] FOREIGN KEY([idRegType])
REFERENCES CUWebinars.[dbo].[RegType] ([idRegType])
ON DELETE CASCADE
GO
ALTER TABLE CUWebinars.[dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.RegType_idRegType]
GO
ALTER TABLE CUWebinars.[dbo].[OrderRow]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderRow_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES CUWebinars.[dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[OrderRow] CHECK CONSTRAINT [FK_dbo.OrderRow_dbo.Webinar_idWebinar]
GO
ALTER TABLE [CUWebinars].[dbo].[Presenter]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Presenter_dbo.WebUser_idUser] FOREIGN KEY([idUser])
REFERENCES [CUWebinars].[dbo].[WebUser] ([idUser])
GO
ALTER TABLE [CUWebinars].[dbo].[Presenter] CHECK CONSTRAINT [FK_dbo.Presenter_dbo.WebUser_idUser]
GO
ALTER TABLE [CUWebinars].[dbo].[RegTypesGroupsXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RegTypesGroupsXref_dbo.RegTypesGroups_idRegTypeGroup] FOREIGN KEY([idRegTypeGroup])
REFERENCES [CUWebinars].[dbo].[RegTypesGroups] ([idRegTypeGroup])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[RegTypesGroupsXref] CHECK CONSTRAINT [FK_dbo.RegTypesGroupsXref_dbo.RegTypesGroups_idRegTypeGroup]
GO
ALTER TABLE [CUWebinars].[dbo].[RegTypesGroupsXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RegTypesGroupsXref_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [CUWebinars].[dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[RegTypesGroupsXref] CHECK CONSTRAINT [FK_dbo.RegTypesGroupsXref_dbo.Webinar_idWebinar]
GO
ALTER TABLE [CUWebinars].[dbo].[RegTypesXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RegTypesXref_dbo.RegType_idRegType] FOREIGN KEY([idRegType])
REFERENCES [CUWebinars].[dbo].[RegType] ([idRegType])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[RegTypesXref] CHECK CONSTRAINT [FK_dbo.RegTypesXref_dbo.RegType_idRegType]
GO
ALTER TABLE [CUWebinars].[dbo].[RegTypesXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RegTypesXref_dbo.RegTypesGroups_idRegTypeGroup] FOREIGN KEY([idRegTypeGroup])
REFERENCES [CUWebinars].[dbo].[RegTypesGroups] ([idRegTypeGroup])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[RegTypesXref] CHECK CONSTRAINT [FK_dbo.RegTypesXref_dbo.RegTypesGroups_idRegTypeGroup]
GO
ALTER TABLE [CUWebinars].[dbo].[Topic]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Topic_dbo.Topic_idParentTopic] FOREIGN KEY([idParentTopic])
REFERENCES [CUWebinars].[dbo].[Topic] ([idTopic])
GO
ALTER TABLE [CUWebinars].[dbo].[Topic] CHECK CONSTRAINT [FK_dbo.Topic_dbo.Topic_idParentTopic]
GO
ALTER TABLE [CUWebinars].[dbo].[Webinar]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Webinar_dbo.Presenter_idPresenter] FOREIGN KEY([idPresenter])
REFERENCES [CUWebinars].[dbo].[Presenter] ([idUser])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[Webinar] CHECK CONSTRAINT [FK_dbo.Webinar_dbo.Presenter_idPresenter]
GO
ALTER TABLE [CUWebinars].[dbo].[WebinarFile]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarFile_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [CUWebinars].[dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[WebinarFile] CHECK CONSTRAINT [FK_dbo.WebinarFile_dbo.Webinar_idWebinar]
GO
ALTER TABLE [CUWebinars].[dbo].[WebinarTopicXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Topic_idTopic] FOREIGN KEY([idTopic])
REFERENCES [CUWebinars].[dbo].[Topic] ([idTopic])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[WebinarTopicXref] CHECK CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Topic_idTopic]
GO
ALTER TABLE [CUWebinars].[dbo].[WebinarTopicXref]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Webinar_idWebinar] FOREIGN KEY([idWebinar])
REFERENCES [CUWebinars].[dbo].[Webinar] ([idWebinar])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[WebinarTopicXref] CHECK CONSTRAINT [FK_dbo.WebinarTopicXref_dbo.Webinar_idWebinar]
GO
ALTER TABLE [CUWebinars].[dbo].[WebUser]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebUser_dbo.Institution_idUserInstitution] FOREIGN KEY([idUserInstitution])
REFERENCES [CUWebinars].[dbo].[Institution] ([idInstitution])
ON DELETE CASCADE
GO
ALTER TABLE [CUWebinars].[dbo].[WebUser] CHECK CONSTRAINT [FK_dbo.WebUser_dbo.Institution_idUserInstitution]
GO


ALTER TABLE CUWebinars.[dbo].[WebUserDiscountXref]  WITH CHECK ADD  CONSTRAINT [FK_WebUserDiscountXref_Discount] FOREIGN KEY([idDiscount])
REFERENCES CUWebinars.[dbo].[Discount] ([idDiscount])
GO

ALTER TABLE CUWebinars.[dbo].[WebUserDiscountXref] CHECK CONSTRAINT [FK_WebUserDiscountXref_Discount]
GO

ALTER TABLE CUWebinars.[dbo].[WebUserDiscountXref]  WITH CHECK ADD  CONSTRAINT [FK_WebUserDiscountXref_WebUser] FOREIGN KEY([idWebUser])
REFERENCES CUWebinars.[dbo].[WebUser] ([idUser])
GO

ALTER TABLE CUWebinars.[dbo].[WebUserDiscountXref] CHECK CONSTRAINT [FK_WebUserDiscountXref_WebUser]
GO



USE CUWebinars
go
-- =============================================
-- Author:		sjh
-- Create date: 5/14
-- Description:	Updates webinar table with GTW connection info
-- =============================================
create PROCEDURE [dbo].[InsertGTWConnectionInfo] 
	-- Add the parameters for the stored procedure here
    @idWebinar INT, 
	@Status INT ,
    @WebinarKey NVARCHAR(MAX) ,
    @CitrixURL NVARCHAR(MAX) ,
    @AccessPhone NVARCHAR(MAX) ,
    @AccessCodeAttendee NVARCHAR(MAX) ,
    @AccessCodePresenter NVARCHAR(MAX) ,
    @AccessCodeOrganizer NVARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
    SET NOCOUNT ON;
	UPDATE [dbo].[Webinar]
   SET [Status] = 7
      ,[WebinarKey] = @WebinarKey
      ,[OrganizerKey] = '901873'
      ,[OrganizerOAuthKey] = '8SSOAgpL2WMxa4iUyQUn1h9bGEmJ' 
      ,[CitrixRegisterUrl] = @CitrixURL
      ,[AccessPhone] = @AccessPhone
      ,[AccessCodeAttendee] = @AccessCodeAttendee
      ,[AccessCodePresenter] = @AccessCodePresenter
      ,[AccessCodeOrganizer] = @AccessCodeOrganizer
	  WHERE idWebinar = @idWebinar

END
GO



ALTER TABLE CUWebinars.dbo.[Order]
ADD idOrderLegacy INT

ALTER TABLE CUWebinars.dbo.WebUser
ADD idUserLegacy INT


EXEC	CUWebinarsMigrator.[dbo].[Migrate_Runner]



GO
IF EXISTS (

SELECT idWebinar, Status, Title FROM CUWebinars.dbo.Webinar WHERE idWebinar NOT IN (SELECT idWebinar FROM  CUWebinars.dbo.WebinarTopicXref))
BEGIN
	PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'
	PRINT '           Missing topic IDs'
	PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'
	

SELECT idWebinar, Status, Title FROM CUWebinars.dbo.Webinar WHERE idWebinar NOT IN (SELECT idWebinar FROM  CUWebinars.dbo.WebinarTopicXref)
PRINT ''
PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'
PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'
PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'


END