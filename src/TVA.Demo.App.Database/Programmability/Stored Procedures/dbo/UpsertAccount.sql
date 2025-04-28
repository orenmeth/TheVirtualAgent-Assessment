CREATE PROCEDURE [dbo].[UpsertAccount]
    @code INT = NULL,
    @person_code INT,
    @account_number VARCHAR(50),
    @outstanding_balance MONEY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE code = @code)
        BEGIN
            UPDATE dbo.Accounts
            SET
                person_code = @person_code,
                account_number = @account_number,
                outstanding_balance = @outstanding_balance
            WHERE
                code = @code;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.Accounts (person_code, account_number, outstanding_balance)
            VALUES (@person_code, @account_number, @outstanding_balance);
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
