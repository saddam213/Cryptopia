using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Enums
{
	public enum EmailTemplateType
	{
		Registration = 0,
		LogonSuccess = 1,
		LogonFail = 2,
		Lockout = 3,
		PasswordReset = 4,
		LockoutWithReset = 5,
		LockoutByUser = 6,
		LockoutByUserWithReset = 7,


		ConfirmWithdraw = 20,
		TwoFactorLogin = 40,
		TwoFactorSettingsUnlock = 41,
		TwoFactorWithdraw = 42,
		TwoFactorTransfer = 43,
		TwoFactorTip = 44,
		SupportRequest = 50,
		SupportNewTicket = 51,
		SupportNewUserReply = 52,
		SupportNewAdminReply = 53,
		SupportTicketSubmitted = 54,
		SupportUnlockUser = 55,
		ContactRequest = 100,


		MarketPlaceNewFeedback = 200,
		MarketPlaceNewBid = 201,
		MarketPlaceNewSold = 202,
		MarketPlaceNewQuestion = 203,
		MarketPlaceNewAnswer = 204,
		MarketPlaceBought = 205,

		MarketPlaceAuctionSold = 207,
		MarketPlaceAuctionWon = 208,
		MarketPlaceAuctionClosed = 209,
		MarketPlaceClosed = 210,

		MarketPlaceWantedAccept = 211,
		MarketPlaceWantedAccepted = 212,

		UserAccountVerified = 300,
		UserAccountRejected = 301,
		TwoFactorReset = 302,
		AccountActivated = 303,

		CoinDelisting = 400
	}

	public enum SystemEmailType
	{
		Email_System = 0,
		Email_Contact = 1,
		Email_NoReply = 2
	}

	public enum TwoFactorType : byte
	{
		None = 0,
		[Display(Name = nameof(Resources.Authorization.twoFactorType_Email), ResourceType = typeof(Resources.Authorization))]
		EmailCode = 1,
		[Display(Name = nameof(Resources.Authorization.twoFactorType_Google), ResourceType = typeof(Resources.Authorization))]
		GoogleCode = 2,
		[Display(Name = nameof(Resources.Authorization.twoFactorType_Pin), ResourceType = typeof(Resources.Authorization))]
		PinCode = 3,
		[Display(Name = nameof(Resources.Authorization.twoFactorType_Password), ResourceType = typeof(Resources.Authorization))]
		Password = 10,
		[Display(Name = nameof(Resources.Authorization.twoFactorType_Questions), ResourceType = typeof(Resources.Authorization))]
		Question = 11,
		[Display(Name = nameof(Resources.Authorization.twoFactorType_Cryptopia), ResourceType = typeof(Resources.Authorization))]
		CryptopiaCode = 12
	}

	public enum TwoFactorComponent : byte
	{
		[Display(Name = nameof(Resources.Authorization.twoFactorComponent_Settings), ResourceType = typeof(Resources.Authorization))]
		Settings = 0,

		[Display(Name = nameof(Resources.Authorization.twoFactorComponent_Login), ResourceType = typeof(Resources.Authorization))]
		Login = 1,

		[Display(Name = nameof(Resources.Authorization.twoFactorComponent_Lockout), ResourceType = typeof(Resources.Authorization))]		
		Lockout = 5,

		[Display(Name = nameof(Resources.Authorization.twoFactorComponent_Withdraw), ResourceType = typeof(Resources.Authorization))]
		Withdraw = 20,

		[Display(Name = nameof(Resources.Authorization.twoFactorComponent_Transfer), ResourceType = typeof(Resources.Authorization))]		
		Transfer = 21,

		[Display(Name = nameof(Resources.Authorization.twoFactorComponent_Tip), ResourceType = typeof(Resources.Authorization))]
		Tip = 22,
	}

	public enum TwoFactorTokenType : byte
	{
		WithdrawConfirm = 0,
		WithdrawCancel = 1,
		UnlockAccount = 2,
		LockAccount,
		EmailConfirm
	}

	public enum SiteTheme
	{
		Light = 0,
		Dark = 1
	}

	public enum UserKarmaType
	{
		Tip,
		Chat,
		Other,
		Forum
	}

	public enum UserIgnoreListType
	{
		Tip = 0,
		Chat = 1,
		Message = 2,
		Forum = 3
	}

	public enum TransferType : byte
	{
		User = 0,
		Paytopia = 1,
		CoinVote = 2,
		Tip = 3,
		Faucet = 4,
		PayBan = 5,
		Reward = 6,
		ShareDividend = 10,
		ReferralBonus = 11,
		Lotto = 12,
		DustBin = 13,
		Mineshaft = 14,
		External = 15,
        CEFS = 16
	}


	public enum CurrencyStatus : byte
	{
		OK = 0,
		Maintenance = 1,
		NoConnections = 2,
		Offline = 5
	}

	public enum CurrencyListingStatus : byte
	{
		Active = 0,
		Delisting = 1,
		Delisted = 2
	}

	public enum NetworkType : byte
	{
		POW = 0,
		POS = 1,
		POP = 2,
		POR = 3,
		Hybrid = 4,
		Asset = 5,
		DPOS = 6,
		Other = 10
	}

	public enum WithdrawFeeType : byte
	{
		Normal = 0,
		Percent = 1,
		Computed = 2
	}

	public enum AlgoType : byte
	{
		Scrypt = 0,
		SHA256 = 1,
		X11 = 2,
		X13 = 3,
		X15 = 4,
		Scrypt_n = 5,
		Quark = 6,
		M7M = 7,
		Qubit = 8,
		Yescrypt = 9,
		NeoScrypt = 10,
		Groestl = 11,
		C11 = 12,
		CryptoNight = 13,
		Keccak = 14,
		Nist5 = 15,
		Skein = 16,
		SHA1 = 17,
		SHA2 = 18,
		SHA3 = 19,
		Lyra2RE = 20,
		Blake256 = 21,
		POS = 100,
		Other = 101,
		Scrypt_jane = 102,
		Scrypt_og = 103,
		Fugue = 104,
		Shavite3 = 105,
		Hefty1 = 106,
		Lyra2Zoin = 107,
		Ethash = 108,
		Equihash = 109,
		Dagger = 110,
		Blake2s = 111,
		None = 112
	}

	public enum WithdrawType : byte
	{
		Normal = 0,
		TermDeposit = 1,
		Other = 2,
	}

	public enum WithdrawStatus : byte
	{
		Pending = 0,
		Processing = 1,
		Complete = 2,
		Error = 3,
		Unconfirmed = 4,
		Canceled = 5
	}

	public enum DepositType : byte
	{
		Normal = 0,
		Transfer = 1,
		Mining = 2,
		ReferralBonus = 3,
		ShareHolderBonus = 4,
		MultiPool = 5,
		Other = 10,
	}

	public enum DepositStatus : byte
	{
		UnConfirmed = 0,
		Confirmed = 1,
		Invalid = 2
	}

	public enum MarketItemFeature : byte
	{
		Normal = 0,
		Featured = 10,
	}

	public enum MarketItemStatus : byte
	{
		Active = 0,
		Closed = 1,
		Complete = 10,
		Canceled = 11,
	}

	public enum MarketItemType : byte
	{
		[Display(Name = nameof(Resources.Market.itemTypeBuySell), ResourceType = typeof(Resources.Market))]
		BuySell = 0,
		[Display(Name = nameof(Resources.Market.itemTypeAuction), ResourceType = typeof(Resources.Market))]
		Auction = 1,
		[Display(Name = nameof(Resources.Market.itemTypeWanted), ResourceType = typeof(Resources.Market))]
		Wanted = 2
	}

	public enum SupportTicketCategory : byte
	{
		[Display(Name = nameof(Resources.Support.ticketCategoryGeneral), ResourceType = typeof(Resources.Support))]
		General = 0,
		[Display(Name = nameof(Resources.Support.ticketCategoryAccount), ResourceType = typeof(Resources.Support))]
		Account = 1,
		[Display(Name = nameof(Resources.Support.ticketCategoryDeposit), ResourceType = typeof(Resources.Support))]
		Deposit = 2,
		[Display(Name = nameof(Resources.Support.ticketCategoryWithdraw), ResourceType = typeof(Resources.Support))]
		Withdraw = 3,
		[Display(Name = nameof(Resources.Support.ticketCategoryExchange), ResourceType = typeof(Resources.Support))]
		Exchange = 4,
		[Display(Name = nameof(Resources.Support.ticketCategoryMining), ResourceType = typeof(Resources.Support))]
		Mining = 5,
		[Display(Name = nameof(Resources.Support.ticketCategoryMarketPlace), ResourceType = typeof(Resources.Support))]
		MarketPlace = 6,
		[Display(Name = nameof(Resources.Support.ticketCategoryYarrbitrage), ResourceType = typeof(Resources.Support))]
		Yarrbitrage = 7,
		[Display(Name = nameof(Resources.Support.ticketCategoryVoting), ResourceType = typeof(Resources.Support))]
		Voting = 8,
		[Display(Name = nameof(Resources.Support.ticketCategoryLotto), ResourceType = typeof(Resources.Support))]
		Lotto = 9,
		[Display(Name = nameof(Resources.Support.ticketCategoryOther), ResourceType = typeof(Resources.Support))]
		Other = 20
	}

	public enum SupportTicketStatus : byte
	{
		New = 0,
		UserReply,
		AdminReply,
		Closed,
		Reopened
	}

	public enum TradeStatus : byte
	{
		Pending = 0,
		Complete = 1,
		Partial = 2,
		Canceled = 3
	}

	public enum TradeHistoryType : byte
	{
		Buy = 0,
		Sell = 1,
		AutoBuy = 2,
		AutoSell = 3
	}

	public enum TradePairStatus : byte
	{
		OK = 0,
		Closing = 1,
		Paused = 2,
		Closed = 3
	}

	public enum ReportReason : byte
	{
		[Display(Name = nameof(Resources.User.messageReportReason_None), ResourceType = typeof(Resources.User))]
		None = 0,
		[Display(Name = nameof(Resources.User.messageReportReason_Spam), ResourceType = typeof(Resources.User))]
		Spam = 1,
		[Display(Name = nameof(Resources.User.messageReportReason_Abusive), ResourceType = typeof(Resources.User))]
		Abusive = 2,
		[Display(Name = nameof(Resources.User.messageReportReason_Advertizing), ResourceType = typeof(Resources.User))]
		Advertizing = 3,
		[Display(Name = nameof(Resources.User.messageReportReason_Suspicious), ResourceType = typeof(Resources.User))]
		Suspicious = 4,
		[Display(Name = nameof(Resources.User.messageReportReason_Dangerous), ResourceType = typeof(Resources.User))]
		Dangerous = 5,
		[Display(Name = nameof(Resources.User.messageReportReason_Other), ResourceType = typeof(Resources.User))]
		Other = 20
	}

	public enum DataNotificationType : byte
	{
		InboxMessage = 0,
		OutboxMessage = 1,
		BalanceUpdate = 2
	}

	public enum NotificationLevelType
	{
		Info = 0,
		Warning = 1,
		Error = 2,
		Success = 3
	}

	public enum NotificationType
	{
		Tip = 0,
		Trade = 1,
		Karma = 2,
		Message = 3,
		Reward = 4,
		Lotto = 5,
		Mineshaft = 6,
		Marketplace = 7
	}

	public enum PoolTargetBits : byte
	{
		Scrypt = 16,
		X11 = 24,
		sha256 = 32,
	}

	public enum PoolStatus : byte
	{
		OK = 0,
		Maintenance = 1,
		Expiring = 2,
		Expired = 3,
		Offline = 4
	}

	public enum PoolBlockStatus : byte
	{
		Unconfirmed = 0,
		Orphan = 1,
		Confirmed = 2,
		Pending = 3,
		Complete = 4,
		Error = 5
	}

	public enum PoolPayoutStatus : byte
	{
		Unconfirmed = 0,
		Orphan = 1,
		Confirmed = 2,
		Pending = 3,
		Complete = 4,
		Error = 5
	}

	public enum MineshaftDataNotificationType : byte
	{
		Statistics = 0,
		Block = 1,
		UserPayout = 2,
		UserStatistics = 3,
		UserWorkerStatistics = 4
	}

	public enum CurrencyType : byte
	{
		Bitcoin = 0,
		CryptoNote = 1,
		Ethereum = 2,
		Factom = 3,
		Pascal = 4,
		Fiat = 5
	}

	public enum ChatBanType
	{
		ChatOnly,
		TipOnly,
		All,
		Warning
	}

	public enum InterfaceType : byte
	{
		Bitcoin = 0,
		Proxtopia = 1,
		ProxtopiaNoAccount = 2,
	}

	public enum CancelTradeType : byte
	{
		Trade,
		TradePair,
		All
	}

	public enum ConvertDollarToBTCType
	{
		USD = 0,
		JPY = 1,
		CNY = 2,
		SGD = 3,
		HKD = 4,
		CAD = 5,
		NZD = 6,
		AUD = 7,
		CLP = 8,
		GBP = 9,
		DKK = 10,
		SEK = 11,
		ISK = 12,
		CHF = 13,
		BRL = 14,
		EUR = 15,
		RUB = 16,
		PLN = 17,
		THB = 18,
		KRW = 19,
		TWD = 20
	}

	public enum TradeNotificationType
	{
		TradePairData,
		TradePairUserData
	}

	public enum TransactionType
	{
		All,
		Deposit,
		Withdraw
	}

	public enum LottoType : byte
	{
		OneTime = 0,
		Recurring = 1,
		RecurringExpire = 2
		//Raffle = 2
	}

	public enum LottoItemStatus : byte
	{
		Active = 0,
		Finished = 1,
		Disabled = 2
	}

	public enum TradeUpdateAction
	{
		Add = 0,
		Remove = 1,
		Update = 2,
		Cancel = 3
	}

	public enum TradeDataType
	{
		Orderbook = 0,
		History = 1,
		OpenOrder = 2,
		Price = 3,
		Balance = 4
	}

	public enum TermDepositStatus : byte
	{
		Active = 0,
		PendingCancel = 1,
		Canceled = 2,
		Closed = 3
	}

	public enum TermDepositPaymentType : byte
	{
		Interest = 0,
		Final = 1,
		Cancel = 2
	}


	public enum PaytopiaItemType : byte
	{
		FeaturedPool = 0,
		FeaturedCurrency = 1,
		PoolListing = 2,
		TipSlot = 3,
		LottoSlot = 4,
		RewardSlot = 5,
		ExchangeListing = 6,
		ComboListing = 7,
		Shares = 8,
		Flair = 9,
		Avatar = 10,
		Emoticon = 11,
		TwoFactor = 12,
	}

	public enum PaytopiaItemCategory : byte
	{
		Genaral = 0,
		Listings = 1,
		Promotion = 2,
		Advertizing = 2
	}

	public enum PaytopiaItemPeriod : byte
	{
		Fixed = 0,
		Day = 1,
		Week = 2,
		Month = 3,
		Year = 5
	}

	public enum PaytopiaPaymentStatus : byte
	{
		Pending = 0,
		Complete = 1,
		Refunded = 2,
	}


	public enum NewsStatus : byte
	{
		Active = 0,
		Deleted = 1
	}

	public enum ReferralStatus : byte
	{
		Active = 0,
		Processing = 1,
		Complete = 2
	}

	public enum AlertType : byte
	{
		Info = 0,
		Success = 1,
		Warning = 2,
		Danger = 3
	}

	public enum TransferSearchType
	{
		ChatHandle,
		MiningHandle,
		UserName,
	}

	public enum ApprovalQueueType : byte
	{
		ChangeEmail = 0,
		ResetTwoFactor = 1,
		ResetAllTwoFactor = 2,
        WithdrawalReprocessing = 3
    }

	public enum ApprovalQueueStatus : byte
	{
		Pending = 0,
		Approved = 1,
		Rejected = 2
	}

	public enum VerificationLevel : byte
	{
		Legacy = 0,
		Level1Pending = 1,
		Level2Pending = 2,
		Level3Pending = 3,
		Level1 = 10,
		Level2 = 20,
		Level3 = 30
	}

	public enum AddressType : byte
	{
		Standard = 0,
		PaymentId = 1,
		PayloadId = 2,
		Message = 3,
		Reference = 4
	}


	public enum ExternalTransactionStatus : byte
	{
		Pending = 0,
		Canceled = 1,
		Complete = 2
	}

	public enum ExternalTransactionType : byte
	{
		Poli = 0
    }
    
	public enum AuthenticatedFeatureType : byte
	{
		Delisting = 0,
        BlacklistIP = 1
	}

	public enum ChartDistributionCount : int
	{
		Top10 = 10,
		Top100 = 100,
		Top1000 = 1000,
		Top10000 = 10000
	}

	public enum NzdtTransactionStatus : byte
	{
		Legacy = 0,

		ReadyForProcessing = 1,
		Processed = 2,

		ErrorUserNotFound = 3,
		ErrorUserNotVerified = 4,
		VoidTransaction = 5,

		FlaggedForReview = 6
	}
}