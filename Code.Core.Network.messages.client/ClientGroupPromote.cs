using System;

namespace Code.Core.Network.messages.client;

public class ClientGroupPromote : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 352;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGroupPromote(reader);
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

	private const int OPCODE = 352;

	private string newLeader;

	private ClientGroupPromote(MessageReader reader)
	{
		newLeader = reader.ReadString();
	}

	public ClientGroupPromote(string newLeader)
	{
		this.newLeader = newLeader;
	}

	public string getNewLeader()
	{
		return newLeader;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(newLeader);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(352);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(newLeader);
		return messageWriter.GetData();
	}
}
