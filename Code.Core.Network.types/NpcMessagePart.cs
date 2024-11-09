using System.Collections.Generic;

namespace Code.Core.Network.types;

public class NpcMessagePart
{
	private readonly sbyte _isSayMessage;

	private readonly IList<string> _lines;

	public NpcMessagePart(MessageReader reader)
	{
		_isSayMessage = reader.ReadInt8();
		_lines = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_lines.Add(reader.ReadString());
		}
	}

	public NpcMessagePart(sbyte isSayMessage, IList<string> lines)
	{
		_isSayMessage = isSayMessage;
		_lines = lines;
	}

	public sbyte GetIsSayMessage()
	{
		return _isSayMessage;
	}

	public IList<string> GetLines()
	{
		return _lines;
	}

	public int Size()
	{
		int num = 3;
		num += (short)(2 * _lines.Count);
		foreach (string line in _lines)
		{
			num += MessageWriter.GetSize(line);
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt8(_isSayMessage);
		writer.WriteInt16((short)_lines.Count);
		foreach (string line in _lines)
		{
			writer.WriteString(line);
		}
	}
}
