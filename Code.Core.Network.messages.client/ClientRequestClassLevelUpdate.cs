namespace Code.Core.Network.messages.client;

public class ClientRequestClassLevelUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 346;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestClassLevelUpdate(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 346;

	private ClientRequestClassLevelUpdate(MessageReader reader)
	{
	}

	public ClientRequestClassLevelUpdate()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(346);
		return messageWriter.GetData();
	}
}
