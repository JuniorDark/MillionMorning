using System;

namespace Code.Core.Network.messages.client;

public class ClientUpdateMood : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 58;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUpdateMood(reader);
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

	private const int OPCODE = 58;

	private string moodName;

	private sbyte persist;

	private ClientUpdateMood(MessageReader reader)
	{
		moodName = reader.ReadString();
		persist = reader.ReadInt8();
	}

	public ClientUpdateMood(string moodName, sbyte persist)
	{
		this.moodName = moodName;
		this.persist = persist;
	}

	public string getMoodName()
	{
		return moodName;
	}

	public sbyte getPersist()
	{
		return persist;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(moodName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(58);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(moodName);
		messageWriter.WriteInt8(persist);
		return messageWriter.GetData();
	}
}
