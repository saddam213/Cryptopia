using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Emoticons
{
	public interface IEmoticonWriter
	{
		Task<IWriterResult> CreateEmoticon(string emoticonFile, CreateEmoticonModel model);
		Task<IWriterResult> AdminUpdateEmoticon(string emoticonFile, UpdateEmoticonModel model);
		Task<IWriterResult> AdminDeleteEmoticon(string emoticonFile, DeleteEmoticonModel model);
	}
}
