using System;

namespace Code.Core.Network.messages.client;

public class ClientClassSelect : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 343;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientClassSelect(reader);
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

	private const int OPCODE = 343;

	private string className;

	private sbyte level;

	private ClientClassSelect(MessageReader reader)
	{
		className = reader.ReadString();
		level = reader.ReadInt8();
	}

	public ClientClassSelect(string className, sbyte level)
	{
		this.className = className;
		this.level = level;
	}

	public string getClassName()
	{
		return className;
	}

	public sbyte getLevel()
	{
		return level;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(className);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(343);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(className);
		messageWriter.WriteInt8(level);
		return messageWriter.GetData();
	}
}
