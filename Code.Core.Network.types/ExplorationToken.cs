namespace Code.Core.Network.types;

public class ExplorationToken : Token
{
	public new class Factory : Token.Factory
	{
		public override Token Create(MessageReader reader)
		{
			return new ExplorationToken(reader);
		}
	}

	private readonly sbyte _isSilver;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public ExplorationToken(MessageReader reader)
		: base(reader)
	{
		_isSilver = reader.ReadInt8();
	}

	public ExplorationToken(sbyte isSilver, vector3 position, sbyte isFound)
		: base(position, isFound)
	{
		_isSilver = isSilver;
	}

	public sbyte GetIsSilver()
	{
		return _isSilver;
	}

	public override int Size()
	{
		return 14;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt8(_isSilver);
	}
}
