using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Common.Pool;
using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;
using System.Web.Mvc;

namespace Cryptopia.Common.PoolWorker
{
	public class AdminPoolWorkerUpdateModel
	{
		public string Name { get; set; }

		[Required]
		public string TargetPool { get; set; }

		public List<PoolModel> Pools { get; set; }
		public AlgoType AlgoType { get; set; }
		public int Id { get; set; }

		[Required(ErrorMessage = "Password is required, most people just use 'x'")]
		[MaxLength(15, ErrorMessage = "WorkerName must be less that 15 characters long.")]
		[Display(Name = "Worker Password")]
		public string Password { get; set; }

		public double TargetDifficulty { get; set; }

		public bool IsAutoSwitch { get; set; }

		[Required]
		public string DifficultyOption { get; set; }
		public List<string> DifficultyOptions { get; set; } = new List<string>(Constant.POOL_DIFFICULTY_OPTIONS.Values);
		public double DefaultDiff { get; set; }
		public string FixedDiffSummary { get; set; }
		public string VarDiffHighSummary { get; set; }
		public string VarDiffMediumSummary { get; set; }
		public string VarDiffLowSummary { get; set; }
	}
}
