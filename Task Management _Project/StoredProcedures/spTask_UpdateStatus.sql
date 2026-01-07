USE [InternalProcessDb]
GO

/****** Object:  StoredProcedure [dbo].[Task_UpdateStatus]    Script Date: 12/6/2025 2:56:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Task_UpdateStatus]
    @TaskId INT,
    @Status NVARCHAR(50),
    @UpdatedByUserId INT
AS
BEGIN
    UPDATE dbo.Tasks
    SET Status = @Status,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedAt = GETUTCDATE()
    WHERE TaskId = @TaskId;
END
GO

