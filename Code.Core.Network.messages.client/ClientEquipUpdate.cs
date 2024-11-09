namespace Code.Core.Network.messages.client;

public class ClientEquipUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 60;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientEquipUpdate(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 60;

	private int item;

	private ClientEquipUpdate(MessageReader reader)
	{
		item = reader.ReadInt32();
	}

	public ClientEquipUpdate(int item)
	{
		this.item = item;
	}

	public int getItem()
	{
		return item;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(60);
		messageWriter.WriteInt32(item);
		return messageWriter.GetData();
	}
}
