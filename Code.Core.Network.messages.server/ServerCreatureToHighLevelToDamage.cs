namespace Code.Core.Network.messages.server;

public class ServerCreatureToHighLevelToDamage : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 326;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureToHighLevelToDamage(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 326;

	private ServerCreatureToHighLevelToDamage(MessageReader reader)
	{
	}

	public ServerCreatureToHighLevelToDamage()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(326);
		return messageWriter.GetData();
	}
}
