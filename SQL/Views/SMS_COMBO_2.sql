USE [SMS]
GO

/****** Object:  View [dbo].[SMS_COMBO_2]    Script Date: 14/09/2021 4:43:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


create VIEW [dbo].[SMS_COMBO_2] as (Select [PIPoint],[Timestamp],[Value],[Operator],[TriggerCondition],[UserName],[CellNumber] from [PIDeliveryListUsers] inner join [SMS_COMBO_1] ON PIDeliveryListUsers.GroupID = [SMS_COMBO_1].GroupID)

GO


