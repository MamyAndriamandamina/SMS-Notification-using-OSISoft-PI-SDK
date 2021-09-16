USE [master]
GO

/****** Object:  StoredProcedure [dbo].[ReadSMSTable]    Script Date: 14/09/2021 4:11:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Mamy ANDRIAMANDAMINA>
-- Create date: <25-MAR-2021>
-- Description:	<SMS Notification Project: used to read all PI Tags from PI point list table configuration>
-- =============================================
CREATE PROCEDURE [dbo].[ReadSMSTable]
AS
declare @ColumnName nvarchar(200)
declare @TableName nvarchar(200)
set @ColumnName = 'PIPointName'
set @TableName = 'PIPointList'
	BEGIN
		EXEC('SELECT '+ @ColumnName +' FROM [SMS].dbo.' + @TableName + '')
	END
GO


