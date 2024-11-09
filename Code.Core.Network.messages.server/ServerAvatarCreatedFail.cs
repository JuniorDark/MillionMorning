namespace Code.Core.Network.messages.server;

public class ServerAvatarCreatedFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 27;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAvatarCreatedFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 27;

	private ServerAvatarCreatedFail(MessageReader reader)
	{
	}

	public ServerAvatarCreatedFail()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(27);
		return messageWriter.GetData();
	}
}
