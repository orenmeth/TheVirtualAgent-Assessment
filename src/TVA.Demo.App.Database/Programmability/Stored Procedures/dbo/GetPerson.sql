CREATE PROCEDURE dbo.GetPerson
	@code INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
	    SELECT
            p.code,
            p.[name],
            p.surname,
            p.id_number
        FROM
            dbo.Persons p WITH(NOLOCK)
        WHERE
            p.code = @code
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