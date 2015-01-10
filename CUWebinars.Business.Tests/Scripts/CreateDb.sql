USE [master]
GO
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'BankWebinars')
DROP DATABASE [BankWebinars];

CREATE DATABASE [BankWebinars]
ON (name='BankWebinars', filename='e:\csv\BankWebinars.mdf')
GO
USE [BankWebinars]
GO
ALTER DATABASE [BankWebinars] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BankWebinars].[dbo].[sp_fulltext_database] @action = 'enable'
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
USE [BankWebinars]
GO
--/****** Object:  User [TTSOp]    Script Date: 29/12/2014 4:03:48 PM ******/
--CREATE USER [TTSOp] FOR LOGIN [TTSOp] WITH DEFAULT_SCHEMA=[dbo]
--GO
--/****** Object:  User [Dave]    Script Date: 29/12/2014 4:03:48 PM ******/
--CREATE USER [Dave] FOR LOGIN [Dave] WITH DEFAULT_SCHEMA=[dbo]
--GO
--sys.sp_addrolemember @rolename = N'db_owner', @membername = N'TTSOp'
--GO
--sys.sp_addrolemember @rolename = N'db_owner', @membername = N'Dave'
--GO
/****** Object:  StoredProcedure [dbo].[InsertGTWConnectionInfo]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[InsertGTWConnectionInfo] 
	-- Add the parameters for the stored procedure here
    @idWebinar INT, 
	@Status INT ,
    @WebinarKey NVARCHAR(MAX) ,
    @OrganizerKey NVARCHAR(MAX) ,
    @OrganizerOAuthKey NVARCHAR(MAX) ,
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
      ,[OrganizerKey] = @OrganizerKey
      ,[OrganizerOAuthKey] = @OrganizerOAuthKey
      ,[CitrixRegisterUrl] = @CitrixURL
      ,[AccessPhone] = @AccessPhone
      ,[AccessCodeAttendee] = @AccessCodeAttendee
      ,[AccessCodePresenter] = @AccessCodePresenter
      ,[AccessCodeOrganizer] = @AccessCodeOrganizer
	  WHERE idWebinar = @idWebinar

END
GO

END
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
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
/****** Object:  Table [dbo].[AdditionalLocation]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdditionalLocation](
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AdditionalLocationsLookupPrice]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdditionalLocationsLookupPrice](
	[idWebinar] [int] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cost] [money] NOT NULL,
 CONSTRAINT [PK_AdditionalLocationsLookupPrice] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Address]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Affiliate]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Affiliate](
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Discount]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discount](
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

GO
/****** Object:  Table [dbo].[Institution]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Institution](
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
/****** Object:  Table [dbo].[Order]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
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
	[idOrderLegacy] [int] NULL,
 CONSTRAINT [PK_dbo.Order] PRIMARY KEY CLUSTERED 
(
	[idOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderRow]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderRow](
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Presenter]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Presenter](
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
/****** Object:  Table [dbo].[RegType]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegType](
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RegTypesGroups]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegTypesGroups](
	[idRegTypeGroup] [int] IDENTITY(1,1) NOT NULL,
	[RegTypeGroupDesc] [nvarchar](50) NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_dbo.RegTypesGroups] PRIMARY KEY CLUSTERED 
(
	[idRegTypeGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RegTypesGroupsXref]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegTypesGroupsXref](
	[idWebinarRegTypeGroup] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[idRegTypeGroup] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RegTypesGroupsXref] PRIMARY KEY CLUSTERED 
(
	[idWebinarRegTypeGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RegTypesXref]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegTypesXref](
	[idRegTypesXref] [int] IDENTITY(1,1) NOT NULL,
	[idRegTypeGroup] [int] NOT NULL,
	[idRegType] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RegTypesXref] PRIMARY KEY CLUSTERED 
(
	[idRegTypesXref] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Topic]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Topic](
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
/****** Object:  Table [dbo].[Webinar]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Webinar](
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
	[CitrixRegisterURL] [nvarchar](max) NULL,
	[AccessPhone] [nvarchar](max) NULL,
	[AccessCodeAttendee] [nvarchar](max) NULL,
	[AccessCodePresenter] [nvarchar](max) NULL,
	[AccessCodeOrganizer] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Webinar] PRIMARY KEY CLUSTERED 
(
	[idWebinar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WebinarFile]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebinarFile](
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
/****** Object:  Table [dbo].[WebinarTopicXref]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebinarTopicXref](
	[idWebinarTopicXref] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[idTopic] [int] NOT NULL,
 CONSTRAINT [PK_dbo.WebinarTopicXref] PRIMARY KEY CLUSTERED 
(
	[idWebinarTopicXref] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WebUser]    Script Date: 29/12/2014 4:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebUser](
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
	[idUserLegacy] [int] NULL,
 CONSTRAINT [PK_dbo.WebUser] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [master]
GO
ALTER DATABASE [BankWebinars] SET  READ_WRITE 
GO
USE [BankWebinars]
SET IDENTITY_INSERT [dbo].[Webinar] ON
INSERT  [dbo].[Webinar]
        ( [idWebinar] ,
          [Description] ,
          [DescriptionLong] ,
          [ImageUrl] ,
          [SmallImageUrl] ,
          [Status] ,
          [Title] ,
          [Date] ,
          [LearnCaption] ,
          [LearnBody] ,
          [WhoAttend] ,
          [Duration] ,
          [RecordingUrl] ,
          [idPresenter] ,
          [WebinarKey] ,
          [OrganizerKey] ,
          [OrganizerOAuthKey] ,
          [ceu] ,
          [ConnectionInfo] ,
          [DateCreated] ,
          [DateChanged]
        )
VALUES  ( 404 ,
          N'<p>Your member passed away yesterday and the family is grieving. Yet, in the midst of all the remembering and honoring of a life, the legal and compliance clock is ticking. In most states, checks can be paid for 10 days after the date of death.</p><p>When your account holder dies, many issues and questions arise. Who can have information on the account? Who owns the account? Who has access to the account? What happens with powers of attorney and authorized signers on account? Can the spouse of the sole proprietor continue to access the account? What about that last tax refund check? Is the account still insured? Can a check be written to a funeral home? What about the checks coming in to pay funeral expense? Learn about checks, IRAs, deposit ownership, trusts, UTMA, affidavits of heirship and other complex issues that can occur when a member dies. </p>' ,
          N'<p>Your member passed away yesterday and the family is grieving. Yet, in the midst of all the remembering and honoring of a life, the legal and compliance clock is ticking. In most states, checks can be paid for 10 days after the date of death.</p><p>When your account holder dies, many issues and questions arise. Who can have information on the account? Who owns the account? Who has access to the account? What happens with powers of attorney and authorized signers on account? Can the spouse of the sole proprietor continue to access the account? What about that last tax refund check? Is the account still insured? Can a check be written to a funeral home? What about the checks coming in to pay funeral expense? Learn about checks, IRAs, deposit ownership, trusts, UTMA, affidavits of heirship and other complex issues that can occur when a member dies. </p>' ,
          N'' ,
          N'' ,
          2 ,
          N'10 Lessons Learned When Your Member Dies' ,
          CAST(0x0000A34800A4CB80 AS DATETIME) ,
          N'Covered Topics' ,
          N'<ul><li>Probate versus non-probate transfers</li><li>When does the will govern and when does the signature card?</li><li>What bypasses a will? PODs, IRAs, JTWROS?</li><li>Living trusts and successor trustees</li><li>When do we know that a customer is deceased?</li><li>When can we use small estate affidavits?</li><li>What happens to powers of attorney, authorized signers, etc.</li><li>How are IRAs, HSAs and UTMAs affected?</li><li>Do sole proprietorships cease at death?</li><li>NCUSIF insurance issues</li></ul>' ,
          N'This informative session is designed for customer service representatives, branch administration, branch managers, tellers, training and development staff, compliance personnel, and anyone who handles customer accounts.' ,
          CAST(1.00 AS DECIMAL(18, 2)) ,
          N'' ,
          10568 ,
          NULL ,
          N'922930' ,
          N'5jxY3KZL48HWknOaOEP2eIzVmOTS' ,
          N'1.25 CE Credits|Recommended for 1.25 CE Credit Hours. After attending this webinar, each attendee can receive a Certificate of Attendance for self-reporting of CE Credits.' ,
          NULL ,
          CAST(0x0000A2EB01477614 AS DATETIME) ,
          CAST(0x0000A2EB01477614 AS DATETIME)
        )
INSERT  [dbo].[Webinar]
        ( [idWebinar] ,
          [Description] ,
          [DescriptionLong] ,
          [ImageUrl] ,
          [SmallImageUrl] ,
          [Status] ,
          [Title] ,
          [Date] ,
          [LearnCaption] ,
          [LearnBody] ,
          [WhoAttend] ,
          [Duration] ,
          [RecordingUrl] ,
          [idPresenter] ,
          [WebinarKey] ,
          [OrganizerKey] ,
          [OrganizerOAuthKey] ,
          [ceu] ,
          [ConnectionInfo] ,
          [DateCreated] ,
          [DateChanged] ,
		  [CitrixRegisterURL],
          [AccessPhone] ,
          [AccessCodeAttendee] ,
          [AccessCodePresenter] ,
          [AccessCodeOrganizer]
        )
VALUES  ( 1568 ,
          N'<p>Attend this webinar to know more about the ins and outs of resolutions and authorizations.</p>' ,
          N'<p>Some of our best and most profitable financial institution customers are legal entities. Unfortunately, establishing relationships with business entities can be confusing, and if done incorrectly, they can become costly liabilities. One of the most effective ways for a financial institution to protect itself from business entity liability is by obtaining a well-drafted, properly-executed resolution or similar authorization from the business. Attend this webinar to know more about the ins and outs of resolutions and authorizations.</p>' ,
          N'http://bankwebinars.com/content/images/bg.jpg' ,
          N'http://bankwebinars.com/content/images/bg.jpg' ,
          3 ,
          N'How to Use Business Resolutions and Authorizations to Protect Your Financial Institution' ,
          CAST(N'2014-06-26 10:00:00.000' AS DATETIME) ,
          N'Topics Will Include:' ,
          N'<ul>  <li>What is the purpose of a resolution or authorization?   </li><li>What language should be contained in a resolution or authorization?   </li><li>Who should sign a business entity resolution or authorization form in order to make it binding on the business?   </li><li>What are the differences between financial institution-prepared and customer-prepared resolutions and authorizations?   </li><li>What are the risks of accepting customer-prepared forms?</li></ul>' ,
          N'<p>Lenders and new account personnel responsible for loan and deposit account documentation.</p>' ,
          CAST(2.00 AS DECIMAL(18, 2)) ,
          N'BusResolutionsHM062614.wmv' ,
          10568 ,
          N'0' ,
          N'0' ,
          N'' ,
          N'2.5 CE Credits|Recommended for 2.5 CE Credit Hours. After attending this webinar, each attendee can receive a Certificate of Attendance for self-reporting of CE Credits.' ,
          N'' ,
          CAST(N'2014-03-13 14:25:00.000' AS DATETIME) ,
          CAST(N'2014-03-13 14:27:00.000' AS DATETIME) ,
          NULL ,
          NULL ,
          NULL ,
          NULL , null
        )

SET IDENTITY_INSERT [dbo].[Webinar] OFF
INSERT [dbo].[Affiliate] ([idUserAff], [CommissionModel], [URL], [WebBanner], [WebFooter], [EmailBanner], [EmailFooter], [ttsDomain], [GAPass], [supportEmail], [DisplayTitle], [BillingModel], [Logo], [ContactPerson], [ContactPhone], [ContactEmail], [ContactFax], [ContactAddress], [TechEmail], [TechPhone], [TechName], [EmailPromo]) VALUES (19, 1, N'http://www.ttstrain.com/', N'
		<div class="container" id="header" style="margin-bottom: 0;">
			<table cellpadding="0" cellspacing="0">
				<tr>
					<td style="height: 77px; width: 550px; background-image: url(/Content/images/BWBannerLeft.jpg);"></td>
					<td style="height: 77px; width: 620px; background-image: url(/Content/images/BannerGradientBack.jpg);">
						<div class="bannerRight aligncenter">
							<p align="center" style="font-size: larger; color: #1F5B93;">
								<b><i>Providing premier web-based seminars featuring<br />
									the best speakers in the financial industry since 2002. </i></b>
							</p>
						</div>
					</td>
				</tr>
			</table>
		</div>
', N'        <div class="container">
	<div class="row" style="background-color: #E7F1FA; color: lightsteelblue; margin-left: 2px; background-repeat: repeat-x; background-image: url(/Content/images/BannerGradientBottomBack.jpg);">
		<div class="span3">
			<h4 align="center" style="color: #1F5B93; margin-bottom: 0px;">Find us</h4>
			<p align="center">
				<a href="http://www.linkedin.com/company/bankwebinars.com" style="text-decoration: underline" target="_blank">
					<img alt="LinkedIn" style="padding: 0px 0px 5px; vertical-align: middle" src="http://images.wisestamp.com/linkedin.png" height="16" width="16"></a>
				<a href="http://www.facebook.com/home.php#%21/pages/Total-Training-Solutions/300546163412" style="text-decoration: underline" target="_blank">
					<img alt="Facebook" style="padding: 0px 0px 5px; vertical-align: middle" src="http://images.wisestamp.com/facebook.png" height="16" width="16"></a>
				<a href="https://twitter.com/ttstrain" style="text-decoration: underline" target="_blank">
					<img alt="Twitter" style="padding: 0px 0px 5px; vertical-align: middle" src="http://images.wisestamp.com/twitter.png" height="16" width="16"></a>
			</p>
		</div>
		<div class="span6">
			<h4 align="center" style="color: #1F5B93; margin-bottom: 0px;">Powered by Total Training Solutions Inc.</h4>
			<p align="center" style="color: lightsteelblue; margin-bottom: 0px;">
				<a href="http://www.TTSTrain.com">(800) 831-0678 | www.TTSTrain.com </a>
				<br />
				<br />
				@*<a href="/Home/PrivacyStatement">Privacy Statement</a>&nbsp;&nbsp; &nbsp;  &nbsp; <a href="javascript:void( window.open(''http://form.jotform.us/form/21224022479143'', ''blank'',''scrollbars=yes,toolbar=no,width=700,height=500''))">Technical Support</a>*@
			</p>
		</div>
		<div class="span2">
			<h4 style="color: #1F5B93; margin-bottom: 0px;" align="center">Mailing List</h4>
			<p style="text-align: center; line-height: 110%"><a href="/Home/Mailing">Subscribe to our mailing list and be the first to know about our news and special deals!</a></p>
		</div>
	</div>
	<!-- end .row -->
</div>', N'', N'', N'Bennett', NULL, NULL, N'Total Training Solutions', N'billed', NULL, N'Mark', N'608-849-5563', N'affiliate@ttstrain.com', N'', N'PO Box 310', N'affiliate@ttstrain.com', N'608-849-5563', N'Mark', N'None')
INSERT [dbo].[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (19, 1, N'A', CAST(0x0000A2BE00AC550B AS DateTime), N'Mark', N'Bennett', NULL, 26, N'Mark_Bennett@ttstrain.com', NULL, NULL, NULL, NULL, 3, N'na')
INSERT [dbo].[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (26368, 1, N'A', CAST(0x0000A3240097B1FA AS DateTime), N'Cindy', N'Wanamaker', NULL, 44869, N'cindyw@fmfcu.org', NULL, NULL, NULL, NULL, 3, N'Chief Operations Officer')
INSERT [dbo].[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (10568, 1, N'A', CAST(0x0000A2BE00ABCBB0 AS DateTime), N'Deborah', N'Crawford', NULL, 25, N'Deborah_Crawford@ttstrain.com', NULL, NULL, NULL, NULL, 3, N'na')
INSERT [dbo].[Presenter] ([idUser], [Biography], [BiographyLong], [PhotoFull], [PhotoThumb]) VALUES (10568, N'<p><img class="alignleft" src="http://ttstimages.ttstrain.com/images/presenters/crawford.jpg" alt="Photo of Deborah Crawford" />Deborah Crawford is the President of gettechnical inc. She specializes in compliance and regulations for the deposit side of financial institutions.  Her 20+ year career in banking and training began at Hibernia National Bank. She has been a seminar leader for many state associations and credit union groups across the nation.  She has Bachelors and Masters degrees from Louisiana State University. </p>', N'<p><img class="alignleft" src="http://ttstimages.ttstrain.com/images/presenters/crawford.jpg" alt="Photo of Deborah Crawford" />Deborah Crawford is the President of gettechnical inc. She specializes in compliance and regulations for the deposit side of financial institutions.  Her 20+ year career in banking and training began at Hibernia National Bank. She has been a seminar leader for many state associations and credit union groups across the nation.  She has Bachelors and Masters degrees from Louisiana State University. </p>', N'http://ttstimages.ttstrain.com/images/presenters/crawford.jpg', N'http://ttstimages.ttstrain.com/images/presenters/crawford.jpg')
SET IDENTITY_INSERT [dbo].[RegType] ON
INSERT [dbo].[RegType] ([idRegType], [RegTypeExplain], [RegTypeLabel], [Price], [TaxExempt], [SortOrder], [SKU], [ShowLiveNotifications], [ShowRecordingNotifications], [ShowShippedNotifications], [Stage1CheckoutConfirmationMsg], [Stage2CheckoutConfirmationMsg], [Stage1EmailConfirmationMsg], [Stage2EmailConfirmationMsg]) VALUES (88, N'Attend the live session with the opportunity to ask questions of the presenter.  You also receive handouts.', N'Live Session Only', 155, NULL, 1, N'Live_Session_Only_1Hr_155_97', N'Yes', N'No', N'No', N'This order is for the live session only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email.', N'This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', N'The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions.', N'Connection information is summerized below and is also available at http://www.BankWebinars.com.')
SET IDENTITY_INSERT [dbo].[RegType] OFF
SET IDENTITY_INSERT [dbo].[Institution] ON 
INSERT [dbo].[Institution] ([idInstitution], [InstitutionName], [InstitutionType], [domainName], [RegIdentifier], [Address], [City], [State], [Zip]) VALUES (25, N'TTS', N'New', NULL, N'N', NULL, N'city', N'st', N'55555')
INSERT [dbo].[Institution] ([idInstitution], [InstitutionName], [InstitutionType], [domainName], [RegIdentifier], [Address], [City], [State], [Zip]) VALUES (23613, N'Fidelity Federal Savings and Loan Association', N'FDIC', NULL, N'31167', N'25 East Silver Springs Blvd', N'Ocala', N'FL', N'32670')
INSERT [dbo].[Institution] ([idInstitution], [InstitutionName], [InstitutionType], [domainName], [RegIdentifier], [Address], [City], [State], [Zip]) VALUES (26, N'Total Training Solutions', N'New', NULL, N'N', NULL, N'city', N'st', N'55555')
INSERT [dbo].[Institution] ([idInstitution], [InstitutionName], [InstitutionType], [domainName], [RegIdentifier], [Address], [City], [State], [Zip]) VALUES (44869, N'Franklin Mint Federal Credit Union', N'New', N'fmfcu.org', N'N', NULL, N'Broomall', N'PA', N'19008')
SET IDENTITY_INSERT [dbo].[Institution] OFF
SET IDENTITY_INSERT [dbo].[Topic] ON 
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (15, N'Compliance', NULL, N'', 1)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (16, N'IRA', NULL, N'', 2)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (17, N'Customer Service', NULL, N'', 3)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (18, N'Security', NULL, N'', 5)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (19, N'Operations', NULL, N'', 4)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (20, N'Auditing', NULL, N'', 6)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (21, N'Sales', NULL, N'', 7)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (22, N'Lending', NULL, N'', 8)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (23, N'Human Resources', NULL, N'', 9)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (25, N'Computer Skills', NULL, N'', 1)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (26, N'Risk Management', NULL, N'', 1)
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (27, N'Teller', NULL, N'', 1)
SET IDENTITY_INSERT [dbo].[Topic] OFF
SET IDENTITY_INSERT [dbo].[WebinarTopicXref] ON 
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (5883, 1568, 19)
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (5884, 1568, 26)
SET IDENTITY_INSERT [dbo].[WebinarTopicXref] OFF
GO
/****** Object:  Index [IX_idOrderRow]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idOrderRow] ON [dbo].[AdditionalLocation]
(
	[idOrderRow] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idUser]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON [dbo].[Address]
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idUserAff]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idUserAff] ON [dbo].[Affiliate]
(
	[idUserAff] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idAffiliate]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idAffiliate] ON [dbo].[Order]
(
	[idAffiliate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idUser]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON [dbo].[Order]
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Discount_idDiscount]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_Discount_idDiscount] ON [dbo].[OrderRow]
(
	[Discount_idDiscount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idOrder]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idOrder] ON [dbo].[OrderRow]
(
	[idOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idRegType]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idRegType] ON [dbo].[OrderRow]
(
	[idRegType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON [dbo].[OrderRow]
(
	[idWebinar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idUser]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idUser] ON [dbo].[Presenter]
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idParentTopic]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idParentTopic] ON [dbo].[Topic]
(
	[idParentTopic] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idPresenter]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idPresenter] ON [dbo].[Webinar]
(
	[idPresenter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON [dbo].[WebinarFile]
(
	[idWebinar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idTopic]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idTopic] ON [dbo].[WebinarTopicXref]
(
	[idTopic] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idWebinar]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idWebinar] ON [dbo].[WebinarTopicXref]
(
	[idWebinar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_idUserInstitution]    Script Date: 29/12/2014 4:03:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_idUserInstitution] ON [dbo].[WebUser]
(
	[idUserInstitution] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
USE [master]
GO
ALTER DATABASE [BankWebinars] SET  READ_WRITE 
GO
