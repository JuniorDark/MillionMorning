using System;

namespace Code.Core.Network.messages.client;

public class ClientFriendRemoved : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 399;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientFriendRemoved(reader);
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

	private const int OPCODE = 399;

	private string removedFriendId;

	private ClientFriendRemoved(MessageReader reader)
	{
		removedFriendId = reader.ReadString();
	}

	public ClientFriendRemoved(string removedFriendId)
	{
		this.removedFriendId = removedFriendId;
	}

	public string getRemovedFriendId()
	{
		return removedFriendId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(removedFriendId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(399);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(removedFriendId);
		return messageWriter.GetData();
	}
}
