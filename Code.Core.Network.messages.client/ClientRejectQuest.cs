namespace Code.Core.Network.messages.client;

public class ClientRejectQuest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 7;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRejectQuest(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 7;

	private ClientRejectQuest(MessageReader reader)
	{
	}

	public ClientRejectQuest()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(7);
		return messageWriter.GetData();
	}
}
