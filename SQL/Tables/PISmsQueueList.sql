USE [SMS]
GO

/****** Object:  Table [dbo].[PISmsQueueList]    Script Date: 14/09/2021 4:39:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PISmsQueueList](
	[PIPoint] [nvarchar](max) NULL,
	[Timestamp] [nvarchar](max) NULL,
	[Value] [nvarchar](max) NULL,
	[Operator] [nvarchar](max) NULL,
	[TriggerCondition] [nvarchar](max) NULL,
	[UserName] [nvarchar](max) NULL,
	[CellNumber] [nvarchar](max) NULL,
	[SMS_Status] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


