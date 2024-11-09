namespace Code.Core.Network.messages.client;

public class ClientRequestLeaveWorldMap : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 138;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestLeaveWorldMap(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 138;

	private ClientRequestLeaveWorldMap(MessageReader reader)
	{
	}

	public ClientRequestLeaveWorldMap()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(138);
		return messageWriter.GetData();
	}
}
