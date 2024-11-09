namespace Code.Core.Network.types;

public class SelectedClass
{
	private readonly string _className;

	private readonly sbyte _level;

	public SelectedClass(MessageReader reader)
	{
		_className = reader.ReadString();
		_level = reader.ReadInt8();
	}

	public SelectedClass(string className, sbyte level)
	{
		_className = className;
		_level = level;
	}

	public string GetClassName()
	{
		return _className;
	}

	public sbyte GetLevel()
	{
		return _level;
	}

	public int Size()
	{
		return 3 + MessageWriter.GetSize(_className);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_className);
		writer.WriteInt8(_level);
	}
}
