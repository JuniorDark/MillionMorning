namespace Code.Core.Network.messages.server;

public class ServerExpUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 307;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerExpUpdate(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 307;

	private int exp;

	private ServerExpUpdate(MessageReader reader)
	{
		exp = reader.ReadInt32();
	}

	public ServerExpUpdate(int exp)
	{
		this.exp = exp;
	}

	public int getExp()
	{
		return exp;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(307);
		messageWriter.WriteInt32(exp);
		return messageWriter.GetData();
	}
}
