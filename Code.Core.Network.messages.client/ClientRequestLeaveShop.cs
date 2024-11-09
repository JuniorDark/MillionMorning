namespace Code.Core.Network.messages.client;

public class ClientRequestLeaveShop : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 133;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestLeaveShop(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 133;

	private ClientRequestLeaveShop(MessageReader reader)
	{
	}

	public ClientRequestLeaveShop()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(133);
		return messageWriter.GetData();
	}
}
