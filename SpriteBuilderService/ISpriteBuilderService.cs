namespace SpriteBuilderService
{
	using System.ServiceModel;
	using System.Threading.Tasks;

	[ServiceContract]
	public interface ISpriteBuilderService
	{
		[OperationContract]
		Task<bool> UpdateSprites(int id);
	}
}