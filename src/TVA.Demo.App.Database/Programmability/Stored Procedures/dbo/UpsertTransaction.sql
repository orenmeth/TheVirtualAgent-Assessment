CREATE PROCEDURE [dbo].[UpsertTransaction]
    @code INT = NULL,
    @account_code INT,
    @transaction_date DATETIME,
    @amount MONEY,
    @description VARCHAR(100),
    @RETURN_CODE INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
    BEGIN TRANSACTION;
        -- DO NOT ALLOW transactions with an amount of zero
        IF @amount = 0
        BEGIN
            RAISERROR('Transaction amount cannot be zero.', 16, 1);
            RETURN;
        END

        -- DO NOT PROCEED if the account does not exist.
        IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE code = @account_code)
        BEGIN
            RAISERROR('Account code not found.', 16, 1);
            RETURN;
        END

        -- DO NOT allow transactions to be posted to closed accounts
        IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE code = @account_code AND account_status_id = 2)
        BEGIN
            RAISERROR('Transaction cannot be posted to a closed account.', 16, 1);
            RETURN;
        END

        IF CAST(@transaction_date AS DATE) > CAST(GETDATE() AS DATE)
        BEGIN
        RAISERROR('Transaction date cannot be in the future.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM dbo.Transactions WHERE code = @code)
        BEGIN
            -- Store the original account row in the audit table
            INSERT INTO dbo.AccountsAudit (code, person_code, account_number, outstanding_balance, account_status_id, change_date, change_reason)
            SELECT code, person_code, account_number, outstanding_balance, account_status_id, GETDATE(), 'U'
            FROM dbo.Accounts WHERE code = @account_code;

            -- Revert previous outstanding balance change when updating
            DECLARE @original_change_amount MONEY
            SET @original_change_amount = (SELECT TOP 1 amount FROM dbo.Transactions WHERE code = @code)

            UPDATE dbo.Transactions
            SET
                account_code = @account_code,
                transaction_date = @transaction_date,
                capture_date = GETDATE(),
                amount = @amount,
                [description] = @description
            WHERE
                code = @code;
            SET @RETURN_CODE = @code;

            UPDATE dbo.Accounts
            SET outstanding_balance = outstanding_balance - @original_change_amount + @amount
            WHERE
                code = @account_code;
        END
        ELSE
        BEGIN
            -- Store the original account row in the audit table
            INSERT INTO dbo.AccountsAudit (code, person_code, account_number, outstanding_balance, account_status_id, change_date, change_reason)
            SELECT code, person_code, account_number, outstanding_balance, account_status_id, GETDATE(), 'I'
            FROM dbo.Accounts WHERE code = @account_code;

            INSERT INTO dbo.Transactions (account_code, transaction_date, capture_date, amount, [description])
            VALUES (@account_code, @transaction_date, GETDATE(), @amount, @description);
            SET @RETURN_CODE = SCOPE_IDENTITY();

            UPDATE dbo.Accounts
            SET outstanding_balance = outstanding_balance + @amount
            WHERE
                code = @account_code;
        END
    COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
        SET @RETURN_CODE = -1;
        RETURN -1;
    END CATCH
END
GO
