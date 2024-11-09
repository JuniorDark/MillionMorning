namespace Code.Core.Network.messages.client;

public class ClientRequestRespawn : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 92;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestRespawn(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 92;

	private ClientRequestRespawn(MessageReader reader)
	{
	}

	public ClientRequestRespawn()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(92);
		return messageWriter.GetData();
	}
}
