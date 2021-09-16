USE [SMS]
GO

/****** Object:  View [dbo].[SMS_COMBO_1]    Script Date: 14/09/2021 4:43:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[SMS_COMBO_1] AS
select [PIPoint],[Timestamp],[Value],[Operator],[TriggerCondition],[GroupID] FROM [PIPointList] 
full outer JOIN [PIPointSnapshot] ON PIPointSnapshot.PIPoint = PIPointList.PIPointName




GO


