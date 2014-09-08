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
ALTER DATABASE [MembershipReboot] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MembershipReboot] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [MembershipReboot]
GO
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
/****** Object:  Table [dbo].[GroupChilds]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupChilds](
	[GroupID] [uniqueidentifier] NOT NULL,
	[ChildGroupID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.GroupChilds] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[ChildGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Groups]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Groups](
	[ID] [uniqueidentifier] NOT NULL,
	[Tenant] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Groups] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LinkedAccountClaims]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LinkedAccountClaims](
	[UserAccountID] [uniqueidentifier] NOT NULL,
	[ProviderName] [nvarchar](30) NOT NULL,
	[ProviderAccountID] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](150) NOT NULL,
	[Value] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_dbo.LinkedAccountClaims] PRIMARY KEY CLUSTERED 
(
	[UserAccountID] ASC,
	[ProviderName] ASC,
	[ProviderAccountID] ASC,
	[Type] ASC,
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LinkedAccounts]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LinkedAccounts](
	[UserAccountID] [uniqueidentifier] NOT NULL,
	[ProviderName] [nvarchar](30) NOT NULL,
	[ProviderAccountID] [nvarchar](100) NOT NULL,
	[LastLogin] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.LinkedAccounts] PRIMARY KEY CLUSTERED 
(
	[UserAccountID] ASC,
	[ProviderName] ASC,
	[ProviderAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PasswordResetSecrets]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordResetSecrets](
	[PasswordResetSecretID] [uniqueidentifier] NOT NULL,
	[UserAccountID] [uniqueidentifier] NOT NULL,
	[Question] [nvarchar](150) NOT NULL,
	[Answer] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_dbo.PasswordResetSecrets] PRIMARY KEY CLUSTERED 
(
	[PasswordResetSecretID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TwoFactorAuthTokens]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TwoFactorAuthTokens](
	[UserAccountID] [uniqueidentifier] NOT NULL,
	[Token] [nvarchar](100) NOT NULL,
	[Issued] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TwoFactorAuthTokens] PRIMARY KEY CLUSTERED 
(
	[UserAccountID] ASC,
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserAccounts]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccounts](
	[ID] [uniqueidentifier] NOT NULL,
	[Tenant] [nvarchar](50) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Created] [datetime] NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
	[PasswordChanged] [datetime] NULL,
	[RequiresPasswordReset] [bit] NOT NULL,
	[MobileCode] [nvarchar](100) NULL,
	[MobileCodeSent] [datetime] NULL,
	[MobilePhoneNumber] [nvarchar](20) NULL,
	[AccountTwoFactorAuthMode] [int] NOT NULL,
	[CurrentTwoFactorAuthStatus] [int] NOT NULL,
	[IsAccountVerified] [bit] NOT NULL,
	[IsLoginAllowed] [bit] NOT NULL,
	[IsAccountClosed] [bit] NOT NULL,
	[AccountClosed] [datetime] NULL,
	[LastLogin] [datetime] NULL,
	[LastFailedLogin] [datetime] NULL,
	[FailedLoginCount] [int] NOT NULL,
	[VerificationKey] [nvarchar](100) NULL,
	[VerificationPurpose] [int] NULL,
	[VerificationKeySent] [datetime] NULL,
	[HashedPassword] [nvarchar](200) NULL,
	[LastFailedPasswordReset] [datetime] NULL,
	[FailedPasswordResetCount] [int] NOT NULL,
	[MobilePhoneNumberChanged] [datetime] NULL,
	[VerificationStorage] [nvarchar](100) NULL,
 CONSTRAINT [PK_dbo.UserAccounts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserCertificates]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCertificates](
	[UserAccountID] [uniqueidentifier] NOT NULL,
	[Thumbprint] [nvarchar](150) NOT NULL,
	[Subject] [nvarchar](250) NULL,
 CONSTRAINT [PK_dbo.UserCertificates] PRIMARY KEY CLUSTERED 
(
	[UserAccountID] ASC,
	[Thumbprint] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserClaims]    Script Date: 2/06/2014 11:25:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserClaims](
	[UserAccountID] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](150) NOT NULL,
	[Value] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_dbo.UserClaims] PRIMARY KEY CLUSTERED 
(
	[UserAccountID] ASC,
	[Type] ASC,
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Index [IX_GroupID]    Script Date: 2/06/2014 11:25:14 AM ******/
CREATE NONCLUSTERED INDEX [IX_GroupID] ON [dbo].[GroupChilds]
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAccountID]    Script Date: 2/06/2014 11:25:14 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserAccountID] ON [dbo].[LinkedAccountClaims]
(
	[UserAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAccountID]    Script Date: 2/06/2014 11:25:14 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserAccountID] ON [dbo].[LinkedAccounts]
(
	[UserAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAccountID]    Script Date: 2/06/2014 11:25:14 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserAccountID] ON [dbo].[PasswordResetSecrets]
(
	[UserAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAccountID]    Script Date: 2/06/2014 11:25:14 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserAccountID] ON [dbo].[TwoFactorAuthTokens]
(
	[UserAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAccountID]    Script Date: 2/06/2014 11:25:14 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserAccountID] ON [dbo].[UserCertificates]
(
	[UserAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAccountID]    Script Date: 2/06/2014 11:25:14 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserAccountID] ON [dbo].[UserClaims]
(
	[UserAccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UserAccounts] ADD  DEFAULT ((0)) FOR [FailedPasswordResetCount]
GO
ALTER TABLE [dbo].[GroupChilds]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.GroupChilds_dbo.Groups_GroupID] FOREIGN KEY([GroupID])
REFERENCES [dbo].[Groups] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GroupChilds] NOCHECK CONSTRAINT [FK_dbo.GroupChilds_dbo.Groups_GroupID]
GO
ALTER TABLE [dbo].[LinkedAccountClaims]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.LinkedAccountClaims_dbo.UserAccounts_UserAccountID] FOREIGN KEY([UserAccountID])
REFERENCES [dbo].[UserAccounts] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LinkedAccountClaims] NOCHECK CONSTRAINT [FK_dbo.LinkedAccountClaims_dbo.UserAccounts_UserAccountID]
GO
ALTER TABLE [dbo].[LinkedAccounts]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.LinkedAccounts_dbo.UserAccounts_UserAccountID] FOREIGN KEY([UserAccountID])
REFERENCES [dbo].[UserAccounts] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LinkedAccounts] NOCHECK CONSTRAINT [FK_dbo.LinkedAccounts_dbo.UserAccounts_UserAccountID]
GO
ALTER TABLE [dbo].[PasswordResetSecrets]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.PasswordResetSecrets_dbo.UserAccounts_UserAccountID] FOREIGN KEY([UserAccountID])
REFERENCES [dbo].[UserAccounts] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PasswordResetSecrets] NOCHECK CONSTRAINT [FK_dbo.PasswordResetSecrets_dbo.UserAccounts_UserAccountID]
GO
ALTER TABLE [dbo].[TwoFactorAuthTokens]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.TwoFactorAuthTokens_dbo.UserAccounts_UserAccountID] FOREIGN KEY([UserAccountID])
REFERENCES [dbo].[UserAccounts] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TwoFactorAuthTokens] NOCHECK CONSTRAINT [FK_dbo.TwoFactorAuthTokens_dbo.UserAccounts_UserAccountID]
GO
ALTER TABLE [dbo].[UserCertificates]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.UserCertificates_dbo.UserAccounts_UserAccountID] FOREIGN KEY([UserAccountID])
REFERENCES [dbo].[UserAccounts] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserCertificates] NOCHECK CONSTRAINT [FK_dbo.UserCertificates_dbo.UserAccounts_UserAccountID]
GO
ALTER TABLE [dbo].[UserClaims]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.UserClaims_dbo.UserAccounts_UserAccountID] FOREIGN KEY([UserAccountID])
REFERENCES [dbo].[UserAccounts] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserClaims] NOCHECK CONSTRAINT [FK_dbo.UserClaims_dbo.UserAccounts_UserAccountID]
GO
USE [master]
GO
ALTER DATABASE [MembershipReboot] SET  READ_WRITE 
GO
