namespace Code.Core.Network.messages.client;

public class ClientStopClimb : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 98;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientStopClimb(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 98;

	private ClientStopClimb(MessageReader reader)
	{
	}

	public ClientStopClimb()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(98);
		return messageWriter.GetData();
	}
}
