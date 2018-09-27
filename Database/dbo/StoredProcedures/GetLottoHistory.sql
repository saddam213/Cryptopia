CREATE PROCEDURE [dbo].[GetLottoHistory]
	@UserId UNIQUEIDENTIFIER = NULL
AS  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	SELECT
		li.Id AS LottoItemId,
		u.ChatHandle AS [User],
		li.Name,
		c.Symbol,
		lh.Amount,
		lh.[Percent],
		lh.Position,
		li.Fee,
		li.CharityFee,
		lh.LottoTicketId,
		lh.LottoDrawId,
		lh.[Timestamp]
	FROM [dbo].[LottoHistory] lh WITH (NOLOCK)
	JOIN [dbo].[User] u WITH (NOLOCK) ON u.Id = lh.UserId
	JOIN [dbo].[LottoItem] li WITH (NOLOCK) ON li.Id = lh.LottoItemId
	JOIN [dbo].[Currency] c WITH (NOLOCK) ON c.Id = li.CurrencyId
	WHERE @UserId IS NULL OR u.Id = @UserId
RETURN