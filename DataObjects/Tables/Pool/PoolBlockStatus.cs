namespace Cryptopia.API.DataObjects
{
	public enum PoolBlockStatus : byte
	{
		Unconfirmed = 0,
		Confirmed = 1,
		Orphan = 2,
		Accounted = 3
	}
}
