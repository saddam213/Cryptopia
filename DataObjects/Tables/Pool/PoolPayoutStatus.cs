namespace Cryptopia.API.DataObjects
{
	public enum PoolPayoutStatus : byte
	{
		Unconfirmed = 0,
		Confirmed = 1,
		Processing = 2,
		Complete = 3,
		Orphan = 4,
		Error = 5
	}
}
