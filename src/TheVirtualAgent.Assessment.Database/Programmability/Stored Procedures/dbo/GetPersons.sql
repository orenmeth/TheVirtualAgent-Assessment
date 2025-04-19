CREATE OR ALTER PROCEDURE [dbo].[GetPersons]
AS
BEGIN
    SELECT
        p.code,
        p.name,
        p.surname,
        p.id_number
    FROM
        dbo.Persons p WITH(NOLOCK);
END;
GO
