using System;

namespace Code.Core.Network.messages.client;

public class ClientUpdateEmote : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 56;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUpdateEmote(reader);
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

	private const int OPCODE = 56;

	private string emoteName;

	private ClientUpdateEmote(MessageReader reader)
	{
		emoteName = reader.ReadString();
	}

	public ClientUpdateEmote(string emoteName)
	{
		this.emoteName = emoteName;
	}

	public string getEmoteName()
	{
		return emoteName;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(emoteName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(56);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(emoteName);
		return messageWriter.GetData();
	}
}
