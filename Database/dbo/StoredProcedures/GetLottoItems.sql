CREATE PROCEDURE [dbo].[GetLottoItems]
	@UserId UNIQUEIDENTIFIER
AS
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	SELECT
		 li.Id AS LottoItemId 
		,li.Name AS Name 
		,li.[Description] AS [Description]
		,li.Fee AS Fee
		,li.[Hours] AS [Hours]
		,li.IsEnabled AS IsEnabled
		,li.LottoType AS LottoType
		,li.Rate AS Rate
		,li.StartDate AS StartDate
		,li.NextDraw AS NextDraw
		,li.Prizes AS Prizes
		,c.Symbol AS Symbol
		,li.CharityFee AS CharityFee
		,li.IsEnabled AS IsEnabled
		,li.CurrentDrawId
		,(SELECT COUNT(1) FROM LottoTicket WITH (NOLOCK) WHERE LottoItemId = li.Id AND IsArchived = 0) AS TicketsInDraw
		,(SELECT COUNT(1) FROM LottoTicket WITH (NOLOCK) WHERE LottoItemId = li.Id AND IsArchived = 0 AND UserId = @UserId) AS UserTicketsInDraw
	FROM [dbo].[LottoItem] li WITH (NOLOCK)
	JOIN [dbo].[Currency] c WITH (NOLOCK) ON c.Id = li.CurrencyId
	WHERE c.IsEnabled = 1
	
RETURN