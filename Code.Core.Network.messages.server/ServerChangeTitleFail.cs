namespace Code.Core.Network.messages.server;

public class ServerChangeTitleFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 193;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerChangeTitleFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 193;

	private ServerChangeTitleFail(MessageReader reader)
	{
	}

	public ServerChangeTitleFail()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(193);
		return messageWriter.GetData();
	}
}
