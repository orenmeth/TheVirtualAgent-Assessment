CREATE PROCEDURE [dbo].[UpsertPerson]
    @code INT = NULL,
    @first_name VARCHAR(50),
    @last_name VARCHAR(50),
    @id_number VARCHAR(50),
    @RETURN_CODE INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
    BEGIN TRANSACTION;
        /*
          Check if the ID number is valid
          1. If we are inserting, check if the ID number already exists in the database.
          2. If we are updating, check if the ID number already exists for another person.
        */
        IF EXISTS (SELECT 1 FROM dbo.Persons WHERE id_number = @id_number AND (@code IS NULL OR code != @code))
        BEGIN
            RAISERROR('ID number conflict. The provided ID number is already in use.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM dbo.Persons WHERE code = @code)
        BEGIN
            UPDATE dbo.Persons
            SET
                [name] = @first_name,
                surname = @last_name,
                id_number = @id_number
            WHERE
                code = @code;
            SET @RETURN_CODE = @code;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.Persons ([name], surname, id_number)
            VALUES (@first_name, @last_name, @id_number);
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
