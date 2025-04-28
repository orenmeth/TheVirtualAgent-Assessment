CREATE PROCEDURE [dbo].[GetTransactions]
AS
BEGIN

    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            t.code,
            t.account_code,
            t.transaction_date,
            t.capture_date,
            t.amount,
            t.[description]
        FROM
            dbo.Transactions t WITH(NOLOCK)
    END TRY

    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;
        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO