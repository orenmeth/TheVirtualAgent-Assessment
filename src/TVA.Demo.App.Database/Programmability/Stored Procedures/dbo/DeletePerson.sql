CREATE PROCEDURE [dbo].[DeletePerson]
    @code INT,
    @delete_related_accounts_and_transactions BIT = 0 -- 0: No, will fail if accounts exist; 1: Yes, will delete related accounts and transactions
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Persons WHERE code = @code)
        BEGIN
            RAISERROR('Person code not found.', 16, 1);
            RETURN;
        END

        IF @delete_related_accounts_and_transactions = 1
        BEGIN
            DELETE FROM dbo.Transactions
            WHERE account_code IN (SELECT code FROM dbo.Accounts WHERE person_code = @code);

            DELETE FROM dbo.Accounts
            WHERE person_code = @code;
        END

        DELETE FROM dbo.Persons
        WHERE code = @code;
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
