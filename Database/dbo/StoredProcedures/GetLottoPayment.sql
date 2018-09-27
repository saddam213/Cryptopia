CREATE PROCEDURE [dbo].[GetLottoPayment]
	@LottoItemId INT,
	@UserId UNIQUEIDENTIFIER
AS  
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
		li.CurrencyId,
		li.Name,
		li.[Description],
		li.Rate,
		c.Symbol,
		b.Total - (b.HeldForTrades + b.PendingWithdraw + b.Unconfirmed) AS Balance 
	FROM LottoItem li WITH (NOLOCK)
	JOIN Currency c WITH (NOLOCK) ON c.Id = li.CurrencyId
	JOIN Balance b WITH (NOLOCK) ON b.CurrencyId = li.CurrencyId AND b.UserId = @UserId
	WHERE li.Id = @LottoItemId

RETURN