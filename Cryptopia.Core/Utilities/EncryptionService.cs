using Cryptopia.Common.Utilities;
using System;
using System.Security.Cryptography;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Utilities
{
	public class EncryptionService : IEncryptionService
	{

		public IServiceResult<EncryptionKeyPair> GenerateEncryptionKeyPair()
		{
			using (var cryptoProvider = new RNGCryptoServiceProvider())
			{
				var key = Guid.NewGuid().ToString("N");
				byte[] secretKeyByteArray = new byte[32]; //256 bit
				cryptoProvider.GetBytes(secretKeyByteArray);
				var secret = Convert.ToBase64String(secretKeyByteArray);
				var result = new EncryptionKeyPair
				{
					PublicKey = key,
					PrivateKey = secret
				};

				return new ServiceResult<EncryptionKeyPair>(true, result);
			}
		}

	}
}
