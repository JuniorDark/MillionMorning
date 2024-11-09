namespace Code.Core.Network.messages.client;

public class ClientRequestLocalPlayerInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 23;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestLocalPlayerInfo(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 23;

	private ClientRequestLocalPlayerInfo(MessageReader reader)
	{
	}

	public ClientRequestLocalPlayerInfo()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(23);
		return messageWriter.GetData();
	}
}
