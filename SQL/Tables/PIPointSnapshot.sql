USE [SMS]
GO

/****** Object:  Table [dbo].[PIPointSnapshot]    Script Date: 14/09/2021 4:39:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PIPointSnapshot](
	[PIPoint] [nvarchar](max) NULL,
	[Timestamp] [nvarchar](max) NULL,
	[Value] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


