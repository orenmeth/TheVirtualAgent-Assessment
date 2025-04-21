CREATE PROCEDURE [dbo].[GetAccount]
    @code INT
AS
BEGIN
    SELECT
        a.code,
        a.person_code,
        a.account_number,
        a.outstanding_balance
    FROM
        dbo.Accounts a WITH(NOLOCK)
    WHERE
        a.code = @code;
END;
GO