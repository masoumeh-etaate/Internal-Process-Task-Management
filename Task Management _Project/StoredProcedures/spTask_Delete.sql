USE [InternalProcessDb]
GO

/****** Object:  StoredProcedure [dbo].[Task_Delete]    Script Date: 12/6/2025 2:55:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Task_Delete]
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- حذف تاریخچه تسک
    DELETE FROM TaskHistory WHERE TaskId = @TaskId;

    -- حذف تسک
    DELETE FROM Tasks WHERE TaskId = @TaskId;
END
GO

