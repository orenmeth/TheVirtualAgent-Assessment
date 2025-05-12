CREATE PROCEDURE [dbo].[GetTransactionsByAccountCode]
    @account_code INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE code = @account_code)
        BEGIN
            RAISERROR('Account code not found.', 16, 1);
            RETURN;
        END

        SELECT
            t.code,
            t.account_code,
            t.transaction_date,
            t.capture_date,
            t.amount,
            t.[description]
        FROM
            dbo.Transactions t WITH(NOLOCK)
        WHERE
            t.account_code = @account_code;
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
