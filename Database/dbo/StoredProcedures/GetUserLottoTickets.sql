CREATE PROCEDURE [dbo].[GetUserLottoTickets]
	@UserId UNIQUEIDENTIFIER = NULL
AS  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	SELECT
		li.Id AS LottoItemId,
		lt.Id AS LottoTicketId,
		lt.DrawId AS LottoDrawId,
		li.Name,
		c.Symbol,
		li.Rate,
		li.NextDraw
	FROM [dbo].[LottoTicket] lt WITH (NOLOCK)
	JOIN [dbo].[LottoItem] li WITH (NOLOCK) ON li.Id = lt.LottoItemId
	JOIN [dbo].[Currency] c WITH (NOLOCK) ON c.Id = li.CurrencyId
	WHERE lt.UserId = @UserId AND lt.IsArchived = 0
RETURN
