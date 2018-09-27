using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.API.Logging.Base;
using Cryptopia.Logging;
using System.Collections.Generic;
using Cryptopia.API.DataAccess;
using Cryptopia.API.Objects;
using System.Runtime.CompilerServices;
using Cryptopia.Common;
using System.Net.Mail;
using System.Net;
using Cryptopia.Enums;

namespace Cryptopia.PoolService.Implementation
{
	/// <summary>
	/// Class for polling database for market auctions
	/// </summary>
	public class MarketMonitor
    {
        #region Fields

        /// <summary>
        /// indicates if tracking is enabled
        /// </summary>
        private bool _isEnabled;

        /// <summary>
        /// The short poll period (seconds)
        /// </summary>
        private readonly int _pollPeriod;

        /// <summary>
        /// The log instance
        /// </summary>
        private readonly Log Log = LoggingManager.GetLog(typeof(MarketMonitor));

        private string _emailUser;
        private string _emailPass;
        private string _emailServer;
        private int _emailPort;

        private List<HubEmailTemplate> _emailTemplates = new List<HubEmailTemplate>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketMonitor" /> class.
        /// </summary>
        /// <param name="pollPeriod">The poll period.</param>
        public MarketMonitor(int pollPeriod, string emailUser, string emailPass, string emailServer, int emailPort)
        {
            _pollPeriod = pollPeriod;
            _emailUser = emailUser;
            _emailPass = emailPass;
            _emailServer = emailServer;
            _emailPort = emailPort;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the market poller.
        /// </summary>
        public void Start()
        {
            if (_isEnabled)
            {
                return;
            }

            _isEnabled = true;
            Log.Message(LogLevel.Info, "[Start] - Starting market poll, Period: {0} Seconds.", _pollPeriod);
            PollMarkets();
        }

        /// <summary>
        /// Stops the market poller
        /// </summary>
        public void Stop()
        {
            Log.Message(LogLevel.Info, "[Stop] - Stopping market poll loop.");
            _isEnabled = false;
        }


        private async void PollMarkets()
        {
            while (_isEnabled)
            {
                await Task.Delay(TimeSpan.FromSeconds(_pollPeriod));
                await QueryMarkets();
            }
        }


        /// <summary>
        /// Queries the deposits.
        /// </summary>
        /// <param name="isShortPoll">if set to <c>true</c> is shortpoll.</param>
        /// <returns></returns>
        private async Task QueryMarkets()
        {
            try
            {
                Log.Message(LogLevel.Info, "QueryMarkets...");

                using (var userRepo = new Repository<User>())
                using (var currencyRepo = new Repository<Currency>())
                using (var marketItemRepo = new Repository<MarketItem>())
                using (var marketItemBidRepo = new Repository<MarketItemBid>())
                {
                    var marketItems = await marketItemRepo.GetAllAsync(x => DateTime.UtcNow >= x.CloseDate && x.Status == MarketItemStatus.Active);
                    foreach (var marketItem in marketItems)
                    {
                        try
                        {
                            Log.Message(LogLevel.Info, "MarketItem closed, MarketItemId: {0}, Type: {1}", marketItem.Id, marketItem.Type);
                            string marketItemLink = string.Format("https://www.cryptopia.co.nz/MarketItem/{0}", marketItem.Id);
                            var seller = await userRepo.GetAsync(x => x.Id == marketItem.UserId);
                            if (marketItem.Type == MarketItemType.Auction)
                            {
                                var marketItemBids = await marketItemBidRepo.GetAllAsync(x => x.MarketItemId == marketItem.Id);
                                if (marketItemBids != null && marketItemBids.Any())
                                {
                                    var topBid = marketItemBids.MaxBy(x => x.BidAmount);
                                    if (topBid.BidAmount >= marketItem.ReservePrice)
                                    {
                                        Log.Message(LogLevel.Info, "Auction highest bid has met reserve, Processing sale, MarketItemId: {0}", marketItem.Id);
                                        topBid.IsWinningBid = true;
                                        marketItem.Status = MarketItemStatus.Complete;
                                        var buyer = await userRepo.GetAsync(x => x.Id == topBid.UserId);
                                        var currency = await currencyRepo.GetAsync(x => x.Id == marketItem.CurrencyId);

                                        await marketItemRepo.SaveAsync();
                                        await marketItemBidRepo.SaveAsync();
                                        string price = string.Format("{0} {1}", topBid.BidAmount, currency.Symbol);

                                        Log.Message(LogLevel.Debug, "Sending MarketPlaceAuctionSold email to seller, Email: {0}", seller.Email);
                                        await SendEmailAsync(EmailTemplateType.MarketPlaceAuctionSold, seller.Email, seller.UserName, marketItem.Title, marketItemLink, price, buyer.UserName, buyer.Email);

                                        Log.Message(LogLevel.Debug, "Sending MarketPlaceAuctionWon email to buyer, Email: {0}", buyer.Email);
                                        await SendEmailAsync(EmailTemplateType.MarketPlaceAuctionWon, buyer.Email, buyer.UserName, marketItem.Title, marketItemLink, price, seller.UserName, seller.Email);
                                    }
                                    else
                                    {
                                        Log.Message(LogLevel.Info, "Auction highest bid has not met reserve, Closing auction, MarketItemId: {0}", marketItem.Id);
                                        marketItem.Status = MarketItemStatus.Closed;
                                        await marketItemRepo.SaveAsync();

                                        Log.Message(LogLevel.Debug, "Sending MarketPlaceAuctionSold email to seller, Email: {0}", seller.Email);
                                        await SendEmailAsync(EmailTemplateType.MarketPlaceAuctionClosed, seller.Email, seller.UserName, marketItem.Title, marketItemLink);
                                    }
                                }
                                else
                                {
                                    Log.Message(LogLevel.Info, "Auction no bids found reserve not met, Closing auction, MarketItemId: {0}", marketItem.Id);
                                    marketItem.Status = MarketItemStatus.Closed;
                                    await marketItemRepo.SaveAsync();

                                    Log.Message(LogLevel.Debug, "Sending MarketPlaceAuctionSold email to seller, Email: {0}", seller.Email);
                                    await SendEmailAsync(EmailTemplateType.MarketPlaceAuctionClosed, seller.Email, seller.UserName, marketItem.Title, marketItemLink);
                                }
                            }
                            else if (marketItem.Type == MarketItemType.BuySell)
                            {

                                Log.Message(LogLevel.Info, "BuySell item did not sell, Closing item, MarketItemId: {0}", marketItem.Id);
                                marketItem.Status = MarketItemStatus.Closed;
                                await marketItemRepo.SaveAsync();

                                Log.Message(LogLevel.Debug, "Sending MarketPlaceAuctionSold email to seller, Email: {0}", seller.Email);
                                await SendEmailAsync(EmailTemplateType.MarketPlaceClosed, seller.Email, seller.UserName,marketItem.Title, marketItemLink);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception("An exception occured updating MarketItem, MarketItemId: {0}", ex, marketItem.Id);
                            LogDatabaseError(ex, "Failed to update MarketItem");
                        }
                    }

                }

                Log.Message(LogLevel.Info, "QueryMarkets complete.");
            }
            catch (Exception ex)
            {
                Log.Exception("An exception occured processing markets", ex);
                LogDatabaseError(ex, "Failed to query MarketItems");
            }
        }



        /// <summary>
        /// Logs an error to the database.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <param name="memberName">Name of the member.</param>
        private void LogDatabaseError(Exception ex, string message, [CallerMemberName]string memberName = "")
        {
            try
            {
                using (var logRepository = new Repository<ErrorLog>())
                {
                    var logmessage = logRepository.CreateInstance();
                    logmessage.Component = "PoolService";
                    logmessage.Method = memberName;
                    logmessage.Request = message;
                    logmessage.Exception = ex != null ? ex.ToString() : string.Empty;
                    logmessage.Timestamp = DateTime.UtcNow;
                    logRepository.Insert(logmessage);
                    logRepository.Save();
                }
            }
            catch { }
        }

		///// <summary>
		///// Sends the notification.
		///// </summary>
		///// <param name="notification">The notification.</param>
		///// <returns></returns>
		//private async Task SendNotification(MarketNotification notification)
		//{
		//	if (notification != null)
		//	{
		//		Log.Message(LogLevel.Info, "Sending notification, UserId: {0}, Notification: {1} - {2}", notification.UserId, notification.Header, notification.Message);
		//		using (var notificationService = new NotificationServiceClient())
		//		{
		//			await notificationService.SendUserNotificationAsync(NotificationType.Info, notification.UserId, notification.Header, notification.Message);
		//		}
		//	}
		//}

      

        protected async Task SendEmailAsync(EmailTemplateType type, string destination, params object[] formatParameters)
        {
            try
            {
                var template = _emailTemplates.FirstOrDefault(x => x.Type == type);
                if (template == null)
                {
                    using (var repo = new Repository<HubEmailTemplate>(Connection.CryptopiaHub))
                    {
                        template = await repo.GetAsync(x => x.Type == type);
                        if (template != null)
                        {
                            _emailTemplates.Add(template);
                        }
                    }
                }

                var emailParameters = new List<object>();
                emailParameters.AddRange(formatParameters);
                using (var email = new MailMessage(new MailAddress(_emailUser, "Cryptopia"), new MailAddress(destination)))
                {
                    email.Subject = template.Subject;
                    email.Body = string.Format(template.Template, formatParameters.ToArray());
                    email.IsBodyHtml = template.IsHtml;

                    using (var mailClient = new SmtpClient(_emailServer, _emailPort))
                    {
                        mailClient.Credentials = new NetworkCredential(_emailUser, _emailPass);
                        mailClient.EnableSsl = true;
                        await mailClient.SendMailAsync(email);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("Failed to send marketplace email, Type: {0}, Email: {1}", ex, type, destination);
                LogDatabaseError(ex, string.Format("Failed to send marketplace email, Type: {0}, Email: {1}", type, destination));
            }
        }



        #endregion
    }

    public class MarketNotification
    {
        public Guid UserId { get; set; }
        public string Worker { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }

        public MarketNotification(Guid userId, string header, string message)
        {
            UserId = userId;
            Header = header;
            Message = message;
        }

        public MarketNotification(string worker, string header, string message)
        {
            Worker = worker;
            Header = header;
            Message = message;
        }
    }
}
