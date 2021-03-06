﻿using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.AspNetUsers")]
	public class HubUser
	{
		private Guid _userId;

		[Column(Name = "Id", Storage = "_aspStringId", DbType = "nvarchar(128) NOT NULL IDENTITY", IsPrimaryKey = true)] private string _aspStringId = string.Empty;

		public Guid Id
		{
			get
			{
				if (_userId == Guid.Empty)
				{
					Guid.TryParse(_aspStringId, out _userId);
				}
				return _userId;
			}
		}

		[Column(DbType = "nvarchar(256) NOT NULL")]
		public string UserName { get; set; }

		[Column(DbType = "nvarchar(256) NOT NULL")]
		public string Email { get; set; }

		[Column(DbType = "nvarchar(256) NOT NULL")]
		public string ChatHandle { get; set; }

		[Column(DbType = "nvarchar(256) NOT NULL")]
		public string MiningHandle { get; set; }

		[Column(DbType = "nvarchar(256) NOT NULL")]
		public string Referrer { get; set; }

		[Column(DbType = "FLOAT NOT NULL")]
		public double TrustRating { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool DisableRewards { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool DisableTipNotify { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool DisablePoolNotify { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool DisableExchangeNotify { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool DisableFaucetNotify { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool DisableMarketplaceNotify { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsDisabled { get; set; }

		[Column(DbType = "datetime2 NULL")]
		public DateTime? RegisterDate { get; set; }

		[Column(DbType = "nvarchar(128) NULL")]
		public string ApiKey { get; set; }

		[Column(DbType = "nvarchar(256) NULL")]
		public string ApiSecret { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsApiEnabled { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsApiWithdrawEnabled { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsApiUnsafeWithdrawEnabled { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool DisableWithdrawEmailConfirmation { get; set; }
	}
}