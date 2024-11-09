namespace Code.Core.Network.messages.server;

public class ServerPing : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 182;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPing(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	public const int OPCODE = 182;

	private ServerPing(MessageReader reader)
	{
	}

	public ServerPing()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(182);
		return messageWriter.GetData();
	}
}
