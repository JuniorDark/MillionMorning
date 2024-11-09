namespace Code.Core.Network.messages.server;

public class ServerCreatureStunned : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 167;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureStunned(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 167;

	private int creatureId;

	private ServerCreatureStunned(MessageReader reader)
	{
		creatureId = reader.ReadInt32();
	}

	public ServerCreatureStunned(int creatureId)
	{
		this.creatureId = creatureId;
	}

	public int getCreatureId()
	{
		return creatureId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(167);
		messageWriter.WriteInt32(creatureId);
		return messageWriter.GetData();
	}
}
