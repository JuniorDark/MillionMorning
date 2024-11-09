namespace Code.Core.Network.messages.server;

public class ServerQuestAccepted : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 8;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerQuestAccepted(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 8;

	private ServerQuestAccepted(MessageReader reader)
	{
	}

	public ServerQuestAccepted()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(8);
		return messageWriter.GetData();
	}
}
