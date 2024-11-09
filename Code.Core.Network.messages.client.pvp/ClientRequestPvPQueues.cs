namespace Code.Core.Network.messages.client.pvp;

public class ClientRequestPvPQueues : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 433;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestPvPQueues(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 433;

	private ClientRequestPvPQueues(MessageReader reader)
	{
	}

	public ClientRequestPvPQueues()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(433);
		return messageWriter.GetData();
	}
}
