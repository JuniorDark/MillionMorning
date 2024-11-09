namespace Code.Core.Network.messages.server;

public class ServerUseConverterFailed : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 149;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUseConverterFailed(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 149;

	private ServerUseConverterFailed(MessageReader reader)
	{
	}

	public ServerUseConverterFailed()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(149);
		return messageWriter.GetData();
	}
}
