namespace Code.Core.Network.messages.server;

public class ServerCreatureToLowLevelForXp : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 325;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureToLowLevelForXp(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 325;

	private ServerCreatureToLowLevelForXp(MessageReader reader)
	{
	}

	public ServerCreatureToLowLevelForXp()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(325);
		return messageWriter.GetData();
	}
}
