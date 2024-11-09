namespace Code.Core.Network.messages.client;

public class ClientRequestBecameMember : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 211;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestBecameMember(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 211;

	private ClientRequestBecameMember(MessageReader reader)
	{
	}

	public ClientRequestBecameMember()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(211);
		return messageWriter.GetData();
	}
}
