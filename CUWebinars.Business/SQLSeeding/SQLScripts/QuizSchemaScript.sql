/******************************************************** Tables etc. ********************************************************/
GO
/****** Object:  Table [dbo].[Option]    Script Date: 17/05/2015 3:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Option](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](800) NOT NULL,
 CONSTRAINT [PK_Option] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Question]    Script Date: 17/05/2015 3:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Question](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](800) NOT NULL,
	[idTimeLimit] [int] NULL,
	[idType] [int] NOT NULL,
 CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestionWithOption]    Script Date: 17/05/2015 3:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuestionWithOption](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[idQuestion] [int] NOT NULL,
	[idOption] [int] NOT NULL,
	[CorrectAnswer] [bit] NOT NULL,
	[Letter] [char](1) NOT NULL,
 CONSTRAINT [PK_QuestionWithOption] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Quiz]    Script Date: 17/05/2015 3:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Quiz](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[idWebinar] [int] NOT NULL,
	[idTimeLimit] [int] NULL,
	[QuizCode] [nvarchar](10) NULL,
 CONSTRAINT [PK_Quiz] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuizUserAnswer]    Script Date: 17/05/2015 3:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuizUserAnswer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[idQuizQuestion] [int] NOT NULL,
	[Letter] [char](1) NOT NULL,
	[idQuizUserOrder] [int] NOT NULL,
 CONSTRAINT [PK_QuizUserAnswer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[QuizUserOrder]    Script Date: 17/05/2015 3:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuizUserOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[idOrder] [int] NOT NULL,
	[idQuiz] [int] NOT NULL,
	[Score] [int] NOT NULL,
	[QuestionCount] [int] NOT NULL,
	[DateQuizTaken] [datetime] NOT NULL,
 CONSTRAINT [PK_QuizUserOrder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuizWithQuestion]    Script Date: 17/05/2015 3:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuizWithQuestion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[idQuiz] [int] NOT NULL,
	[idQuestion] [int] NOT NULL,
	[QuestionNumber] [int] NOT NULL,
 CONSTRAINT [PK_QuizWithQuestion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TimeLimit]    Script Date: 17/05/2015 3:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimeLimit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Hours] [int] NOT NULL,
	[Minutes] [int] NOT NULL,
	[Seconds] [int] NOT NULL,
 CONSTRAINT [PK_TimeLimit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

/******************************************************** Indexes, FKs etc. ********************************************************/

GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Question_TimeLimit] FOREIGN KEY([idTimeLimit])
REFERENCES [dbo].[TimeLimit] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Question_TimeLimit]
GO
ALTER TABLE [dbo].[QuestionWithOption]  WITH CHECK ADD  CONSTRAINT [FK_QuestionWithOption_Option] FOREIGN KEY([idOption])
REFERENCES [dbo].[Option] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QuestionWithOption] CHECK CONSTRAINT [FK_QuestionWithOption_Option]
GO
ALTER TABLE [dbo].[QuestionWithOption]  WITH CHECK ADD  CONSTRAINT [FK_QuestionWithOption_Question] FOREIGN KEY([idQuestion])
REFERENCES [dbo].[Question] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QuestionWithOption] CHECK CONSTRAINT [FK_QuestionWithOption_Question]
GO
ALTER TABLE [dbo].[Quiz]  WITH CHECK ADD  CONSTRAINT [FK_Quiz_TimeLimit] FOREIGN KEY([idTimeLimit])
REFERENCES [dbo].[TimeLimit] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Quiz] CHECK CONSTRAINT [FK_Quiz_TimeLimit]
GO
ALTER TABLE [dbo].[Quiz]  WITH CHECK ADD  CONSTRAINT [FK_Quiz_Webinar] FOREIGN KEY([idWebinar])
REFERENCES [dbo].[Webinar] ([idWebinar])
GO
ALTER TABLE [dbo].[Quiz] CHECK CONSTRAINT [FK_Quiz_Webinar]
GO
ALTER TABLE [dbo].[QuizUserAnswer]  WITH CHECK ADD  CONSTRAINT [FK_QuizUserAnswer_QuizUserOrder] FOREIGN KEY([idQuizUserOrder])
REFERENCES [dbo].[QuizUserOrder] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QuizUserAnswer] CHECK CONSTRAINT [FK_QuizUserAnswer_QuizUserOrder]
GO
ALTER TABLE [dbo].[QuizUserAnswer]  WITH CHECK ADD  CONSTRAINT [FK_QuizUserAnswer_QuizWithQuestion] FOREIGN KEY([idQuizQuestion])
REFERENCES [dbo].[QuizWithQuestion] ([Id])
GO
ALTER TABLE [dbo].[QuizUserAnswer] CHECK CONSTRAINT [FK_QuizUserAnswer_QuizWithQuestion]
GO
ALTER TABLE [dbo].[QuizUserOrder]  WITH CHECK ADD  CONSTRAINT [FK_QuizUserOrder_Order] FOREIGN KEY([idOrder])
REFERENCES [dbo].[Order] ([idOrder])
GO
ALTER TABLE [dbo].[QuizUserOrder] CHECK CONSTRAINT [FK_QuizUserOrder_Order]
GO
ALTER TABLE [dbo].[QuizUserOrder]  WITH CHECK ADD  CONSTRAINT [FK_QuizUserOrder_Quiz] FOREIGN KEY([idQuiz])
REFERENCES [dbo].[Quiz] ([Id])
GO
ALTER TABLE [dbo].[QuizUserOrder] CHECK CONSTRAINT [FK_QuizUserOrder_Quiz]
GO
ALTER TABLE [dbo].[QuizWithQuestion]  WITH CHECK ADD  CONSTRAINT [FK_QuizWithQuestion_Question] FOREIGN KEY([idQuestion])
REFERENCES [dbo].[Question] ([Id])
GO
ALTER TABLE [dbo].[QuizWithQuestion] CHECK CONSTRAINT [FK_QuizWithQuestion_Question]
GO
ALTER TABLE [dbo].[QuizWithQuestion]  WITH CHECK ADD  CONSTRAINT [FK_QuizWithQuestion_Quiz] FOREIGN KEY([idQuiz])
REFERENCES [dbo].[Quiz] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QuizWithQuestion] CHECK CONSTRAINT [FK_QuizWithQuestion_Quiz]
GO
