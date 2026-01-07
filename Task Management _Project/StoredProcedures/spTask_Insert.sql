USE [InternalProcessDb]
GO

/****** Object:  StoredProcedure [dbo].[Task_Insert]    Script Date: 12/6/2025 2:52:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Task_Insert]
    @Title NVARCHAR(250),
    @Description NVARCHAR(MAX) = NULL,
    @CreatedByUserId INT,
    @AssignedToUserId INT = NULL,
    @Status NVARCHAR(50) = 'New',
    @Priority NVARCHAR(50) = NULL,
    @DueDate DATETIME2 = NULL,
    @NewTaskId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Tasks (Title, Description, CreatedByUserId, AssignedToUserId, Status, Priority, DueDate)
    VALUES (@Title, @Description, @CreatedByUserId, @AssignedToUserId, @Status, @Priority, @DueDate);

    SET @NewTaskId = SCOPE_IDENTITY();

    INSERT INTO TaskHistory (TaskId, ChangedByUserId, ChangeType, OldValue, NewValue)
    VALUES (@NewTaskId, @CreatedByUserId, 'Create', NULL, @Title);
END
GO

