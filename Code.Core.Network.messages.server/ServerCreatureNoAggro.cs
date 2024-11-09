namespace Code.Core.Network.messages.server;

public class ServerCreatureNoAggro : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 147;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureNoAggro(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 147;

	private int creatureId;

	private ServerCreatureNoAggro(MessageReader reader)
	{
		creatureId = reader.ReadInt32();
	}

	public ServerCreatureNoAggro(int creatureId)
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
		messageWriter.WriteOpCode(147);
		messageWriter.WriteInt32(creatureId);
		return messageWriter.GetData();
	}
}
