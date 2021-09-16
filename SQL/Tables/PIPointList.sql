USE [SMS]
GO

/****** Object:  Table [dbo].[PIPointList]    Script Date: 14/09/2021 4:39:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PIPointList](
	[GroupID] [int] NOT NULL,
	[PIPointName] [nvarchar](max) NOT NULL,
	[Operator] [nvarchar](max) NOT NULL,
	[TriggerCondition] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[PIPointList]  WITH CHECK ADD  CONSTRAINT [FK_GroupID] FOREIGN KEY([GroupID])
REFERENCES [dbo].[PIDistributionGroup] ([GroupID])
GO

ALTER TABLE [dbo].[PIPointList] CHECK CONSTRAINT [FK_GroupID]
GO


