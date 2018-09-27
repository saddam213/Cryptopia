namespace SpriteBuilderService
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.ServiceModel;
	using System.Threading.Tasks;
	using Cryptopia.Base.Logging;
	using Cryptopia.Common.DataContext;
	using Cryptopia.Data.DataContext;
	using Cryptopia.Entity;
	using Cryptopia.Enums;
	using Cryptopia.Infrastructure.Common.DataContext;
	using DoTheSpriteThing;
	using DoTheSpriteThing.Images;

	[ServiceBehavior]
	public class SpriteBuilderService : ISpriteBuilderService
	{
		private readonly IExchangeDataContextFactory _exchangeDataContextFactory = new ExchangeDataContextFactory();
		private readonly Log _log = LoggingManager.GetLog(typeof(SpriteBuilderService));

		public async Task<bool> UpdateSprites(int currencyId)
		{
			try
			{
				var siteDirectoryPath = ConfigurationManager.AppSettings["siteDirectoryPath"];
				var imagesDirectoryPath = Path.Combine(siteDirectoryPath, ConfigurationManager.AppSettings["imagesDirectoryPath"].Replace("~\\", string.Empty));
				var spriteSheetImageOutputPath = Path.Combine(siteDirectoryPath, ConfigurationManager.AppSettings["spriteSheetImageOutputPath"].Replace("~\\", string.Empty));
				var spriteSheetCssOutputPath = Path.Combine(siteDirectoryPath, ConfigurationManager.AppSettings["spriteSheetCssOutputPath"].Replace("~\\", string.Empty));
				var spriteCssRelativeUrl = ConfigurationManager.AppSettings["spriteCssRelativeUrl"];

				List<string> symbols;
				Currency currency;

				var spriteManager = new SpriteManager();
				var images = Directory.EnumerateFiles(imagesDirectoryPath).Where(fn => (Path.GetFileNameWithoutExtension(fn) ?? string.Empty).EndsWith("-small")).ToList();

				using (var context = _exchangeDataContextFactory.CreateReadOnlyContext())
				{
					symbols = await context.Currency.Where(c => c.ListingStatus == CurrencyListingStatus.Active).Select(c => c.Symbol).ToListNoLockAsync();

					if (currencyId != 0)
					{
						currency = await context.Currency.FirstOrDefaultNoLockAsync(c => c.Id == currencyId);

						if (currency == null)
						{
							_log.Message(LogLevel.Warn, $"[UpdateSprites] - Currency with id of {currencyId} could not be found");
							return false;
						}

						for (var i = 0; i <= 600; i++)
						{
							if (images.Any(fn => (Path.GetFileNameWithoutExtension(fn) ?? string.Empty).EndsWith($"{currency.Symbol}-small"))) break;
							if (i == 600)
							{
								_log.Message(LogLevel.Warn, $"[UpdateSprites] - An image of name {currency.Symbol}-small could not be found, are you sure the image was uploaded correctly?");
								return false;
							}
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					}
				}

				// Get filepaths of images in the images directory where the file path contains one of the currency symbols in the filepath and make a FileInfo object with each of those matching paths.
				var fileSpriteImages = images.Where(f => symbols.Any(f.Contains)).Select(fn => new FileSpriteImage(new FileInfo(fn))).ToList();
				spriteManager.CreateSprite(fileSpriteImages, new SpriteSettings(spriteSheetImageOutputPath, spriteCssRelativeUrl, spriteSheetCssOutputPath));

				_log.Message(LogLevel.Info, "[UpdateSprites] - Sprite sheet updated");
				return true;
			}
			catch (Exception e)
			{
				_log.Exception("[UpdateSprites] - An exception occured while updating sprites", e);
				return false;
			}
		}
	}
}