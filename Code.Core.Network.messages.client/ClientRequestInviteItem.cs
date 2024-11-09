namespace Code.Core.Network.messages.client;

public class ClientRequestInviteItem : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 284;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestInviteItem(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 284;

	private ClientRequestInviteItem(MessageReader reader)
	{
	}

	public ClientRequestInviteItem()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(284);
		return messageWriter.GetData();
	}
}
