namespace Code.Core.Network.messages.client;

public class ClientLeaveGameplayObject : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 102;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientLeaveGameplayObject(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 102;

	private ClientLeaveGameplayObject(MessageReader reader)
	{
	}

	public ClientLeaveGameplayObject()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(102);
		return messageWriter.GetData();
	}
}
