namespace Code.Core.Network.messages.server;

public class ServerMovableAttackDone : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 224;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerMovableAttackDone(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 224;

	private int id;

	private ServerMovableAttackDone(MessageReader reader)
	{
		id = reader.ReadInt32();
	}

	public ServerMovableAttackDone(int id)
	{
		this.id = id;
	}

	public int getId()
	{
		return id;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(224);
		messageWriter.WriteInt32(id);
		return messageWriter.GetData();
	}
}
