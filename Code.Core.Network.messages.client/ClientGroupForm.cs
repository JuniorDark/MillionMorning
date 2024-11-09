namespace Code.Core.Network.messages.client;

public class ClientGroupForm : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 347;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGroupForm(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 347;

	private ClientGroupForm(MessageReader reader)
	{
	}

	public ClientGroupForm()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(347);
		return messageWriter.GetData();
	}
}
