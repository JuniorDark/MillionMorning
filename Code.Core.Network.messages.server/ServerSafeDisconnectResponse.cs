namespace Code.Core.Network.messages.server;

public class ServerSafeDisconnectResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 183;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerSafeDisconnectResponse(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 183;

	private ServerSafeDisconnectResponse(MessageReader reader)
	{
	}

	public ServerSafeDisconnectResponse()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(183);
		return messageWriter.GetData();
	}
}
