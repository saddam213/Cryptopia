CREATE PROCEDURE [dbo].[GetMarketItemQuestions]
      @MarketItemId INT = 0
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
		 mq.Id
		,ms.UserName
		,mq.Question
		,mq.Answer
		,mq.Timestamp
	FROM MarketItemQuestion mq
	JOIN [User] ms ON ms.Id = mq.UserId
	WHERE mq.MarketItemId = @MarketItemId
	ORDER By Timestamp DESC
    
RETURN

GO



