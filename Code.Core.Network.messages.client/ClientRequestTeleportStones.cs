namespace Code.Core.Network.messages.client;

public class ClientRequestTeleportStones : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 233;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestTeleportStones(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 233;

	private ClientRequestTeleportStones(MessageReader reader)
	{
	}

	public ClientRequestTeleportStones()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(233);
		return messageWriter.GetData();
	}
}
