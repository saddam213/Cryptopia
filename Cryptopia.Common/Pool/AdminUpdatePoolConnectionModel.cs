using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;
using System.Web.Mvc;

namespace Cryptopia.Common.Pool
{
	public class AdminUpdatePoolConnectionModel
	{
		public AlgoType AlgoType { get; set; }
		public double DefaultDiff { get; set; }
		public string DefaultPool { get; set; }
	
		public string Host { get; set; }
		public string Name { get; set; }
		public List<PoolModel> Pools { get; set; }
		public int Port { get; set; }

		[AllowHtml]
		public string FixedDiffSummary { get; set; }

		[AllowHtml]
		public string VarDiffHighSummary { get; set; }

		[AllowHtml]
		public string VarDiffLowSummary { get; set; }

		[AllowHtml]
		public string VarDiffMediumSummary { get; set; }
	}
}
