﻿CREATE PROCEDURE [dbo].[GetAccounts]
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            a.code,
            a.person_code,
            a.account_number,
            a.outstanding_balance,
            a.account_status_id
        FROM
            dbo.Accounts a WITH(NOLOCK)
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