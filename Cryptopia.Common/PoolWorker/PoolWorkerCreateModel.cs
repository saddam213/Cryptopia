using Cryptopia.Common.Pool;
using Cryptopia.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Common.PoolWorker
{
	public class PoolWorkerCreateModel
	{
		public string FullName { get; set; }

		[Required(ErrorMessage = "WorkerName is required")]
		[MaxLength(15, ErrorMessage = "WorkerName must be less that 15 characters long.")]
		[Display(Name = "Worker Name")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Password is required, most people just use 'x'")]
		[MaxLength(15, ErrorMessage = "WorkerName must be less that 15 characters long.")]
		[Display(Name = "Worker Password")]
		public string Password { get; set; }

		[Required]
		public AlgoType? AlgoType { get; set; }

		public double TargetDifficulty { get; set; }

		public bool IsAutoSwitch { get; set; }

		[Required]
		public string DifficultyOption { get; set; }
		public List<string> DifficultyOptions { get; set; } = new List<string>(Constant.POOL_DIFFICULTY_OPTIONS.Values);

		public List<AlgoType> AlgoTypes { get; set; }
		public List<PoolConnectionModel> Connections { get; set; }



	}


}