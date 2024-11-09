namespace Code.Core.Network.messages.server;

public class ServerRequestHomeBoxPosition : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 300;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerRequestHomeBoxPosition(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 300;

	private ServerRequestHomeBoxPosition(MessageReader reader)
	{
	}

	public ServerRequestHomeBoxPosition()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(300);
		return messageWriter.GetData();
	}
}
