USE [master]
GO
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'CUWebinars')
DROP DATABASE [CUWebinars];

CREATE DATABASE [CUWebinars]
ON (name='CUWebinars', filename='E:\CUWebinars.mdf')
GO
USE [CUWebinars]
GO
ALTER DATABASE [CUWebinars] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CUWebinars].[dbo].[sp_fulltext_database] @action = 'enable'
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
ALTER DATABASE [CUWebinars] SET  ENABLE_BROKER 
GO
ALTER DATABASE [CUWebinars] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CUWebinars] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CUWebinars] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CUWebinars] SET ALLOW_SNAPSHOT_ISOLATION OFF 
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
USE [CUWebinars]

GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [PLAGUIS\Dave]

GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[AdditionalLocation]    Script Date: 22/05/2014 11:10:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
 CONSTRAINT [PK_dbo.AdditionalLocation] PRIMARY KEY CLUSTERED 
(
	[idAdditionalLocation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Address]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[Affiliate]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[Discount]    Script Date: 22/05/2014 11:10:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discount](
	[idDiscount] [int] IDENTITY(1,1) NOT NULL,
	[discountType] [tinyint] NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[percentOff] [decimal](18, 2) NOT NULL,
	[flatOff] [decimal](18, 2) NOT NULL,
	[usesNumber] [int] NOT NULL,
	[dateValidFrom] [datetime] NOT NULL,
	[dateValidTo] [datetime] NOT NULL,
	[status] [nvarchar](50) NOT NULL,
	[dateBilled] [datetime] NULL,
	[cost] [decimal](18, 2) NULL,
	[Notes] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Discount] PRIMARY KEY CLUSTERED 
(
	[idDiscount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Institution]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[Order]    Script Date: 22/05/2014 11:10:24 AM ******/
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
 CONSTRAINT [PK_dbo.Order] PRIMARY KEY CLUSTERED 
(
	[idOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderRow]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[Presenter]    Script Date: 22/05/2014 11:10:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Presenter](
	[idUser] [int] NOT NULL,
	[Biography] [nvarchar](2500) NULL,
	[BiographyLong] [nvarchar](2500) NOT NULL,
	[PhotoFull] [nvarchar](200) NULL,
	[PhotoThumb] [nvarchar](200) NULL,
 CONSTRAINT [PK_dbo.Presenter] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RegType]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[RegTypesGroups]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[RegTypesGroupsXref]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[RegTypesXref]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[Topic]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[Webinar]    Script Date: 22/05/2014 11:10:24 AM ******/
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
	[AccessPhone] [nvarchar](max) NULL,
	[AccessCode] [nvarchar](max) NULL,
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
/****** Object:  Table [dbo].[WebinarFile]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[WebinarTopicXref]    Script Date: 22/05/2014 11:10:24 AM ******/
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
/****** Object:  Table [dbo].[WebUser]    Script Date: 22/05/2014 11:10:24 AM ******/
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
USE [master]
GO
ALTER DATABASE [CUWebinars] SET  READ_WRITE 
GO
USE [CUWebinars]

SET IDENTITY_INSERT [dbo].[Webinar] ON
INSERT [dbo].[Webinar] ([idWebinar], [Description], [DescriptionLong], [ImageUrl], [SmallImageUrl], [Status], [Title], [Date], [LearnCaption], [LearnBody], [WhoAttend], [Duration], [RecordingUrl], [idPresenter], [WebinarKey], [OrganizerKey], [OrganizerOAuthKey], [AccessPhone], [AccessCode], [ceu], [ConnectionInfo], [DateCreated], [DateChanged]) VALUES (404, N'<p>Your member passed away yesterday and the family is grieving. Yet, in the midst of all the remembering and honoring of a life, the legal and compliance clock is ticking. In most states, checks can be paid for 10 days after the date of death.</p><p>When your account holder dies, many issues and questions arise. Who can have information on the account? Who owns the account? Who has access to the account? What happens with powers of attorney and authorized signers on account? Can the spouse of the sole proprietor continue to access the account? What about that last tax refund check? Is the account still insured? Can a check be written to a funeral home? What about the checks coming in to pay funeral expense? Learn about checks, IRAs, deposit ownership, trusts, UTMA, affidavits of heirship and other complex issues that can occur when a member dies. </p>', N'<p>Your member passed away yesterday and the family is grieving. Yet, in the midst of all the remembering and honoring of a life, the legal and compliance clock is ticking. In most states, checks can be paid for 10 days after the date of death.</p><p>When your account holder dies, many issues and questions arise. Who can have information on the account? Who owns the account? Who has access to the account? What happens with powers of attorney and authorized signers on account? Can the spouse of the sole proprietor continue to access the account? What about that last tax refund check? Is the account still insured? Can a check be written to a funeral home? What about the checks coming in to pay funeral expense? Learn about checks, IRAs, deposit ownership, trusts, UTMA, affidavits of heirship and other complex issues that can occur when a member dies. </p>', N'', N'', 2, N'10 Lessons Learned When Your Member Dies', CAST(0x0000A34800A4CB80 AS DateTime), N'Covered Topics', N'<ul><li>Probate versus non-probate transfers</li><li>When does the will govern and when does the signature card?</li><li>What bypasses a will? PODs, IRAs, JTWROS?</li><li>Living trusts and successor trustees</li><li>When do we know that a customer is deceased?</li><li>When can we use small estate affidavits?</li><li>What happens to powers of attorney, authorized signers, etc.</li><li>How are IRAs, HSAs and UTMAs affected?</li><li>Do sole proprietorships cease at death?</li><li>NCUSIF insurance issues</li></ul>', N'This informative session is designed for customer service representatives, branch administration, branch managers, tellers, training and development staff, compliance personnel, and anyone who handles customer accounts.', CAST(1.00 AS Decimal(18, 2)), N'', 10568, NULL, N'922930', N'5jxY3KZL48HWknOaOEP2eIzVmOTS', NULL, NULL, N'1.25 CE Credits|Recommended for 1.25 CE Credit Hours. After attending this webinar, each attendee can receive a Certificate of Attendance for self-reporting of CE Credits.', NULL, CAST(0x0000A2EB01477614 AS DateTime), CAST(0x0000A2EB01477614 AS DateTime))

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
INSERT [dbo].[Presenter] ([idUser], [Biography], [BiographyLong], [PhotoFull], [PhotoThumb]) VALUES (10568, N'<p><img class="alignleft" src="https://ttseast.blob.core.windows.net/images/presenters/crawford.jpg" alt="Photo of Deborah Crawford" />Deborah Crawford is the President of gettechnical inc. She specializes in compliance and regulations for the deposit side of financial institutions.  Her 20+ year career in banking and training began at Hibernia National Bank. She has been a seminar leader for many state associations and credit union groups across the nation.  She has Bachelors and Masters degrees from Louisiana State University. </p>', N'<p><img class="alignleft" src="https://ttseast.blob.core.windows.net/images/presenters/crawford.jpg" alt="Photo of Deborah Crawford" />Deborah Crawford is the President of gettechnical inc. She specializes in compliance and regulations for the deposit side of financial institutions.  Her 20+ year career in banking and training began at Hibernia National Bank. She has been a seminar leader for many state associations and credit union groups across the nation.  She has Bachelors and Masters degrees from Louisiana State University. </p>', N'https://ttseast.blob.core.windows.net/images/presenters/crawford.jpg', N'https://ttseast.blob.core.windows.net/images/presenters/crawford.jpg')
SET IDENTITY_INSERT [dbo].[RegType] ON
INSERT [dbo].[RegType] ([idRegType], [RegTypeExplain], [RegTypeLabel], [Price], [TaxExempt], [SortOrder], [SKU], [ShowLiveNotifications], [ShowRecordingNotifications], [ShowShippedNotifications], [Stage1CheckoutConfirmationMsg], [Stage2CheckoutConfirmationMsg], [Stage1EmailConfirmationMsg], [Stage2EmailConfirmationMsg]) VALUES (88, N'Attend the live session with the opportunity to ask questions of the presenter.  You also receive handouts.', N'Live Session Only', 155, NULL, 1, N'Live_Session_Only_1Hr_155_97', N'Yes', N'No', N'No', N'This order is for the live session only. The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) well notify you by email.', N'This order is for the live session only. The information required to connect to the event is summarized below. Detailed information has been sent to your email and is also available at http://www.BankWebinars.com/MyWebinars.', N'The connection information for your event is not yet generated, but as soon as it is (usually 2-3 days before the event) youll receive another email with complete connection instructions.', N'Connection information is summerized below and is also available at http://www.BankWebinars.com.')
SET IDENTITY_INSERT [dbo].[RegType] OFF
