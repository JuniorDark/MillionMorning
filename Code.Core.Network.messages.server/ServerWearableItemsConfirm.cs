namespace Code.Core.Network.messages.server;

public class ServerWearableItemsConfirm : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 187;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerWearableItemsConfirm(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 187;

	private ServerWearableItemsConfirm(MessageReader reader)
	{
	}

	public ServerWearableItemsConfirm()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(187);
		return messageWriter.GetData();
	}
}
