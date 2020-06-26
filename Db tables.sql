USE [QS]
GO

/****** Object:  Table [dbo].[Answers_tbl]    Script Date: 5/24/2018 1:53:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Answers_tbl](
	[A_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[q_ID] [bigint] NULL,
	[states] [bit] NOT NULL,
	[action] [nvarchar](max) NULL,
	[done] [bit] NOT NULL,
	[triggerID] [bigint] NOT NULL,
	[TableType] [nvarchar](max) NULL,
	[DoneDate] [datetime] NULL,
	[DoneTrigger] [bigint] NULL,
 CONSTRAINT [PK__Answers___71AC6D41CCE7C482] PRIMARY KEY CLUSTERED 
(
	[A_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Answers_tbl]  WITH CHECK ADD  CONSTRAINT [FK__Answers_tb__q_ID__3B75D760] FOREIGN KEY([q_ID])
REFERENCES [dbo].[Questions_tbl] ([Q_ID])
GO

ALTER TABLE [dbo].[Answers_tbl] CHECK CONSTRAINT [FK__Answers_tb__q_ID__3B75D760]
GO

ALTER TABLE [dbo].[Answers_tbl]  WITH CHECK ADD  CONSTRAINT [FK_Answers_tbl_Triggers_tbl] FOREIGN KEY([triggerID])
REFERENCES [dbo].[Triggers_tbl] ([TriggerID])
GO

ALTER TABLE [dbo].[Answers_tbl] CHECK CONSTRAINT [FK_Answers_tbl_Triggers_tbl]
GO

ALTER TABLE [dbo].[Answers_tbl]  WITH CHECK ADD  CONSTRAINT [FK_Answers_tbl_Triggers_tbl1] FOREIGN KEY([DoneTrigger])
REFERENCES [dbo].[Triggers_tbl] ([TriggerID])
GO

ALTER TABLE [dbo].[Answers_tbl] CHECK CONSTRAINT [FK_Answers_tbl_Triggers_tbl1]
GO

ALTER TABLE [dbo].[Answers_tbl]  WITH CHECK ADD  CONSTRAINT [CK__Answers_t__Table__3D5E1FD2] CHECK  (([TableType]='Safety' OR [TableType]='Quality'))
GO

ALTER TABLE [dbo].[Answers_tbl] CHECK CONSTRAINT [CK__Answers_t__Table__3D5E1FD2]
GO

/************************************************/


USE [QS]
GO

/****** Object:  Table [dbo].[Questions_tbl]    Script Date: 5/24/2018 1:54:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Questions_tbl](
	[Q_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[question] [nvarchar](max) NULL,
	[AnsType] [bit] NOT NULL,
	[Available] [bit] NOT NULL,
	[TableType] [nvarchar](max) NULL,
 CONSTRAINT [PK__Question__F4FC372E9BEA38FD] PRIMARY KEY CLUSTERED 
(
	[Q_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Questions_tbl]  WITH CHECK ADD  CONSTRAINT [CK__Questions__Table__1BFD2C07] CHECK  (([TableType]='Safety' OR [TableType]='Quality'))
GO

ALTER TABLE [dbo].[Questions_tbl] CHECK CONSTRAINT [CK__Questions__Table__1BFD2C07]
GO

ALTER TABLE [dbo].[Questions_tbl]  WITH CHECK ADD  CONSTRAINT [CK__Questions__Table__267ABA7A] CHECK  (([TableType]='Safety' OR [TableType]='Quality'))
GO

ALTER TABLE [dbo].[Questions_tbl] CHECK CONSTRAINT [CK__Questions__Table__267ABA7A]
GO


/************************************************/

USE [QS]
GO

/****** Object:  Table [dbo].[Summary_tbl]    Script Date: 5/24/2018 1:55:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Summary_tbl](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TrigID] [bigint] NOT NULL,
	[SafetyBefore] [bit] NULL,
	[SafetyAfter] [bit] NULL,
	[QualityBefore] [bit] NULL,
	[QualityAfter] [bit] NULL,
	[Inserted] [datetime] NULL,
	[LastEdited] [datetime] NULL,
 CONSTRAINT [PK_Summary_tbl] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Summary_tbl]  WITH CHECK ADD  CONSTRAINT [FK_Summary_Triggers_tbl] FOREIGN KEY([TrigID])
REFERENCES [dbo].[Triggers_tbl] ([TriggerID])
GO

ALTER TABLE [dbo].[Summary_tbl] CHECK CONSTRAINT [FK_Summary_Triggers_tbl]
GO


/********************************************************/


USE [QS]
GO

/****** Object:  Table [dbo].[Triggers_tbl]    Script Date: 5/24/2018 1:55:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Triggers_tbl](
	[TriggerID] [bigint] IDENTITY(1,1) NOT NULL,
	[line] [int] NOT NULL,
	[shift] [nvarchar](5) NOT NULL,
	[daydate] [date] NOT NULL,
	[usernum] [bigint] NULL,
 CONSTRAINT [PK__Triggers__11321F02D7210E01] PRIMARY KEY CLUSTERED 
(
	[TriggerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/***********************************************************/

USE [QS]
GO

/****** Object:  Table [dbo].[User_Line]    Script Date: 5/24/2018 1:56:03 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[User_Line](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[User_num] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Line] [int] NULL,
	[Type] [nvarchar](50) NULL,
 CONSTRAINT [PK_User_Line] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


