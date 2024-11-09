using System;

namespace Code.Core.Network.messages.server;

public class ServerLevelExpRecieved : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 317;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLevelExpRecieved(reader);
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

	private const int OPCODE = 317;

	private string fullLevelName;

	private int exp;

	private ServerLevelExpRecieved(MessageReader reader)
	{
		fullLevelName = reader.ReadString();
		exp = reader.ReadInt32();
	}

	public ServerLevelExpRecieved(string fullLevelName, int exp)
	{
		this.fullLevelName = fullLevelName;
		this.exp = exp;
	}

	public string getFullLevelName()
	{
		return fullLevelName;
	}

	public int getExp()
	{
		return exp;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(fullLevelName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(317);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(fullLevelName);
		messageWriter.WriteInt32(exp);
		return messageWriter.GetData();
	}
}
