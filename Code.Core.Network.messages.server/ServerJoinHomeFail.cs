namespace Code.Core.Network.messages.server;

public class ServerJoinHomeFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 247;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerJoinHomeFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 247;

	private ServerJoinHomeFail(MessageReader reader)
	{
	}

	public ServerJoinHomeFail()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(247);
		return messageWriter.GetData();
	}
}
