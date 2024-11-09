namespace Code.Core.Network.types;

public class VoteData
{
	private readonly sbyte _voteValue;

	private readonly sbyte _voteType;

	private readonly int _voteObjectIdentifier;

	public VoteData(MessageReader reader)
	{
		_voteValue = reader.ReadInt8();
		_voteType = reader.ReadInt8();
		_voteObjectIdentifier = reader.ReadInt32();
	}

	public VoteData(sbyte voteValue, sbyte voteType, int voteObjectIdentifier)
	{
		_voteValue = voteValue;
		_voteType = voteType;
		_voteObjectIdentifier = voteObjectIdentifier;
	}

	public sbyte GetVoteValue()
	{
		return _voteValue;
	}

	public sbyte GetVoteType()
	{
		return _voteType;
	}

	public int GetVoteObjectIdentifier()
	{
		return _voteObjectIdentifier;
	}

	public int Size()
	{
		return 6;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt8(_voteValue);
		writer.WriteInt8(_voteType);
		writer.WriteInt32(_voteObjectIdentifier);
	}
}
