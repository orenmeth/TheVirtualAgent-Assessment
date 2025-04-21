CREATE PROCEDURE [dbo].[GetPerson]
    @code INT
AS
BEGIN
    SELECT
        p.code,
        p.[name],
        p.surname,
        p.id_number
    FROM
        dbo.Persons p WITH(NOLOCK)
    WHERE
        p.code = @code;
END;
GO

