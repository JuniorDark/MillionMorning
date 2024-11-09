namespace Code.Core.Network.messages.server;

public class ServerChangeLevelFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 48;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerChangeLevelFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 48;

	private ServerChangeLevelFail(MessageReader reader)
	{
	}

	public ServerChangeLevelFail()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(48);
		return messageWriter.GetData();
	}
}
