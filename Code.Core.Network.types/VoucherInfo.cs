namespace Code.Core.Network.types;

public class VoucherInfo
{
	private readonly short _level;

	private readonly string _iconPath;

	public VoucherInfo(MessageReader reader)
	{
		_level = reader.ReadInt16();
		_iconPath = reader.ReadString();
	}

	public VoucherInfo(short level, string iconPath)
	{
		_level = level;
		_iconPath = iconPath;
	}

	public short GetLevel()
	{
		return _level;
	}

	public string GetIconPath()
	{
		return _iconPath;
	}

	public int Size()
	{
		return 4 + MessageWriter.GetSize(_iconPath);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt16(_level);
		writer.WriteString(_iconPath);
	}
}
