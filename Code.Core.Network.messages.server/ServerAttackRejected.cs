namespace Code.Core.Network.messages.server;

public class ServerAttackRejected : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 117;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAttackRejected(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 117;

	private ServerAttackRejected(MessageReader reader)
	{
	}

	public ServerAttackRejected()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(117);
		return messageWriter.GetData();
	}
}
