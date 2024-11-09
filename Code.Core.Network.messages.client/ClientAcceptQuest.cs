namespace Code.Core.Network.messages.client;

public class ClientAcceptQuest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 6;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAcceptQuest(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 6;

	private ClientAcceptQuest(MessageReader reader)
	{
	}

	public ClientAcceptQuest()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(6);
		return messageWriter.GetData();
	}
}
