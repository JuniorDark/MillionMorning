using System;

namespace Code.Core.Network.messages.client;

public class ClientGroupTravel : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 354;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGroupTravel(reader);
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

	private const int OPCODE = 354;

	private string level;

	private sbyte accepted;

	private ClientGroupTravel(MessageReader reader)
	{
		level = reader.ReadString();
		accepted = reader.ReadInt8();
	}

	public ClientGroupTravel(string level, sbyte accepted)
	{
		this.level = level;
		this.accepted = accepted;
	}

	public string getLevel()
	{
		return level;
	}

	public sbyte getAccepted()
	{
		return accepted;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(level);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(354);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(level);
		messageWriter.WriteInt8(accepted);
		return messageWriter.GetData();
	}
}
