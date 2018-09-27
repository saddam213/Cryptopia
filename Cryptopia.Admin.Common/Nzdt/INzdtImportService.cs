using Cryptopia.Infrastructure.Common.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Nzdt
{
	public interface INzdtImportService
	{
		Task<IServiceResult<NzdtUploadResultModel>> ValidateAndUpload(string adminUserId, Stream inputStream);
	}
}
