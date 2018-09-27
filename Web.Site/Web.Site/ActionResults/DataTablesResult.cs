using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Web.Site.ActionResults
{
	public class DataTablesResult : ActionResult
	{
		public DataTablesResponse Data { get; set; }

		public DataTablesResult(DataTablesResponse data)
		{
			Data = data;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			HttpResponseBase response = context.HttpContext.Response;
			response.Write(JsonConvert.SerializeObject(Data));
		}
	}
}