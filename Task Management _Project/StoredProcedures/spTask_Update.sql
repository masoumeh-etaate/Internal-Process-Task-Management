USE [InternalProcessDb]
GO

/****** Object:  StoredProcedure [dbo].[Task_Update]    Script Date: 12/6/2025 2:51:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Task_Update]
    @TaskId INT,
    @Title NVARCHAR(250),
    @Description NVARCHAR(MAX),
    @AssignedToUserId INT = NULL,
    @Status NVARCHAR(50),
    @Priority NVARCHAR(50),
    @DueDate DATETIME2 = NULL,
    @ChangedByUserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldTitle NVARCHAR(250) = (SELECT Title FROM Tasks WHERE TaskId = @TaskId);

    UPDATE Tasks
    SET 
        Title = @Title,
        Description = @Description,
        AssignedToUserId = @AssignedToUserId,
        Status = @Status,
        Priority = @Priority,
        DueDate = @DueDate
    WHERE TaskId = @TaskId;

    INSERT INTO TaskHistory (TaskId, ChangedByUserId, ChangeType, OldValue, NewValue)
    VALUES (@TaskId, @ChangedByUserId, 'Update', @OldTitle, @Title);
END
GO

