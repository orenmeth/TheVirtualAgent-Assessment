CREATE OR ALTER PROCEDURE GetTransaction
    @code INT
AS
BEGIN
    SELECT
        t.code,
        t.account_code,
        t.transaction_date,
        t.capture_date,
        t.amount,
        t.[description]
    FROM
        dbo.Transactions t WITH(NOLOCK)
    WHERE
        t.code = @code;
END;