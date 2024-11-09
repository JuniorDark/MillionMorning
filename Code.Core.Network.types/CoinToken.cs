namespace Code.Core.Network.types;

public class CoinToken : Token
{
	public new class Factory : Token.Factory
	{
		public override Token Create(MessageReader reader)
		{
			return new CoinToken(reader);
		}
	}

	private const int TYPE_ID = 2;

	public override int GetTypeId()
	{
		return 2;
	}

	public CoinToken(MessageReader reader)
		: base(reader)
	{
	}

	public CoinToken(vector3 position, sbyte isFound)
		: base(position, isFound)
	{
	}

	public override int Size()
	{
		return 13;
	}
}
