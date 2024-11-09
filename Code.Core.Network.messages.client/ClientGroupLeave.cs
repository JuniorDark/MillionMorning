namespace Code.Core.Network.messages.client;

public class ClientGroupLeave : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 350;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGroupLeave(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 350;

	private ClientGroupLeave(MessageReader reader)
	{
	}

	public ClientGroupLeave()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(350);
		return messageWriter.GetData();
	}
}
