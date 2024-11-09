namespace Code.Core.Network.messages.client;

public class ClientUpdatePrivacyLevel : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 199;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUpdatePrivacyLevel(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 199;

	private int privacyLevel;

	private ClientUpdatePrivacyLevel(MessageReader reader)
	{
		privacyLevel = reader.ReadInt32();
	}

	public ClientUpdatePrivacyLevel(int privacyLevel)
	{
		this.privacyLevel = privacyLevel;
	}

	public int getPrivacyLevel()
	{
		return privacyLevel;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(199);
		messageWriter.WriteInt32(privacyLevel);
		return messageWriter.GetData();
	}
}
