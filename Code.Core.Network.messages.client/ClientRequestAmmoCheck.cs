namespace Code.Core.Network.messages.client;

public class ClientRequestAmmoCheck : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 213;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestAmmoCheck(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 213;

	private ClientRequestAmmoCheck(MessageReader reader)
	{
	}

	public ClientRequestAmmoCheck()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(213);
		return messageWriter.GetData();
	}
}
