USE [InternalProcessDb]
GO

/****** Object:  StoredProcedure [dbo].[Task_Search]    Script Date: 12/6/2025 2:52:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Task_Search]
    @Status NVARCHAR(50) = NULL,
    @AssignedToUserId INT = NULL,
    @Search NVARCHAR(250) = NULL
AS
BEGIN
    SELECT t.*
    FROM Tasks t
    WHERE 
        (@Status IS NULL OR t.Status = @Status)
        AND (@AssignedToUserId IS NULL OR t.AssignedToUserId = @AssignedToUserId)
        AND (
                @Search IS NULL 
                OR t.Title LIKE '%' + @Search + '%'
                OR t.Description LIKE '%' + @Search + '%'
            )
    ORDER BY t.CreatedAt DESC;
END
GO

