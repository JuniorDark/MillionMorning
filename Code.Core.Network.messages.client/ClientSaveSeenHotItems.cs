namespace Code.Core.Network.messages.client;

public class ClientSaveSeenHotItems : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 297;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientSaveSeenHotItems(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 297;

	private int hotItemsHashCode;

	private ClientSaveSeenHotItems(MessageReader reader)
	{
		hotItemsHashCode = reader.ReadInt32();
	}

	public ClientSaveSeenHotItems(int hotItemsHashCode)
	{
		this.hotItemsHashCode = hotItemsHashCode;
	}

	public int getHotItemsHashCode()
	{
		return hotItemsHashCode;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(297);
		messageWriter.WriteInt32(hotItemsHashCode);
		return messageWriter.GetData();
	}
}
