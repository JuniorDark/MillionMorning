using System;

namespace Code.Core.Network.messages.server;

public class ServerClassSelect : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 344;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerClassSelect(reader);
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

	private const int OPCODE = 344;

	private string className;

	private sbyte level;

	private sbyte response;

	private ServerClassSelect(MessageReader reader)
	{
		className = reader.ReadString();
		level = reader.ReadInt8();
		response = reader.ReadInt8();
	}

	public ServerClassSelect(string className, sbyte level, sbyte response)
	{
		this.className = className;
		this.level = level;
		this.response = response;
	}

	public string getClassName()
	{
		return className;
	}

	public sbyte getLevel()
	{
		return level;
	}

	public sbyte getResponse()
	{
		return response;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(className);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(344);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(className);
		messageWriter.WriteInt8(level);
		messageWriter.WriteInt8(response);
		return messageWriter.GetData();
	}
}
