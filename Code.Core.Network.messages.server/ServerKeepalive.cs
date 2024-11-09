namespace Code.Core.Network.messages.server;

public class ServerKeepalive : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 303;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerKeepalive(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 303;

	private ServerKeepalive(MessageReader reader)
	{
	}

	public ServerKeepalive()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(303);
		return messageWriter.GetData();
	}
}
