namespace Code.Core.Network.messages.client;

public class ClientSetAfk : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 430;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientSetAfk(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 430;

	private readonly sbyte afk;

	private ClientSetAfk(MessageReader reader)
	{
		afk = reader.ReadInt8();
	}

	public ClientSetAfk(sbyte afk)
	{
		this.afk = afk;
	}

	public bool IsAfk()
	{
		return afk == 1;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(430);
		messageWriter.WriteInt8(afk);
		return messageWriter.GetData();
	}
}
