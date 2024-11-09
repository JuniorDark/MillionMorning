namespace Code.Core.Network.types;

public class Token
{
	public class Factory
	{
		public virtual Token Create(MessageReader reader)
		{
			return new Token(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly vector3 _position;

	private readonly sbyte _isFound;

	private static readonly int TypeId;

	static Token()
	{
		ChildFactories = new Factory[5];
		TypeId = 0;
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new ExplorationToken.Factory();
		ChildFactories[2] = new CoinToken.Factory();
		ChildFactories[3] = new PremiumToken.Factory();
		ChildFactories[4] = new StaticGem.Factory();
	}

	public static Token Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return TypeId;
	}

	public Token(MessageReader reader)
	{
		_position = new vector3(reader);
		_isFound = reader.ReadInt8();
	}

	public Token(vector3 position, sbyte isFound)
	{
		_position = position;
		_isFound = isFound;
	}

	public vector3 GetPosition()
	{
		return _position;
	}

	public sbyte GetIsFound()
	{
		return _isFound;
	}

	public virtual int Size()
	{
		return 13;
	}

	public virtual void Write(MessageWriter writer)
	{
		_position.Write(writer);
		writer.WriteInt8(_isFound);
	}
}
