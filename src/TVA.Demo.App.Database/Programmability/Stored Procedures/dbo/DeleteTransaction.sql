CREATE PROCEDURE [dbo].[DeleteTransaction]
    @code INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN
            DELETE FROM dbo.Transactions
            WHERE code = @code;
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

