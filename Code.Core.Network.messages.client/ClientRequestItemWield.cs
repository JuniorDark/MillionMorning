namespace Code.Core.Network.messages.client;

public class ClientRequestItemWield : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 30;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestItemWield(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 30;

	private int itemID;

	private ClientRequestItemWield(MessageReader reader)
	{
		itemID = reader.ReadInt32();
	}

	public ClientRequestItemWield(int itemID)
	{
		this.itemID = itemID;
	}

	public int getItemID()
	{
		return itemID;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(30);
		messageWriter.WriteInt32(itemID);
		return messageWriter.GetData();
	}
}
