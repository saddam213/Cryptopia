using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Utilities
{
	public interface IEncryptionService
	{
		IServiceResult<EncryptionKeyPair> GenerateEncryptionKeyPair();
	}
}
