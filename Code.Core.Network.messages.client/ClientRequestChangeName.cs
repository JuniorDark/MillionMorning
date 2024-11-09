using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestChangeName : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 333;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestChangeName(reader);
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

	private const int OPCODE = 333;

	private string newName;

	private int itemId;

	private ClientRequestChangeName(MessageReader reader)
	{
		newName = reader.ReadString();
		itemId = reader.ReadInt32();
	}

	public ClientRequestChangeName(string newName, int itemId)
	{
		this.newName = newName;
		this.itemId = itemId;
	}

	public string getNewName()
	{
		return newName;
	}

	public int getItemId()
	{
		return itemId;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(newName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(333);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(newName);
		messageWriter.WriteInt32(itemId);
		return messageWriter.GetData();
	}
}
