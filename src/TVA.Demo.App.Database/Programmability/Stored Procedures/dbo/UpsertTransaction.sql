CREATE PROCEDURE [dbo].[UpsertTransaction]
    @code INT = NULL,
    @account_code INT,
    @transaction_date DATETIME,
    @amount MONEY,
    @description VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM dbo.Transactions WHERE code = @code)
        BEGIN
            UPDATE dbo.Transactions
            SET
                account_code = @account_code,
                transaction_date = @transaction_date,
                capture_date = GETUTCDATE(),
                amount = @amount,
                [description] = @description
            WHERE
                code = @code;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.Transactions (account_code, transaction_date, capture_date, amount, [description])
            VALUES (@account_code, @transaction_date, GETUTCDATE(), @amount, @description);
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
