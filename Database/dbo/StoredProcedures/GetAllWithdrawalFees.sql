CREATE PROCEDURE GetAllWithdrawalFees
AS
BEGIN
	SET NOCOUNT ON;

SELECT C.Name, SUM(W.Fee)
FROM Withdraw W
JOIN Currency C ON C.Id = W.CurrencyId
GROUP BY C.Name
END
GO
