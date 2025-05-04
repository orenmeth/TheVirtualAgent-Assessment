CREATE PROCEDURE [dbo].[GetAccountsByPersonCode]
    @person_code INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Persons WHERE code = @person_code)
        BEGIN
            RAISERROR('Person code not found.', 16, 1);
            RETURN;
        END

        SELECT
            a.code,
            a.person_code,
            a.account_number,
            a.outstanding_balance
        FROM
            dbo.Accounts a WITH(NOLOCK)
        WHERE
            a.person_code = @person_code;
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
