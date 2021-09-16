USE [SMS]
GO

/****** Object:  Table [dbo].[PIDeliveryListUsers]    Script Date: 14/09/2021 4:39:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PIDeliveryListUsers](
	[GroupID] [int] NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[CellNumber] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[PIDeliveryListUsers]  WITH CHECK ADD  CONSTRAINT [FK_GrouID] FOREIGN KEY([GroupID])
REFERENCES [dbo].[PIDistributionGroup] ([GroupID])
GO

ALTER TABLE [dbo].[PIDeliveryListUsers] CHECK CONSTRAINT [FK_GrouID]
GO


