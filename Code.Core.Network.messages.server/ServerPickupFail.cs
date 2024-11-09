namespace Code.Core.Network.messages.server;

public class ServerPickupFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 12;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPickupFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 12;

	private ServerPickupFail(MessageReader reader)
	{
	}

	public ServerPickupFail()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(12);
		return messageWriter.GetData();
	}
}
