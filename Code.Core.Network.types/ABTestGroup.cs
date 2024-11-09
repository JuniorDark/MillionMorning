namespace Code.Core.Network.types;

public class ABTestGroup
{
	private readonly int _testIdentifier;

	private readonly sbyte _group;

	public ABTestGroup(MessageReader reader)
	{
		_testIdentifier = reader.ReadInt32();
		_group = reader.ReadInt8();
	}

	public ABTestGroup(int testIdentifier, sbyte group)
	{
		_testIdentifier = testIdentifier;
		_group = group;
	}

	public int GetTestIdentifier()
	{
		return _testIdentifier;
	}

	public sbyte GetGroup()
	{
		return _group;
	}

	public int Size()
	{
		return 5;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_testIdentifier);
		writer.WriteInt8(_group);
	}
}
