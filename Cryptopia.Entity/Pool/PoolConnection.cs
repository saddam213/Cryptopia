using Cryptopia.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Cryptopia.Entity
{
	public class PoolConnection
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public AlgoType AlgoType { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }

		[MaxLength(128)]
		public string Host { get; set; }

		public int Port { get; set; }

		public double DefaultDiff { get; set; }

		public string DefaultPool { get; set; }

		[MaxLength(4000)]
		public string FixedDiffSummary { get; set; }

		[MaxLength(4000)]
		public string VarDiffLowSummary { get; set; }

		[MaxLength(4000)]
		public string VarDiffHighSummary { get; set; }

		[MaxLength(4000)]
		public string VarDiffMediumSummary { get; set; }

		public bool IsEnabled { get; set; }
	}
}