USE [InternalProcessDb]
GO

/****** Object:  StoredProcedure [dbo].[Task_GetById]    Script Date: 12/6/2025 2:54:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Task_GetById]
    @TaskId INT
AS
BEGIN
    SELECT 
        t.*,
        u.Username AS CreatedByName,
        a.Username AS AssignedToName
    FROM Tasks t
        LEFT JOIN Users u ON u.UserId = t.CreatedByUserId
        LEFT JOIN Users a ON a.UserId = t.AssignedToUserId
    WHERE t.TaskId = @TaskId;
END
GO

