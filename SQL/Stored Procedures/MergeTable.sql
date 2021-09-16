USE [master]
GO

/****** Object:  StoredProcedure [dbo].[MergeTable]    Script Date: 14/09/2021 4:20:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Mamy Razanamparany ANDRIAMANDAMINA>
-- Create date: <04-APR-2018>
-- Updated date: <14-SEP-2021>
-- Description:	<Using the power of SQL Merge Statement to update, insert values into PI Point SnapShot Table>
-- =============================================
CREATE PROCEDURE [dbo].[MergeTable] (@streams nvarchar(MAX))
--stream = (PIPoint, Timestamp, Value)
--streams = (PIPoint, Timestamp, Value),(PIPoint, Timestamp, Value),(PIPoint, Timestamp, Value)
AS
BEGIN
	SET NOCOUNT ON;
	Exec('
	BEGIN		
		MERGE [SMS].[dbo].[PIPointSnapshot] AS target 
		USING 
		(VALUES 
			'+@streams+'
		)
		AS source ([PIPoint], [Timestamp], [Value])
		ON (target.[PIPoint] = source.[PIPoint])
		WHEN MATCHED THEN
		UPDATE SET [Timestamp] = source.[Timestamp],[Value] = source.[Value]
		WHEN NOT MATCHED THEN
		INSERT ([PIPoint], [Timestamp], [Value])
		VALUES (source.[PIPoint],source.[Timestamp], source.[Value]);
		END
	')
END
GO


