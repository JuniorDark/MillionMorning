using System;
using System.Collections.Generic;

namespace Code.Core.Network.messages.client;

public class ClientRequestLevelPlayerCounts : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 189;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestLevelPlayerCounts(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 189;

	private IList<string> levels;

	private ClientRequestLevelPlayerCounts(MessageReader reader)
	{
		levels = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			levels.Add(reader.ReadString());
		}
	}

	public ClientRequestLevelPlayerCounts(IList<string> levels)
	{
		this.levels = levels;
	}

	public IList<string> getLevels()
	{
		return levels;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += (short)(2 * levels.Count);
		foreach (string level in levels)
		{
			num += MessageWriter.GetSize(level);
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(189);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)levels.Count);
		foreach (string level2 in levels)
		{
			messageWriter.WriteString(level2);
		}
		return messageWriter.GetData();
	}
}
