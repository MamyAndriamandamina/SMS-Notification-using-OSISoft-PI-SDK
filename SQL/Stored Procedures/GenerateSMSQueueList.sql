USE [master]
GO

/****** Object:  StoredProcedure [dbo].[GenerateSMSQueueList]    Script Date: 14/09/2021 4:24:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Mamy Razanamparany ANDRIAMANDAMINA>
-- Create date: <28-APR-2021>
-- Description:	<Generate SMS Queue List based on latest snapshot and all sms configuration (user group, user list, trigger condition, etc)>
-- =============================================
CREATE PROCEDURE [dbo].[GenerateSMSQueueList]
AS
BEGIN
exec(
'
MERGE [SMS].[dbo].[PISmsQueueList] AS target 
		USING 
		(Select * from [SMS].[dbo].[SMS_COMBO_2])
		AS source ([PIPoint],[Timestamp],[Value],[Operator],[TriggerCondition],[UserName],[CellNumber])
		ON (target.[PIPoint] = source.[PIPoint] AND target.[UserName] = source.[UserName])
		WHEN MATCHED THEN
		UPDATE SET [Timestamp] = source.[Timestamp],[Value] = source.[Value],[Operator] = source.[Operator],[TriggerCondition] = source.[TriggerCondition],[UserName] = source.[UserName],[CellNumber] = source.[CellNumber]
		WHEN NOT MATCHED THEN
		INSERT ([PIPoint],[Timestamp],[Value],[Operator],[TriggerCondition],[UserName],[CellNumber])
		VALUES (source.[PIPoint],source.[Timestamp],source.[Value],source.[Operator],source.[TriggerCondition],source.[UserName],source.[CellNumber]);
')
END

GO


