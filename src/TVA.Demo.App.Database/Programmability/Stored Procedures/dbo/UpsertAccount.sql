CREATE PROCEDURE [dbo].[UpsertAccount]
    @code INT = NULL,
    @person_code INT,
    @account_number VARCHAR(50),
    @outstanding_balance DECIMAL(18, 2),
    @account_status_id INT,
    @RETURN_CODE INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
    BEGIN TRANSACTION;
        -- DO NOT PROCEED if the person does not exist.
        IF NOT EXISTS (SELECT 1 FROM dbo.Persons WHERE code = @person_code)
        BEGIN
            RAISERROR('Person code not found.', 16, 1);
            RETURN;
        END

        /*
          Check if the account number is valid
          1. If we are inserting, check if the account number already exists in the database.
          2. If we are updating, check if the account number already exists for another account.
        */
        IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE account_number = @account_number AND (@code IS NULL OR code != @code))
        BEGIN
            RAISERROR('Account number conflict. The provided account number is already in use.', 16, 1);
            RETURN;
        END

        -- DO NOT allow accounts to be closed if they have a non-zero balance
        IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE code = @code AND outstanding_balance != 0 AND @account_status_id = 2)
        BEGIN
            RAISERROR('Account cannot be closed while it has a non-zero balance.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE code = @code)
        BEGIN
            -- Store the original row in the audit table
            INSERT INTO dbo.AccountsAudit (code, person_code, account_number, outstanding_balance, account_status_id, change_date, change_reason)
            SELECT code, person_code, account_number, outstanding_balance, account_status_id, GETDATE(), 'U'
            FROM dbo.Accounts WHERE code = @code;

            UPDATE dbo.Accounts
            SET
                person_code = @person_code,
                account_number = @account_number,
                account_status_id = @account_status_id
            WHERE
                code = @code;
            SET @RETURN_CODE = @code;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.Accounts (person_code, account_number, outstanding_balance, account_status_id)
            VALUES (@person_code, @account_number, CAST(@outstanding_balance AS MONEY), @account_status_id);
            SET @RETURN_CODE = SCOPE_IDENTITY();
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
