namespace Code.Core.Network.messages.client;

public class ClientRequestLeaveCharBuilder : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 215;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestLeaveCharBuilder(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 215;

	private ClientRequestLeaveCharBuilder(MessageReader reader)
	{
	}

	public ClientRequestLeaveCharBuilder()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(215);
		return messageWriter.GetData();
	}
}
