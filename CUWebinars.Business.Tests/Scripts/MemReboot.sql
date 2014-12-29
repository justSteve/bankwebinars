USE [master]
GO
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'MembershipReboot')
DROP DATABASE [MembershipReboot];

CREATE DATABASE [MembershipReboot] 
ON (name='MembershipReboot', filename='e:\csv\MembershipReboot.mdf')
GO
USE [MembershipReboot]
GO
ALTER DATABASE [MembershipReboot] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MembershipReboot].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MembershipReboot] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MembershipReboot] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MembershipReboot] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MembershipReboot] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MembershipReboot] SET ARITHABORT OFF 
GO
ALTER DATABASE [MembershipReboot] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MembershipReboot] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [MembershipReboot] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MembershipReboot] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MembershipReboot] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MembershipReboot] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MembershipReboot] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MembershipReboot] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MembershipReboot] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MembershipReboot] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MembershipReboot] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MembershipReboot] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MembershipReboot] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MembershipReboot] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MembershipReboot] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [MembershipReboot] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MembershipReboot] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [MembershipReboot] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MembershipReboot] SET RECOVERY FULL 
GO
ALTER DATABASE [MembershipReboot] SET  MULTI_USER 
GO
ALTER DATABASE [MembershipReboot] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MembershipReboot] SET DB_CHAINING OFF 
GO
USE [MembershipReboot]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 29/12/2014 3:57:50 PM ******/
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
/****** Object:  Table [dbo].[GroupChilds]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupChilds](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ParentKey] [int] NOT NULL,
	[ChildGroupID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.GroupChilds] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ParentKey_ChildGroupID] UNIQUE NONCLUSTERED 
(
	[ParentKey] ASC,
	[ChildGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Groups]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Groups](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Tenant] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Groups] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LinkedAccountClaims]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LinkedAccountClaims](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ParentKey] [int] NOT NULL,
	[ProviderName] [nvarchar](30) NOT NULL,
	[ProviderAccountID] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](150) NOT NULL,
	[Value] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_dbo.LinkedAccountClaims] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ParentKey_ProviderName_ProviderAccountID_Type_Value] UNIQUE NONCLUSTERED 
(
	[ParentKey] ASC,
	[ProviderName] ASC,
	[ProviderAccountID] ASC,
	[Type] ASC,
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LinkedAccounts]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LinkedAccounts](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ParentKey] [int] NOT NULL,
	[ProviderName] [nvarchar](30) NOT NULL,
	[ProviderAccountID] [nvarchar](100) NOT NULL,
	[LastLogin] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.LinkedAccounts] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ParentKey_ProviderName_ProviderAccountID] UNIQUE NONCLUSTERED 
(
	[ParentKey] ASC,
	[ProviderName] ASC,
	[ProviderAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PasswordResetSecrets]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordResetSecrets](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ParentKey] [int] NOT NULL,
	[PasswordResetSecretID] [uniqueidentifier] NOT NULL,
	[Question] [nvarchar](150) NOT NULL,
	[Answer] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_dbo.PasswordResetSecrets] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ParentKey_Question] UNIQUE NONCLUSTERED 
(
	[ParentKey] ASC,
	[Question] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TwoFactorAuthTokens]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TwoFactorAuthTokens](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ParentKey] [int] NOT NULL,
	[Token] [nvarchar](100) NOT NULL,
	[Issued] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TwoFactorAuthTokens] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserAccounts]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccounts](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Tenant] [nvarchar](50) NOT NULL,
	[Username] [nvarchar](254) NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
	[IsAccountClosed] [bit] NOT NULL,
	[AccountClosed] [datetime] NULL,
	[IsLoginAllowed] [bit] NOT NULL,
	[LastLogin] [datetime] NULL,
	[LastFailedLogin] [datetime] NULL,
	[FailedLoginCount] [int] NOT NULL,
	[PasswordChanged] [datetime] NULL,
	[RequiresPasswordReset] [bit] NOT NULL,
	[Email] [nvarchar](254) NULL,
	[IsAccountVerified] [bit] NOT NULL,
	[LastFailedPasswordReset] [datetime] NULL,
	[FailedPasswordResetCount] [int] NOT NULL,
	[MobileCode] [nvarchar](100) NULL,
	[MobileCodeSent] [datetime] NULL,
	[MobilePhoneNumber] [nvarchar](20) NULL,
	[MobilePhoneNumberChanged] [datetime] NULL,
	[AccountTwoFactorAuthMode] [int] NOT NULL,
	[CurrentTwoFactorAuthStatus] [int] NOT NULL,
	[VerificationKey] [nvarchar](100) NULL,
	[VerificationPurpose] [int] NULL,
	[VerificationKeySent] [datetime] NULL,
	[VerificationStorage] [nvarchar](100) NULL,
	[HashedPassword] [nvarchar](200) NULL,
 CONSTRAINT [PK_dbo.UserAccounts] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserCertificates]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCertificates](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ParentKey] [int] NOT NULL,
	[Thumbprint] [nvarchar](150) NOT NULL,
	[Subject] [nvarchar](250) NULL,
 CONSTRAINT [PK_dbo.UserCertificates] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ParentKey_Thumbprint] UNIQUE NONCLUSTERED 
(
	[ParentKey] ASC,
	[Thumbprint] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserClaims]    Script Date: 29/12/2014 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserClaims](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[ParentKey] [int] NOT NULL,
	[Type] [nvarchar](150) NOT NULL,
	[Value] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_dbo.UserClaims] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ParentKey_Type_Value] UNIQUE NONCLUSTERED 
(
	[ParentKey] ASC,
	[Type] ASC,
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Index [IX_ParentKey]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentKey] ON [dbo].[GroupChilds]
(
	[ParentKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ID]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ID] ON [dbo].[Groups]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Tenant_Name]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenant_Name] ON [dbo].[Groups]
(
	[Tenant] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentKey]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentKey] ON [dbo].[LinkedAccountClaims]
(
	[ParentKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentKey]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentKey] ON [dbo].[LinkedAccounts]
(
	[ParentKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ProviderName_ProviderAccountID]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_ProviderName_ProviderAccountID] ON [dbo].[LinkedAccounts]
(
	[ProviderName] ASC,
	[ProviderAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentKey]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentKey] ON [dbo].[PasswordResetSecrets]
(
	[ParentKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentKey]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentKey] ON [dbo].[TwoFactorAuthTokens]
(
	[ParentKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ID]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ID] ON [dbo].[UserAccounts]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Tenant_Email]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_Tenant_Email] ON [dbo].[UserAccounts]
(
	[Tenant] ASC,
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Tenant_Username]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenant_Username] ON [dbo].[UserAccounts]
(
	[Tenant] ASC,
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Username]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_Username] ON [dbo].[UserAccounts]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_VerificationKey]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_VerificationKey] ON [dbo].[UserAccounts]
(
	[VerificationKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentKey]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentKey] ON [dbo].[UserCertificates]
(
	[ParentKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Thumbprint]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_Thumbprint] ON [dbo].[UserCertificates]
(
	[Thumbprint] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentKey]    Script Date: 29/12/2014 3:57:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentKey] ON [dbo].[UserClaims]
(
	[ParentKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GroupChilds]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GroupChilds_dbo.Groups_ParentKey] FOREIGN KEY([ParentKey])
REFERENCES [dbo].[Groups] ([Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GroupChilds] CHECK CONSTRAINT [FK_dbo.GroupChilds_dbo.Groups_ParentKey]
GO
ALTER TABLE [dbo].[LinkedAccountClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LinkedAccountClaims_dbo.UserAccounts_ParentKey] FOREIGN KEY([ParentKey])
REFERENCES [dbo].[UserAccounts] ([Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LinkedAccountClaims] CHECK CONSTRAINT [FK_dbo.LinkedAccountClaims_dbo.UserAccounts_ParentKey]
GO
ALTER TABLE [dbo].[LinkedAccounts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LinkedAccounts_dbo.UserAccounts_ParentKey] FOREIGN KEY([ParentKey])
REFERENCES [dbo].[UserAccounts] ([Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LinkedAccounts] CHECK CONSTRAINT [FK_dbo.LinkedAccounts_dbo.UserAccounts_ParentKey]
GO
ALTER TABLE [dbo].[PasswordResetSecrets]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PasswordResetSecrets_dbo.UserAccounts_ParentKey] FOREIGN KEY([ParentKey])
REFERENCES [dbo].[UserAccounts] ([Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PasswordResetSecrets] CHECK CONSTRAINT [FK_dbo.PasswordResetSecrets_dbo.UserAccounts_ParentKey]
GO
ALTER TABLE [dbo].[TwoFactorAuthTokens]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TwoFactorAuthTokens_dbo.UserAccounts_ParentKey] FOREIGN KEY([ParentKey])
REFERENCES [dbo].[UserAccounts] ([Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TwoFactorAuthTokens] CHECK CONSTRAINT [FK_dbo.TwoFactorAuthTokens_dbo.UserAccounts_ParentKey]
GO
ALTER TABLE [dbo].[UserCertificates]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserCertificates_dbo.UserAccounts_ParentKey] FOREIGN KEY([ParentKey])
REFERENCES [dbo].[UserAccounts] ([Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserCertificates] CHECK CONSTRAINT [FK_dbo.UserCertificates_dbo.UserAccounts_ParentKey]
GO
ALTER TABLE [dbo].[UserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserClaims_dbo.UserAccounts_ParentKey] FOREIGN KEY([ParentKey])
REFERENCES [dbo].[UserAccounts] ([Key])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserClaims] CHECK CONSTRAINT [FK_dbo.UserClaims_dbo.UserAccounts_ParentKey]
GO
USE [master]
GO
ALTER DATABASE [MembershipReboot] SET  READ_WRITE 
GO
