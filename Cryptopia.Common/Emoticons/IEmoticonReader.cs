using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.Emoticons
{
	public interface IEmoticonReader
	{
		Task<List<EmoticonModel>> GetEmoticons(string emoticonFile);
		Task<EmoticonModel> AdminGetEmoticon(string emoticonFile, string code);
		Task<DataTablesResponse> AdminGetEmoticons(string emoticonFile, DataTablesModel model);
	
	}
}
