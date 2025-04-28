CREATE PROCEDURE [dbo].[UpsertPerson]
    @code INT = NULL,
    @first_name VARCHAR(50),
    @last_name VARCHAR(50),
    @id_number DATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM dbo.Persons WHERE code = @code)
        BEGIN
            UPDATE dbo.Persons
            SET
                [name] = @first_name,
                surname = @last_name,
                id_number = @id_number
            WHERE
                code = @code;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.Persons ([name], surname, id_number)
            VALUES (@first_name, @last_name, @id_number);
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
