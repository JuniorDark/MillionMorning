namespace Code.Core.Network.messages.client;

public class ClientRequestCheckForNewHomeItems : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 270;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestCheckForNewHomeItems(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 270;

	private ClientRequestCheckForNewHomeItems(MessageReader reader)
	{
	}

	public ClientRequestCheckForNewHomeItems()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(270);
		return messageWriter.GetData();
	}
}
